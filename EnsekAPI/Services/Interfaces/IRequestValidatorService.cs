namespace EnsekAPI.Services
{
    public interface IRequestValidatorService
    {
        Tuple<bool, string> ValidateCsvRequest(HttpRequest request);
    }
}
