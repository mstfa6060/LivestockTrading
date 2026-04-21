using Livestock.Workers.Consumers;
using Livestock.Workers.Services.Email;
using Livestock.Workers.Services.PriceConversion;
using Livestock.Workers.Services.Push;
using Livestock.Workers.Services.Sms;
using Shared.Infrastructure.Extensions;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSharedNats(builder.Configuration);

builder.Services.AddSingleton<IEmailService, LoggingEmailService>();
builder.Services.AddSingleton<ISmsService, LoggingSmsService>();
builder.Services.AddSingleton<IPushNotificationService, LoggingPushNotificationService>();
builder.Services.AddSingleton<ICurrencyConverter, LoggingCurrencyConverter>();

builder.Services.AddHostedService<NotificationConsumer>();
builder.Services.AddHostedService<MailConsumer>();
builder.Services.AddHostedService<SmsConsumer>();
builder.Services.AddHostedService<PriceConversionConsumer>();

var host = builder.Build();
host.Run();
