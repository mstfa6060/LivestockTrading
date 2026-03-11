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

                var gmailConfig = config.GetSection("Gmail");

                var smtpHost = gmailConfig["SmtpHost"]
                    ?? Environment.GetEnvironmentVariable("BREVO_SMTP_HOST")
                    ?? "smtp-relay.brevo.com";
                var smtpPort = int.Parse(
                    gmailConfig["SmtpPort"]
                    ?? Environment.GetEnvironmentVariable("BREVO_SMTP_PORT")
                    ?? "587");
                var smtpUser = gmailConfig["SmtpUser"]
                    ?? Environment.GetEnvironmentVariable("BREVO_SMTP_USER");
                var smtpPass = gmailConfig["SmtpPass"]
                    ?? Environment.GetEnvironmentVariable("BREVO_SMTP_PASS");
                var fromEmail = gmailConfig["FromEmail"]
                    ?? Environment.GetEnvironmentVariable("BREVO_FROM_EMAIL");
                var fromName = gmailConfig["FromName"]
                    ?? Environment.GetEnvironmentVariable("BREVO_FROM_NAME")
                    ?? "Livestock Trading";

                Console.WriteLine($"[Init] SMTP Host: {smtpHost}, Port: {smtpPort}, From: {fromEmail}");

                return new BrevoSmtpEmailService(smtpHost, smtpPort, smtpUser, smtpPass, fromEmail, fromName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Init] Email servisi oluşturulurken hata: {ex.Message}");
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
