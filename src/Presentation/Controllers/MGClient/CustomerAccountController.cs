using AutoMapper;
using MGCap.Business.Abstract.ApplicationServices;
using MGCap.Domain.Enums;
using MGCap.Domain.Models;
using MGCap.Domain.Options;
using MGCap.Domain.ViewModels.Account;
using MGCap.Presentation.ViewModels.AccountViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TimeZoneConverter;

namespace MGCap.Presentation.Controllers.MGClient
{

    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    public class CustomerAccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly ILogger _logger;
        private readonly JwtOptions _jwtOptions;
        private readonly ICustomerUserService _customerUserService;
        private readonly IContactsApplicationService _contactsApplicationService;
        private IHostingEnvironment _hostingEnvironment;
        private readonly IMobileAppVersionService _mobileAppVersionService;

        public string AppBaseUrl => $"{this.Request.Scheme}://{this.Request.Host}/";

        public CustomerAccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            ISmsSender smsSender,
            ILoggerFactory loggerFactory,
            IOptions<JwtOptions> jwtOptions,
            ICustomerUserService customerUserService,
            IContactsApplicationService contactsApplicationService,
            IMapper mapper,
            IHostingEnvironment environment,
            IMobileAppVersionService mobileAppVersionService
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtOptions = jwtOptions.Value;
            _emailSender = emailSender;
            _smsSender = smsSender;
            _customerUserService = customerUserService;
            _contactsApplicationService = contactsApplicationService;
            _hostingEnvironment = environment;
            _mobileAppVersionService = mobileAppVersionService;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    IActionResult response = StatusCode((int)HttpStatusCode.Unauthorized,
                        new
                        {
                            ErrorCode = ErrorCode.Unauthorized,
                            ErrorMessage = "Your username or password is incorrect"
                        });

                    // Require the user to have a confirmed email before they can log on.
                    var user = await this._userManager.FindByNameAsync(model.Email);
                    if (user == null || !(await this._userManager.IsEmailConfirmedAsync(user)))
                    {
                        return response;
                    }

                    // This doesn't count login failures towards account lockout
                    // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                    var result = await this._signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                    if (result.IsLockedOut)
                    {
                        this._logger.LogWarning(2, "User account locked out.");
                    }

                    if (result.Succeeded)
                    {
                        var appUser = _userManager.Users.SingleOrDefault(r => r.Email == model.Email);
                        var token = await GetCustomerUserWithJwtTokenAsync(appUser);

						DateTime now = DateTime.UtcNow;
                        TimeZoneInfo tzi = TZConvert.GetTimeZoneInfo("America/New_York");
                        now = TimeZoneInfo.ConvertTimeFromUtc(now, tzi);
						if (tzi.IsDaylightSavingTime(now))
						{
							now.AddHours(-1);
						}
						UserLoginInfo info = new UserLoginInfo("MG Client", now.ToString("yyyy-MM-dd HH:mm:ss.fff"), appUser.Email);
                        await _userManager.AddLoginAsync(appUser, info);

                        //If the user exists but doesn't have any association with any company
                        if (string.IsNullOrEmpty(token.AccessToken))
                        {
                            return response;
                        }

                        // The user is not a Customer so create new Customers
                        if (token.Contact.CustomerUserId <= 0)
                        {
                            Contact contact = await _contactsApplicationService.GetContactByEmail(appUser.Email);
                            if (contact != null)
                            {
                                CustomerUser customerUser = new CustomerUser();
                                customerUser.ContactId = contact.ID;
                                customerUser.CompanyId = contact.CompanyId;
                                customerUser.Email = appUser.Email;
                                customerUser.FirstName = contact.FirstName;
                                customerUser.LastName = contact.LastName;
                                customerUser.MiddleName = contact.MiddleName;
                                customerUser.IsActive = true;

                                customerUser = await _customerUserService.AddAsync(customerUser);

                                int count = await _customerUserService.SaveChangesAsync();

                                if (count == 0)
                                {
                                    return NoContent();
                                }
                                else
                                {
                                    token = await GetCustomerUserWithJwtTokenAsync(appUser);
                                }
                            }
                        }

						response = new JsonResult(token);
                    }

                    return response;
                }

                // If we got this far, something failed
                List<string> messages = new List<string>();
                ModelState.Values.ToList().ForEach(v => messages.AddRange(v.Errors.Select(e => e.ErrorMessage)));

                string message = string.Join(" | ", messages);

                return BadRequest(new
                {
                    ErrorCode = ErrorCode.BadRequest,
                    ErrorMessage = message.Equals(string.Empty) ? "Bad request" : message
                });
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("client.account.login: {0}", ex.Message);
#endif
                return NoContent();
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SignUp([FromBody] CustomerUserSignUpViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var customerContact = await _customerUserService.GetCustomerContactByEmail(model.Email);

                    if (customerContact.IsValid() == false)
                    {
                        return StatusCode(
                            (int)HttpStatusCode.PreconditionFailed,
                            new
                            {
                                ErrorCode = ErrorCode.PreconditionFailed,
                                ErrorMessage = "This email is not linked to any customer."
                            });
                    }

                    var user = _userManager.Users.FirstOrDefault(u => u.Email.Equals(model.Email, StringComparison.InvariantCultureIgnoreCase));
                    if (user != null)
                    {
                        if (user.EmailConfirmed)
                        {

                            // create a record into CustomerUser table
                            try
                            {
                                var exists = await _customerUserService.ExistsAsync(cu => cu.CompanyId.Equals(1) &&
                                                                                          cu.Email.Equals(model.Email, StringComparison.InvariantCultureIgnoreCase));
                                if (exists == false)
                                {
                                    var customerUser = Mapper.Map<CustomerUserSignUpViewModel, CustomerUser>(model);
                                    customerUser.ContactId = customerContact.ContactId;
                                    customerUser.CompanyId = 1;
                                    customerUser.IsActive = true;

                                    customerUser = await _customerUserService.AddAsync(customerUser);

                                    int count = await _customerUserService.SaveChangesAsync();

                                    if (count == 0)
                                    {
                                        return NoContent();
                                    }
                                }
                                else
                                {
                                    return StatusCode(
                                        (int)HttpStatusCode.ExpectationFailed,
                                        new
                                        {
                                            ErrorCode = ErrorCode.ExpectationFailed,
                                            ErrorMessage = "There is already a customer user with this email."
                                        });
                                }
                            }
                            catch (Exception ex)
                            {
#if DEBUG
                                Console.WriteLine("client.account.signup.create_customer_user: {0}", ex.Message);
#endif
                                throw ex;
                            }
                        }
                        else
                        {
                            await _userManager.DeleteAsync(user);
                        }

                    }

                    // create the user
                    user = new ApplicationUser
                    {
                        Email = model.Email.ToLowerInvariant(),
                        UserName = model.Email.ToLowerInvariant()
                    };

                    var result = await _userManager.CreateAsync(user);
                    if (result.Succeeded)
                    {
                        var passwordResult = await _userManager.AddPasswordAsync(user, model.Password);
                        if (passwordResult.Succeeded)
                        {
                            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                            var parameters = new Dictionary<string, string>
                            {
                                ["userId"] = user.Id,
                                ["code"] = Guid.NewGuid().ToString(),
                                ["confirm"] = token
                            };

                            var callbackUrl = QueryHelpers.AddQueryString($"{this.AppBaseUrl}auth/customer-mail-confirm", parameters);

                            string plainText = GetSignUpEmailTemplate(callbackUrl);

                            // create a record into CustomerUser table
                            try
                            {
                                var exists = await _customerUserService.ExistsAsync(cu => cu.CompanyId.Equals(1) &&
                                                                                          cu.Email.Equals(model.Email, StringComparison.InvariantCultureIgnoreCase));
                                if (exists == false)
                                {
                                    var customerUser = Mapper.Map<CustomerUserSignUpViewModel, CustomerUser>(model);
                                    customerUser.ContactId = customerContact.ContactId;
                                    customerUser.CompanyId = 1;
                                    customerUser.IsActive = true;

                                    customerUser = await _customerUserService.AddAsync(customerUser);

                                    int count = await _customerUserService.SaveChangesAsync();

                                    if (count == 0)
                                    {
                                        return NoContent();
                                    }
                                }
                                //else
                                //{
                                //    return StatusCode(
                                //        (int)HttpStatusCode.ExpectationFailed,
                                //        new
                                //        {
                                //            ErrorCode = ErrorCode.ExpectationFailed,
                                //            ErrorMessage = "There is already a customer user with this email."
                                //        });
                                //}
                            }
                            catch (Exception ex)
                            {
#if DEBUG
                                Console.WriteLine("client.account.signup.create_customer_user: {0}", ex.Message);
#endif
                                throw ex;
                            }

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

                            _emailSender.SendEmailAsync(model.DebugEmail, "DEBUG: Confirm your account", plainTextMessage: "-", plainText);
                            _emailSender.SendEmailAsync(model.Email, "Confirm your account", plainTextMessage: "-", plainText);

#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

                            return Ok();
                        }
                        else
                        {
                            return NoContent();
                        }
                    }
                    else
                    {
                        return NoContent();
                    }
                }

                // If we got this far, something failed
                List<string> messages = new List<string>();
                ModelState.Values.ToList().ForEach(v => messages.AddRange(v.Errors.Select(e => e.ErrorMessage)));

                string message = string.Join(" | ", messages);

                return BadRequest(new
                {
                    error_code = ErrorCode.BadRequest,
                    error_message = message.Equals(string.Empty) ? "Bad request" : message
                });
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("client.account.signup: {0}", ex.Message);
#endif
                return NoContent();
            }
        }




        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody]ForgotPasswordViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                var user = await this._userManager.FindByNameAsync(model.Email);
                if (user == null || !(await this._userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return BadRequest(this.ModelState);
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
                // Send an email with this link
                var code = await this._userManager.GeneratePasswordResetTokenAsync(user);       

                var parameters = new Dictionary<string, string>
                {
                    ["userId"] = user.Id,
                    ["code"] = code
                };

                var callbackUrl = QueryHelpers.AddQueryString($"{this.AppBaseUrl}auth/customer-reset-password", parameters);

                string plainText = GetResetPasswordEmailTemplate(callbackUrl);

                await this._emailSender.SendEmailAsync(model.Email, "Reset Password", "-", plainText);

                return Ok();
            }

            return BadRequest(this.ModelState);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> EmailInvitedCustomer([FromBody]EmailViewModel   model)
        {

            var email = model.Email;
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest(this.ModelState);
            }

            Contact contact =  await _contactsApplicationService.GetContactByEmail(email);
			if (contact != null)
			{

                MobileAppVersion ios = await _mobileAppVersionService.Latest(MobileApp.MGClient, MobilePlatform.IOS);
                MobileAppVersion android = await _mobileAppVersionService.Latest(MobileApp.MGClient, MobilePlatform.Android);

                var parameters = new Dictionary<string, string>
				{
					["Name"] = contact.FullName,
					["Email"] = email,
                    ["Ios"] = ios.Url,
                    ["Android"] = android.Url
                };

                
                string plainText = GetInvitedCustomerEmailTemplate(parameters);

                await this._emailSender.SendEmailAsync(model.Email, "You have been invited to use MG Capital App.", "-", plainText);

                return Ok();

            }

           

            return BadRequest(this.ModelState);
        }

        #region Private

        private async Task<CustomerUserLoginResponseViewModel> GetCustomerUserWithJwtTokenAsync(ApplicationUser user)
        {
            var contactObj = await _customerUserService.GetWithCompanyDapperAsync(user.Email) ?? new CustomerLoginResponseViewModel();

            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("contact", contactObj.GetHashCode().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(this._jwtOptions.Issuer,
              _jwtOptions.Issuer,
              claims,
              expires: DateTime.Now.AddMinutes(this._jwtOptions.ExpireMinutes),
              signingCredentials: creds);

            return new CustomerUserLoginResponseViewModel
            {
                AccessToken = (new JwtSecurityTokenHandler()).WriteToken(token),
                Contact = contactObj
            };
        }

        #endregion


        #region Email Template

        private string GetSignUpEmailTemplate(string url)
        {

            var path = Path.Combine(
                _hostingEnvironment.ContentRootPath,
                "Resources",
                "SignUpEmailTemplate.html");

            var body = System.IO.File.ReadAllText(path);
            body = body.Replace("#url#", url);
            return body;
        }

        private string GetResetPasswordEmailTemplate(string url)
        {

            var path = Path.Combine(
                _hostingEnvironment.ContentRootPath,
                "Resources",
                "ResetPasswordEmailTemplate.html");

            var body = System.IO.File.ReadAllText(path);
            body = body.Replace("#url#", url);
            return body;
        }

        private string GetInvitedCustomerEmailTemplate(Dictionary<string, string> param)
        {

            var path = Path.Combine(
                _hostingEnvironment.ContentRootPath,
                "Resources",
                "InvitedCustomersEmailTemplate.html");

            var body = System.IO.File.ReadAllText(path);
            body = body.Replace("#FULLNAME#", param["Name"]);
            body = body.Replace("#IOS#", param["Ios"]);
            body = body.Replace("#ANDROID#", param["Android"]);
            return body;
        }

        #endregion
    }
}
