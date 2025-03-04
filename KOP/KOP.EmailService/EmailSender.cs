using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Utils;
using NLog;

namespace KOP.EmailService
{
    public class EmailSender : IEmailSender
    {
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly EmailConfiguration _emailConfig = new EmailConfiguration
        {
            From = "KOPSender",
            SmtpServer = "LDGate.mtb.minsk.by",
            Port = 25,
        };

        public async Task SendEmailAsync(Message message, string emailIconPath)
        {
            var mailMessage = CreateEmailMessage(message, emailIconPath);

            await SendAsync(mailMessage, message);
        }

        private MimeMessage CreateEmailMessage(Message message, string emailIconPath)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_emailConfig.From, _emailConfig.From));
            emailMessage.To.AddRange((IEnumerable<InternetAddress>)message.To);

            // TEST CASE //
            //var stringList = new string[] { "ebaturel@mtb.minsk.by", "nsakirina@mtb.minsk.by" };
            //List<MailboxAddress> mailboxAddressesList = stringList.Select(x => new MailboxAddress(x, x)).ToList();
            //emailMessage.To.AddRange((IEnumerable<InternetAddress>)mailboxAddressesList);
            //    //    // 

            emailMessage.Subject = message.Subject;

            var bodyBuilder = new BodyBuilder();
            var image = bodyBuilder.LinkedResources.Add(emailIconPath ?? "");
            image.IsAttachment = false;
            image.ContentId = MimeUtils.GenerateMessageId();

            // Форматирование HTML-тела письма
            bodyBuilder.HtmlBody = $@"
            <center>
                <div style='border: 15px solid #EDF4FF; width:700px; border-radius: 16px;'>
                    <div style='padding:15px; font-family:Tahoma; border-radius: 16px;'>	
                        <span style='width:50px; height:50px;'>
                            <img style='width:50px; height:50px;' src='cid:{image.ContentId}'>
                        </span>
                        <center>			
                            <div style='text-align: left; padding: 0px 10px 10px 10px; color: #1B74FD;'>
                                <h3><b>Уважаемый(ая)</b></h3>
                                <h2><b>{message.AddresseeName}</b></h2>                              
                                <div style='color:#333;'>
                                    <br>
                                    <div class='MAIN_BLOCKMESSAGE'>
                                        {message.Content}
                                    </div>						            	
                                    <br>
                                </div>
                            </div>
                        </center>
                    </div>
                </div>
            </center>";

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

                    _logger.Error($"Уведомление успешно отправлено cотруднику {message.AddresseeName}");
                }
                catch (Exception ex)
                {
                    _logger.Error($"Не удалось отправить уведомление cотруднику {message.AddresseeName} : {ex.Message}");
                }
                finally
                {
                    await client.DisconnectAsync(true);
                    client.Dispose();
                }
            }
        }
    }
}