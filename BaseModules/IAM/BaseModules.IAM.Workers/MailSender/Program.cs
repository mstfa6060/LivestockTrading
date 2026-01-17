using Arfware.ArfBlocks.Core;
using Arfware.ArfBlocks.Core.Extentions;
using BaseModules.IAM.Workers.MailSender.EventHandlers;
using BaseModules.IAM.Workers.MailSender.Workers;


Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((hostContext, services) =>
    {
        // 🔁 Sadece bir Email servisi tanımla
        //         services.AddSingleton<IEmailService>(sp =>
        //    {
        //        var config = sp.GetRequiredService<IConfiguration>();
        //        var apiKey = config["SendGrid:ApiKey"];
        //        var fromEmail = config["SendGrid:FromEmail"];
        //        var fromName = config["SendGrid:FromName"];
        //        return new SendGridEmailService(apiKey, fromEmail, fromName);
        //    });
        services.AddSingleton<IEmailService>(sp =>
        {
            try
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var brevoConfig = config.GetSection("Brevo");

                var smtpHost = brevoConfig["SmtpHost"];
                var smtpPort = int.Parse(brevoConfig["SmtpPort"]); // BURASI PATLIYOR OLABİLİR
                var smtpUser = brevoConfig["SmtpUser"];
                var smtpPass = brevoConfig["SmtpPass"];
                var fromEmail = brevoConfig["FromEmail"];
                var fromName = brevoConfig["FromName"];

                Console.WriteLine($"📧 [Init] SMTP Host: {smtpHost}, Port: {smtpPort}, From: {fromEmail}");

                return new BrevoSmtpEmailService(smtpHost, smtpPort, smtpUser, smtpPass, fromEmail, fromName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [Init] Email servisi oluşturulurken hata: {ex.Message}");
                throw;
            }
        });


        // Event handler
        services.AddScoped<ForgotPasswordEmailHandler>();

        // Hosted worker
        services.AddHostedService<EmailWorker>();

        // ArfBlocks config
        services.AddArfBlocks(options =>
        {
            options.ApplicationProjectNamespace = "BaseModules.IAM.Application";
            options.ConfigurationSection = hostContext.Configuration.GetSection("ProjectConfigurations");
        });
    })
    .Build()
    .Run();
