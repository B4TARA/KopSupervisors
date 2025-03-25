namespace KOP.EmailService
{
    public interface IEmailSender
    {
        Task SendEmailAsync(Message message);
    }
}