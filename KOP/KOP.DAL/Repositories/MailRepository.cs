using KOP.DAL.Entities;
using KOP.DAL.Interfaces;

namespace KOP.DAL.Repositories
{
    public class MailRepository : RepositoryBase<Mail>, IMailRepository
    {
        public MailRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}