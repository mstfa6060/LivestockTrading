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

                // Gmail kullan (Brevo yerine)
                var gmailConfig = config.GetSection("Gmail");

                var smtpHost = gmailConfig["SmtpHost"];
                var smtpPort = int.Parse(gmailConfig["SmtpPort"]);
                var smtpUser = gmailConfig["SmtpUser"];
                var smtpPass = gmailConfig["SmtpPass"];
                var fromEmail = gmailConfig["FromEmail"];
                var fromName = gmailConfig["FromName"];

                Console.WriteLine($"📧 [Init] Gmail SMTP Host: {smtpHost}, Port: {smtpPort}, From: {fromEmail}");

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
