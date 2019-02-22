## 启用电子邮件

重写数据库迁移类：
```csharp
/// <summary>
/// 邮件数据迁移类。
/// </summary>
public class EmailDataMigration : Mozlite.Extensions.Messages.EmailDataMigration
{

}
```

重写后台发送服务类（如果使用附件，重写`Mozlite.Extensions.Storages.Mail.EmailSendTaskService`）：
```csharp
/// <summary>
/// 邮件发送服务。
/// </summary>
public class EmailTaskService : EmailSendTaskService
{
    /// <summary>
    /// 初始化类<see cref="EmailTaskService"/>。
    /// </summary>
    /// <param name="settingsManager">配置管理接口。</param>
    /// <param name="messageManager">消息管理接口。</param>
    /// <param name="logger">日志接口。</param>
    /// <param name="mediaDirectory">媒体文件操作接口。</param>
    public EmailTaskService(ISettingsManager settingsManager, IMessageManager messageManager, ILogger<EmailSendTaskService> logger, IMediaDirectory mediaDirectory)
        : base(settingsManager, messageManager, logger, mediaDirectory)
    {
    }
}
```

## 启用短信


重写数据库迁移类：
```csharp
/// <summary>
/// 短信数据迁移类。
/// </summary>
public class SmsDataMigration : Mozlite.Extensions.Messages.SMS.SmsDataMigration
{

}
```

重写后台发送服务类（如果使用附件，重写`Mozlite.Extensions.Storages.Mail.EmailSendTaskService`）：
```csharp
/// <summary>
/// 短信发送服务。
/// </summary>
public class SmsTaskService : Mozlite.Extensions.Messages.SMS.SmsTaskService
{
    /// <summary>
    /// 初始化类<see cref="SmsTaskService"/>。
    /// </summary>
    /// <param name="smsManager">短信管理接口。</param>
    public SmsTaskService(ISmsManager smsManager) : base(smsManager)
    {
    }
}
```