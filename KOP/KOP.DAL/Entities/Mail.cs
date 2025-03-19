using KOP.Common.Enums;

namespace KOP.DAL.Entities
{
    public class Mail
    {
        public int Id { get; set; }
        public MailCodes Code { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
    }
}