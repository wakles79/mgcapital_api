using MGCap.Business.Abstract.ApplicationServices;
using MGCap.Business.Implementation.OneSignal;
using MGCap.DataAccess.Abstract.Repository;
using MGCap.Domain.Enums;
using MGCap.Domain.Models;
using MGCap.Domain.Options;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.PushNotifications;
using MGCap.Domain.ViewModels.WorkOrder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OneSignal.RestAPIv3.Client;
using OneSignal.RestAPIv3.Client.Resources;
using OneSignal.RestAPIv3.Client.Resources.Notifications;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace MGCap.Business.Implementation.ApplicationServices
{
    public class PushNotificationService : IPushNotificationService
    {
        private readonly int CompanyId;
        private readonly string UserEmail;
        private readonly IPushNotificationRepository PushNotificationRepository;
        private readonly OneSignalOptions OneSignalOptions;

        public PushNotificationService(
            IHttpContextAccessor httpContext,
            IPushNotificationRepository pushNotificationRepository,
            IOptions<OneSignalOptions> options)
        {
            this.PushNotificationRepository = pushNotificationRepository;
            this.OneSignalOptions = options.Value;

            string companyIdStr = httpContext?.HttpContext?.Request?.Headers["CompanyId"];
            this.CompanyId = string.IsNullOrEmpty(companyIdStr) ? 1 : int.Parse(companyIdStr);
            this.UserEmail = httpContext?.HttpContext?.User?.FindFirst("sub")?.Value?.Trim() ?? "Undefined";
            this._strTimezoneOffset = httpContext?.HttpContext?.Request?.Headers["TimezoneOffset"];
        }

        private readonly string _strTimezoneOffset;
        /// <summary>
        ///     Gets the client's UTC Offset in minutes
        /// </summary>
        public string StrTimezoneOffset => string.IsNullOrEmpty(this._strTimezoneOffset) ? "300" : this._strTimezoneOffset;

        public int TimezonOffset => (int)Math.Round(double.Parse(this.StrTimezoneOffset));

        public async Task CreateNotification(PushNotificationCreateViewModel viewModel)
        {
            try
            {
                var template = viewModel.Templates.FirstOrDefault();

                if (template == null)
                {
                    return;
                }

                Tuple<string, string> tuple;
                PushNotificationReason reason;

                if (viewModel.WorkOrder.StatusId.Equals(WorkOrderStatus.StandBy))
                {
                    reason = PushNotificationReason.Work_Order_Created;
                    tuple = this.NewWorkOrderPushNotification(template, viewModel.WorkOrder);
                }
                else
                {
                    reason = PushNotificationReason.Work_Order_Closed;
                    tuple = this.ClosedWorkOrderPusnNotification(template, viewModel.WorkOrder);
                }

                var options = new NotificationCreateOptions
                {
                    AppId = new Guid(this.OneSignalOptions.AppId),
                    Filters = this.CreateOneSignalFilters(viewModel.Recipients.Select(r => r.Email)),
                    Data = new Dictionary<string, string>() { ["WorkOrderId"] = viewModel.WorkOrder.ID.ToString() },
                    ActionButtons = new List<ActionButtonField> { new ActionButtonField { Id = "id1", Text = "See Details" } }
                };

                options.Headings.Add(LanguageCodes.English, tuple.Item1);
                options.Contents.Add(LanguageCodes.English, tuple.Item2);

                //Validate before Send
                bool sendBefore = await this.PushNotificationRepository.Exists(tuple.Item2, JsonConvert.SerializeObject(options.Data), tuple.Item1, reason);

                if (!sendBefore)
                {
                    var oneSignalClient = new OneSignalClient(this.OneSignalOptions.ApiKey);
                    var result = oneSignalClient.Notifications.Create(options);

                    var saveViewModel = new PushNotificationSaveViewModel
                    {
                        Result = result,
                        Options = options,
                        DataType = PushNotificationDataType.WorkOrder,
                        Reason = reason
                    };

                    await SaveNotification(saveViewModel);
                }
            }
            catch (SqlException sEx)
            {
                throw sEx;
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine(ex.Message);
#endif
                throw new OneSignalException(ex);
            }
        }

        public async Task CreateScheduledNotifications(IEnumerable<WorkOrderNotificationTemplate> templates)
        {
            try
            {
                var sentAfter = DateTime.Today.AddHours(7);
                var oneSignalClient = new OneSignalClient(this.OneSignalOptions.ApiKey);
                var workOrdersDueDateVM = await this.PushNotificationRepository.ReadAllByDueDateAsync(this.CompanyId, this.TimezonOffset);

                var templateDueToday = templates.FirstOrDefault(t => t.WorkOrderStatusId == 32);
                if (templateDueToday != null)
                {
                    foreach (var item in workOrdersDueDateVM)
                    {
                        var options = this.CreateDueTodayNotification(item, templateDueToday, sentAfter);
                        var result = oneSignalClient.Notifications.Create(options);

                        var saveViewModel = new PushNotificationSaveViewModel
                        {
                            Result = result,
                            Options = options,
                            DataType = PushNotificationDataType.Scheduled,
                            Reason = PushNotificationReason.Work_Orders_Due_Today
                        };

                        await this.SaveNotification(saveViewModel);
                    }
                }

                var templatePastDue = templates.FirstOrDefault(t => t.WorkOrderStatusId == 64);
                if (templatePastDue != null)
                {
                    foreach (var item in workOrdersDueDateVM)
                    {
                        var options = this.CreatePastDueNotification(item, templatePastDue, sentAfter);

                        string Contents;
                        options.Contents.TryGetValue(LanguageCodes.English,out Contents);
                        string Headings;
                        options.Headings.TryGetValue(LanguageCodes.English, out Headings);
                        bool sendBefore = await this.PushNotificationRepository.Exists(Contents, JsonConvert.SerializeObject(options.Data), Headings, PushNotificationReason.Work_Orders_Past_Due);

                        if (!sendBefore)
                        {

                            var result = oneSignalClient.Notifications.Create(options);

                            var saveViewModel = new PushNotificationSaveViewModel
                            {
                                Result = result,
                                Options = options,
                                DataType = PushNotificationDataType.Scheduled,
                                Reason = PushNotificationReason.Work_Orders_Past_Due
                            };

                            await this.SaveNotification(saveViewModel);

                        }
                    }
                }
            }
            catch (SqlException sEx)
            {
                Log.Warning(sEx, "SqlException: Error on sending scheduled notification");
                throw sEx;
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine(ex.Message);
#endif
                Log.Warning(ex, "Exception: Error on sending scheduled notification");
                throw new OneSignalException(ex);
            }
        }

        public async Task CreateNewTicketNotification(PushNotificationTicketCreateViewModel viewModel)
        {
            try
            {
                var options = new NotificationCreateOptions
                {
                    AppId = new Guid(OneSignalOptions.AppId),
                    Filters = CreateOneSignalFilters(viewModel.Recipients),
                };

                string contents = string.Format("{0} has submitted ticket #{1} to the inbox.", viewModel.Requester, viewModel.TicketNumber);

                options.Headings.Add(LanguageCodes.English, string.Format("New Inbox for {0}", viewModel.BuildingName));
                options.Contents.Add(LanguageCodes.English, contents);

                bool sendBefore = await this.PushNotificationRepository.Exists(contents, JsonConvert.SerializeObject(options.Data), string.Format("New Inbox for {0}", viewModel.BuildingName), PushNotificationReason.Ticket_Created);

                if (!sendBefore)
                {
                    var oneSignalClient = new OneSignalClient(this.OneSignalOptions.ApiKey);
                    var result = oneSignalClient.Notifications.Create(options);

                    var saveViewModel = new PushNotificationSaveViewModel
                    {
                        Result = result,
                        Options = options,
                        DataType = PushNotificationDataType.Ticket,
                        Reason = PushNotificationReason.Ticket_Created
                    };
                    await SaveNotification(saveViewModel);
                }                
            }
            catch (SqlException sEx)
            {
                Log.Warning(sEx, "SqlException: Error on creating New Ticket Notification");
                throw sEx;
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine(ex.Message);
#endif
                Log.Warning(ex, "Exception: Error on creating New Ticket Notification");
                throw new OneSignalException(ex);
            }
        }

        public async Task CreateNewInspectionNotification(PushNotificationInspectionCreateViewModel viewModel)
        {
            try
            {
                var options = new NotificationCreateOptions
                {
                    AppId = new Guid(OneSignalOptions.AppId),
                    Filters = CreateOneSignalFilters(viewModel.Recipients),
                };

                string status = viewModel.InspectionStatus.ToString().SplitCamelCase();

                string contents = string.Format("Inspection #{0} has a change of status to {1}", viewModel.InspectionNumber, status);

                options.Headings.Add(LanguageCodes.English, string.Format("Inspection #{0} for {1}", viewModel.InspectionNumber, viewModel.BuildingName));
                options.Contents.Add(LanguageCodes.English, contents);
                bool sendBefore = await this.PushNotificationRepository.Exists(contents, JsonConvert.SerializeObject(options.Data), string.Format("New Inbox for {0}", viewModel.BuildingName), viewModel.PushNotificationReason);

                if (!sendBefore)
                {
                    var oneSignalClient = new OneSignalClient(this.OneSignalOptions.ApiKey);
                    var result = oneSignalClient.Notifications.Create(options);

                    var saveViewModel = new PushNotificationSaveViewModel
                    {
                        Result = result,
                        Options = options,
                        DataType = PushNotificationDataType.Inspection,
                        Reason = viewModel.PushNotificationReason
                    };
                    await SaveNotification(saveViewModel);
                }
            }
            catch (SqlException sEx)
            {
                Log.Warning(sEx, "SqlException: Error on creating New Inspection Notification");
                throw sEx;
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine(ex.Message);
#endif
                Log.Warning(ex, "Exception: Error on creating New Inspection Notification");
                throw new OneSignalException(ex);
            }
        }

        public async Task<DataSource<PushNotificationGridViewModel>> ReadAllByUserAsync(DataSourceRequestPushNotifications dataQuery)
        {
            var result = await this.PushNotificationRepository.ReadAllByUserAsync(dataQuery, this.UserEmail, this.CompanyId);

            return result;
        }

        public async Task<bool> MarkAsRead(PushNOtificationMarkAsReadViewModel notification)
        {
            bool result = await this.PushNotificationRepository.MarkAsRead(notification, this.UserEmail, this.CompanyId);

            return result;
        }

        public async Task<bool> Exists(string Content, string Data, string Heading, PushNotificationReason Reason)
        {
            bool result = await this.PushNotificationRepository.Exists( Content,  Data,  Heading,  Reason);

            return result;
        }

        #region Utils

        private Tuple<string, string> NewWorkOrderPushNotification(WorkOrderNotificationTemplate template, WorkOrderEmailDetailsViewModel workOrder)
        {
            string heading = template.SubjectTemplate.Replace("[woNumber]", workOrder.WONumber.ToString());

            string content = template.PlainTextTemplate.Replace("[newLine]", Environment.NewLine)
                                                       .Replace("[truncatedBuildingName]", workOrder.BuildingName.LeftSubstring(45))
                                                       .Replace("[truncatedDescription]", workOrder.Description.LeftSubstring(45));

            return new Tuple<string, string>(heading, content);
        }

        private Tuple<string, string> ClosedWorkOrderPusnNotification(WorkOrderNotificationTemplate template, WorkOrderEmailDetailsViewModel workOrder)
        {
            string heading = template.SubjectTemplate.Replace("[woNumber]", workOrder.WONumber.ToString());

            string content = template.PlainTextTemplate.Replace("[newLine]", Environment.NewLine)
                                                       .Replace("[woNumber]", workOrder.WONumber.ToString())
                                                       .Replace("[buildingName]", workOrder.BuildingName)
                                                       .Replace("[employeeWhoClosed]", workOrder.EmployeeWhoClosedWO);

            return new Tuple<string, string>(heading, content);
        }

        private List<INotificationFilter> CreateOneSignalFilters(IEnumerable<string> recipients)
        {
            List<INotificationFilter> filters = new List<INotificationFilter>();

            foreach (string email in recipients)
            {
                filters.Add(
                    new NotificationFilterField()
                    {
                        Field = NotificationFilterFieldTypeEnum.Tag,
                        Key = "user_email",
                        Relation = "=",
                        Value = email.ToLowerInvariant()
                    });

                filters.Add(new NotificationFilterOperator() { Operator = "OR" });
            }

            filters.RemoveAt(filters.Count - 1);

            return filters;
        }

        private NotificationCreateOptions CreateDueTodayNotification(WorkOrdersByDueDateViewModel workOrder, WorkOrderNotificationTemplate template, DateTime sentAfter)
        {
            var options = new NotificationCreateOptions
            {
                AppId = new Guid(this.OneSignalOptions.AppId),
                Filters = this.CreateOneSignalFilters(new List<string> { workOrder.UserEmail }),
                Data = new Dictionary<string, string>() { ["scheduled"] = "dueToday" },
                SendAfter = sentAfter // SendAfter = DateTime.Now.AddSeconds(15)
            };

            options.Headings.Add(LanguageCodes.English, template.SubjectTemplate
                                                                .Replace("[numberOfDueToday]", workOrder.DueToday.ToString()));
            options.Contents.Add(LanguageCodes.English, template.PlainTextTemplate
                                                                .Replace("[newLine]", Environment.NewLine)
                                                                .Replace("[numberOfDueToday]", workOrder.DueToday.ToString())
                                                                .Replace("[currentDate]", sentAfter.ToString("MMM dd, yyyy")));

            if (workOrder.DueToday > 0)
            {
                options.ActionButtons = new List<ActionButtonField> { new ActionButtonField { Id = "id1", Text = "See List" } };
            }

            return options;
        }

        private NotificationCreateOptions CreatePastDueNotification(WorkOrdersByDueDateViewModel workOrder, WorkOrderNotificationTemplate template, DateTime sentAfter)
        {
            var options = new NotificationCreateOptions
            {
                AppId = new Guid(this.OneSignalOptions.AppId),
                Filters = this.CreateOneSignalFilters(new List<string> { workOrder.UserEmail }),
                Data = new Dictionary<string, string>() { ["scheduled"] = "pastDue" },
                SendAfter = sentAfter // SendAfter = DateTime.Now.AddSeconds(30)
            };

            options.Headings.Add(LanguageCodes.English, template.SubjectTemplate
                                                                .Replace("[numberOfPastDue]", workOrder.PastDue.ToString()));
            options.Contents.Add(LanguageCodes.English, template.PlainTextTemplate
                                                                .Replace("[newLine]", Environment.NewLine)
                                                                .Replace("[numberOfPastDue]", workOrder.PastDue.ToString())
                                                                .Replace("[currentDate]", sentAfter.ToString("MMM dd, yyyy")));

            if (workOrder.PastDue > 0)
            {
                options.ActionButtons = new List<ActionButtonField> { new ActionButtonField { Id = "id1", Text = "See List" } };
            }

            return options;
        }

        private async Task SaveNotification(PushNotificationSaveViewModel viewModel)
        {
            try
            {
                var pushNotification = new PushNotification
                {
                    Heading = viewModel.Options.Headings.FirstOrDefault(h => h.Key.Equals(LanguageCodes.English)).Value,
                    Content = viewModel.Options.Contents.FirstOrDefault(c => c.Key.Equals(LanguageCodes.English)).Value,
                    CompletedAt = DateTime.UtcNow.ToEpoch(),
                    Data = JsonConvert.SerializeObject(viewModel.Options.Data),
                    DataType = viewModel.DataType,
                    Reason = viewModel.Reason,
                    Converted = 0
                };

                if (string.IsNullOrEmpty(viewModel.Result?.Id) == false)
                {
                    pushNotification.OneSignalId = new Guid(viewModel.Result.Id);
                }

                int pId = await this.PushNotificationRepository.InsertPushNotification(pushNotification);

                if (pId > 0)
                {
                    var notificationFilters = new List<PushNotificationFilter>();
                    foreach (NotificationFilterField filter in viewModel.Options.Filters.Where(f => f.GetType().Equals(typeof(NotificationFilterField))))
                    {
                        notificationFilters.Add(new PushNotificationFilter
                        {
                            PushNotificationId = pId,
                            Field = Enum.GetName(typeof(NotificationFilterFieldTypeEnum), filter.Field).ToLowerInvariant(),
                            Key = filter.Key,
                            Relation = filter.Relation,
                            Value = filter.Value
                        });
                    }

                    await this.PushNotificationRepository.InsertPushNotificationFilters(notificationFilters);
                }
            }
            catch (SqlException sEx)
            {
#if DEBUG
                Console.WriteLine(sEx.Message);
#endif
                throw sEx;
            }
        }

        #endregion
    }
}
