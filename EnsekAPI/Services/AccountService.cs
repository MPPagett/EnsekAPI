using EnsekAPI.Data;
using EnsekAPI.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace EnsekAPI.Services
{
    public class AccountService : IAccountService
    {
        public readonly EnsekDbContext _dbContext;

        public AccountService(EnsekDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Account>> Search(string name)
        {
            var nameParsed = name.Trim().ToLower();

            return await _dbContext.Accounts.Where(x => 
                string.IsNullOrEmpty(name) 
                || x.FirstName.Trim().ToLower().Contains(nameParsed) 
                || x.FirstName.Trim().ToLower().Contains(nameParsed))
                .Include(x => x.MeterReadings).ToListAsync();
        }
    }
}
