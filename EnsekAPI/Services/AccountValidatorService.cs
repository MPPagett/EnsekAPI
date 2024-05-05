using EnsekAPI.Data;

namespace EnsekAPI.Services
{
    public class AccountValidatorService : IAccountValidatorService
    {
        private readonly EnsekDbContext _dbContext;
        public AccountValidatorService(EnsekDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Tuple<bool, int> ValidateAccount(string accountId)
        {
            var accountIds = _dbContext.Accounts.Select(x => x.Id).ToList();

            if (!int.TryParse(accountId, out int accountIdAsInt))
            {
                return new Tuple<bool, int>(false, accountIdAsInt);
            }
            else if (accountIdAsInt <= 0 || !accountIds.Contains(accountIdAsInt))
            {
                return new Tuple<bool, int>(false, accountIdAsInt);
            }

            return new Tuple<bool, int>(true, accountIdAsInt);
        }
    }
}
