
**For using Insert methods into BaseDapperRepository you have to:**

 * Add `[Table("DbTableName")]` attribute to model class.
 * Add `[IgnoreInsert]` attribute to mapped objects.
 * Add Dapper to using section: `using Dapper;` 
 * And obviously, inject IBaseDapperRepository where needed ;-)

*For more details, you can see and example at `PushNotificationService.CreateNotification()`*