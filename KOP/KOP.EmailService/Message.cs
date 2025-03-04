using MimeKit;

namespace KOP.EmailService
{
    public class Message
    {
        public List<MailboxAddress> To { get; set; }
        public string AddresseeName { get; set; }
        public string JudgedName { get; set; }
        public DateOnly JudgedContractEndDate { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }

        public Message(IEnumerable<string> to, string subject, string content, string addresseeName
            /* string judgedName, DateOnly judgedContractEndDate, */)
        {
            To = to.Select(x => new MailboxAddress(x, x)).ToList();
            Subject = subject;
            Content = content;
            AddresseeName = addresseeName;
            //JudgedName = judgedName;
            //JudgedContractEndDate = judgedContractEndDate;
        }
    }
}