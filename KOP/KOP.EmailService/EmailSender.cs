using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Utils;
using Serilog;

namespace KOP.EmailService
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailConfiguration _emailConfig;
        private readonly ILogger _logger;
        private readonly string _emailTemplate;
        private const string _projectUrl = "https://kop.mtb.minsk.by/supervisors";
        private const string _emailIconPath = "C:/PROJECTS/KopSupervisors/Files/templates/logo.png";

        public EmailSender(ILogger logger)
        {
            _logger = logger;
            _emailConfig = new EmailConfiguration
            {
                From = "KOPSender@mtbank.by",
                SmtpServer = "LDGate.mtb.minsk.by",
                Port = 25,
            };
            _emailTemplate = LoadEmailTemplate();
        }

        private string LoadEmailTemplate()
        {
            var templatePath = Path.Combine("C:", "PROJECTS", "KopSupervisors", "Files", "templates", "EmailTemplate.html");
            if (!File.Exists(templatePath))
            {
                _logger.Error($"Email template not found at path: {templatePath}");
                throw new FileNotFoundException("Email template not found.", templatePath);
            }

            return File.ReadAllText(templatePath);
        }

        public async Task SendEmailAsync(Message message)
        {
            //// Добавляем Сакирину
            //message.To.AddRange(new List<string> { "nsakirina@mtb.minsk.by" }.Select(x => new MailboxAddress(x, x)));

            // Только самому себе - ebaturel@mtb.minsk.by
            //message.To = new List<string> { "ebaturel@mtb.minsk.by" }.Select(x => new MailboxAddress(x, x)).ToList();

            var mimeMessage = CreateEmailMessage(message);

            await SendAsync(mimeMessage, message);
        }

        private MimeMessage CreateEmailMessage(Message message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_emailConfig.From.Split('@').FirstOrDefault() ?? _emailConfig.From, _emailConfig.From));
            emailMessage.To.AddRange((IEnumerable<InternetAddress>)message.To);
            emailMessage.Bcc.AddRange(message.To);
            emailMessage.Subject = message.Subject;

            var bodyBuilder = new BodyBuilder();

            // Добавление иконки
            var image = bodyBuilder.LinkedResources.Add(_emailIconPath);
            image.IsAttachment = false;
            image.ContentId = MimeUtils.GenerateMessageId();

            // Замена плейсхолдеров
            var template = _emailTemplate
                .Replace("{AddresseeName}", message.AddresseeName)
                .Replace("{Content}", message.Content)
                .Replace("{ProjectUrl}", _projectUrl)
                .Replace("{CidImageContentId}", $"cid:{image.ContentId}");

            // Установка HTML-содержимого в тело письма
            bodyBuilder.HtmlBody = template;

            // Присоединение тела к сообщению
            emailMessage.Body = bodyBuilder.ToMessageBody();

            return emailMessage;
        }

        private async Task SendAsync(MimeMessage mailMessage, Message message)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.Port);
                    await client.SendAsync(mailMessage);

                    _logger.Warning($"Уведомление успешно отправлено cотруднику {message.AddresseeName}");
                }
                catch (Exception ex)
                {
                    _logger.Warning($"Не удалось отправить уведомление cотруднику {message.AddresseeName} : {ex.Message}");
                }
            }
        }
    }
}