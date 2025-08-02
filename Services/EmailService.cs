using MailKit.Net.Smtp;
using MimeKit;

namespace pharmacy_management.Services
{
    public class EmailService
{
    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(Environment.GetEnvironmentVariable("EMAIL_USER")));
        email.To.Add(MailboxAddress.Parse(toEmail));
        email.Subject = subject;
        email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
        {
            Text = body
        };

        // Double-check your environment variables
        var host = Environment.GetEnvironmentVariable("EMAIL_HOST"); 
        Console.WriteLine(host);
        var port = Environment.GetEnvironmentVariable("EMAIL_PORT"); 
        Console.WriteLine(port);
        var username = Environment.GetEnvironmentVariable("EMAIL_USER");
        Console.WriteLine(username);

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(Environment.GetEnvironmentVariable("EMAIL_HOST"),
                                int.Parse(Environment.GetEnvironmentVariable("EMAIL_PORT")!),
                                MailKit.Security.SecureSocketOptions.StartTls);

        await smtp.AuthenticateAsync(
            Environment.GetEnvironmentVariable("EMAIL_USER"),
            Environment.GetEnvironmentVariable("EMAIL_PASS")
        );

        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
}
}
