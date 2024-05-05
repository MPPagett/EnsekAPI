using EnsekAPI.Data.Models;

namespace EnsekAPI.Services
{
    public interface IAccountService
    {
        Task<List<Account>> Search(string name);
    }
}
