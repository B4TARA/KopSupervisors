using MimeKit;

namespace KOP.EmailService
{
    public class Message
    {
        public List<MailboxAddress> To { get; set; }
        public string AddresseeName { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }

        public Message(IEnumerable<string> to, string subject, string content, string addresseeName)
        {
            To = to.Select(x => new MailboxAddress(x, x)).ToList();
            Subject = subject;
            Content = content;
            AddresseeName = addresseeName;
        }
    }
}