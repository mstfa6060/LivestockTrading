
using BaseModules.IAM.Workers.SmsSender.EventHandlers;
using BaseModules.IAM.Workers.SmsSender.Services;
using BaseModules.IAM.Workers.SmsSender.Workers;

Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddSingleton<ISmsService, FakeSmsService>(); // Buraya NetgsmSmsService eklenebilir
        services.AddScoped<OtpSmsHandler>();
        services.AddHostedService<SmsWorker>();
    })
    .Build()
    .Run();
