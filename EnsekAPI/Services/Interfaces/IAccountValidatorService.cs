namespace EnsekAPI.Services
{
    public interface IAccountValidatorService
    {
        Tuple<bool, int> ValidateAccount(string accountId);
    }
}
