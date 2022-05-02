// -----------------------------------------------------------------------
// <copyright file="AccountController.cs" company="Axzes">
// This source code is Copyright Axzes and MAY NOT be copied, reproduced,
// published, distributed or transmitted to or stored in any manner without prior
// written consent from Axzes (https://www.axzes.com).
// </copyright>
// -----------------------------------------------------------------------


using AutoMapper;
using MGCap.Business.Abstract.ApplicationServices;
using MGCap.Domain.Enums;
using MGCap.Domain.Models;
using MGCap.Domain.ViewModels.Account;
using MGCap.Presentation.Extensions;
using MGCap.Presentation.ViewModels.AccountViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using TimeZoneConverter;

namespace MGCap.Presentation.Controllers
{
    public class AccountController : BaseController
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IContactsApplicationService _contactsApplicationService;
        private IHttpContextAccessor HttpContextAccessor;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            ISmsSender smsSender,
            ILoggerFactory loggerFactory,
            IConfiguration configuration,
            IContactsApplicationService contactsApplicationService,
            IEmployeesApplicationService employeeAppService,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor
            ) : base(employeeAppService, mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _contactsApplicationService = contactsApplicationService;
            _emailSender = emailSender;
            _smsSender = smsSender;
            HttpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                IActionResult response = StatusCode(
                                    (int)HttpStatusCode.Unauthorized,
                                    new
                                    {
                                        error_code = ErrorCode.Unauthorized,
                                        error_message = "Your username or password is incorrect"
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
                    string AppId = HttpContextAccessor?.HttpContext?.Request?.Headers["AppId"];
                    AppId = string.IsNullOrEmpty(AppId) ? "Web Portal": AppId;

					DateTime now = DateTime.UtcNow;
                    TimeZoneInfo tzi = TZConvert.GetTimeZoneInfo("America/New_York");
                    now = TimeZoneInfo.ConvertTimeFromUtc(now, tzi);
					if (tzi.IsDaylightSavingTime(now))
					{
						now.AddHours(-1);
					}
					UserLoginInfo info = new UserLoginInfo(AppId, now.ToString("yyyy-MM-dd HH:mm:ss.fff"), appUser.Email);

                    await _userManager.AddLoginAsync(appUser, info);
                    var token = await GenerateJwtTokenAsync(appUser);
                    // If the user exists but doesn't have any association with any company
                    if (token.Count == 0)
                    {
                        return response;
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
                error_code = ErrorCode.BadRequest,
                error_message = message.Equals(string.Empty) ? "Bad request" : message
            });
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                var user = this._userManager.Users
                                            .Where(u => u.Email == model.Email)
                                            .FirstOrDefault();
                string code = "";
                string confirmCode = "";
                // Aspnet User creation
                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        UserName = model.Email,
                        Email = model.Email,
                    };
                    var result = await this._userManager.CreateAsync(user);
                    if (result.Succeeded)
                    {
                        // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
                        // Send an email with this link
                        code = await this._userManager.GeneratePasswordResetTokenAsync(user);
                        confirmCode = await this._userManager.GenerateEmailConfirmationTokenAsync(user);
                        var paramaters = new Dictionary<string, string>
                        {
                            { "userId", user.Id },
                            { "code", code },
                            { "confirm", confirmCode }
                        };
                        var callbackUrl = QueryHelpers.AddQueryString($"{this.AppBaseUrl}auth/mail-confirm", paramaters);

                        string plainText = "Click the link to define your password: " + callbackUrl;

                        // Should we await for this? I mean, I think all the email sending has to be
                        // non-blocking
                        //await _emailSender.SendEmailAsync(
                        //   model.Email,
                        //   "Confirm your account",
                        //   plainTextMessage: plainText);
                    }
                }

                // Employee Creation
                if (!EmployeeApplicationService.Exists(model.Email))
                {
                    try
                    {
                        var employeeObj = this.Mapper.Map<RegisterViewModel, Employee>(model);
                        var contactObj = this.Mapper.Map<RegisterViewModel, Contact>(model);
                        await _contactsApplicationService.AddAsync(contactObj);
                        ContactEmail contactEmail = new ContactEmail() { Email = model.Email, Type = "Work", Default = true , ContactId = contactObj.ID };
                        await _contactsApplicationService.AddEmailAsync(contactEmail);
                        await _contactsApplicationService.SaveChangesAsync();
                        employeeObj.ContactId = contactObj.ID;
                        await this.EmployeeApplicationService.AddAsync(employeeObj);
                        await this.EmployeeApplicationService.SaveChangesAsync();
                        employeeObj = this.EmployeeApplicationService.AssignRolePermissions(employeeObj);
                        await this.EmployeeApplicationService.SaveChangesAsync();
                    }
                    catch (Exception)
                    {
                        // Fail silently for the moment
                        return BadRequest(this.ModelState);
                    }
                }

                return Ok();
            }
            // If we got this far, something failed
            return BadRequest(this.ModelState);
        }

        // GET: /Account/ConfirmEmail
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string confirm, string code)
        {
            var values = new List<string> { userId, confirm, code };
            if (values.Any(string.IsNullOrEmpty))
            {
                return BadRequest(this.ModelState);
            }

            var user = await this._userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return BadRequest(this.ModelState);
            }

            var result = await this._userManager.ConfirmEmailAsync(user, confirm);

            if (!result.Succeeded)
            {
                return BadRequest(this.ModelState);
            }

            return new JsonResult(new { email = user.Email });
        }

        // GET: /Account/UserEmail?userId=<code>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetUserEmail(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(this.ModelState);
            }

            var user = await this._userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return BadRequest(this.ModelState);
            }

            return new JsonResult(new { email = user.Email });
        }

        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }

            var user = await this._userManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return BadRequest(this.ModelState);
            }

            var result = await this._userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return Ok();
            }

            return BadRequest(this.ModelState);
        }

        // POST: /Account/ForgotPassword
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

                var paramaters = new Dictionary<string, string> { { "userId", user.Id }, { "code", code } };

                var callbackUrl = QueryHelpers.AddQueryString($"{this.AppBaseUrl}auth/reset-password", paramaters);

                string plainText = "Click the link to reset your password: " + callbackUrl;

                await this._emailSender.SendEmailAsync(model.Email, "Reset Password", plainTextMessage: plainText);

                return Ok();
            }

            // If we got this far, something failed, redisplay form
            return BadRequest(this.ModelState);
        }

        // POST: /Account/SendCredentials
        [HttpPost]
        public async Task<IActionResult> SendCredentials([FromBody] EmailViewModel obj)
        {
            var email = obj.Email;
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest(this.ModelState);
            }

            var user = await this._userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return BadRequest(this.ModelState);
            }

            if (user.EmailConfirmed)
            {
                return await ForgotPassword(new ForgotPasswordViewModel { Email = user.Email });
            }

            var code = await this._userManager.GeneratePasswordResetTokenAsync(user);
            var confirmCode = await this._userManager.GenerateEmailConfirmationTokenAsync(user);
            var paramaters = new Dictionary<string, string>
                        {
                            { "userId", user.Id },
                            { "code", code },
                            { "confirm", confirmCode }
                        };
            var callbackUrl = QueryHelpers.AddQueryString($"{this.AppBaseUrl}auth/mail-confirm", paramaters);

            string plainText = "Click the link to define your password: " + callbackUrl;

            // Should we await for this? I mean, I think all the email sending has to be
            // non-blocking
            await _emailSender.SendEmailAsync(
               user.Email,
               "Confirm your account",
               plainTextMessage: plainText);
            return Ok();
        }

        private async Task<LoginResponseViewModel> GenerateJwtTokenAsync(ApplicationUser user)
        {
            var employeesObjs = await this.EmployeeApplicationService.ReadWithCompanyDapperAsync(user.Email) ?? new List<EmployeeLoginReponseViewModel>();

            var companiesIds = employeesObjs
                                ?.Select(e => e.CompanyId.ToString())
                                ?.Aggregate((c1, c2) => $"{c1},{c2}")
                                ?? "";

            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("companies", companiesIds)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this._configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(this._configuration["Jwt:Issuer"],
              this._configuration["Jwt:Issuer"],
              claims,
              expires: DateTime.Now.AddMinutes(int.Parse(this._configuration["Jwt:ExpireMinutes"])),
              signingCredentials: creds);

            return new LoginResponseViewModel
            {
                Token = (new JwtSecurityTokenHandler()).WriteToken(token),
                Employees = employeesObjs
            };
        }

        #region Email
        private async Task<string> GetWelcomeEmailTemplate(string url)
        {
            var body = string.Empty;
            using (var client = new HttpClient())
            {
                body = await client.GetStringAsync(this.Url.AbsoluteContent("/MailTemplates/userDefinePassword.html"));
            }

            return body.Replace("#url#", url);
        }

        private async Task<string> GetResetPasswordEmailTemplate(string url)
        {
            var body = string.Empty;
            using (var client = new HttpClient())
            {
                body = await client.GetStringAsync(this.Url.AbsoluteContent("/MailTemplates/userResetPassword.html"));
            }

            return body.Replace("#url#", url);
        }
        #endregion

    }
}
