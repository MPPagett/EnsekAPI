namespace EnsekAPI.Services
{
    public interface IValidatorService
    {
        Tuple<bool, int> ValidateInt(string input, int? min = null);
        Tuple<bool, DateTime> ValidateDateTime(string input);

    }
}
