using KOP.DAL.Entities;
using KOP.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KOP.DAL.Repositories
{
    public class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<bool> IsServiceNumberUniqueAsync(int serviceNumber)
        {
            return !await _dbContext.Employees.AnyAsync(e => e.ServiceNumber == serviceNumber);
        }
    }
}