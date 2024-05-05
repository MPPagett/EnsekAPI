using Azure.Core;
using EnsekAPI.Models;

namespace EnsekAPI.Services
{
    public class RequestValidatorService : IRequestValidatorService
    {
        public Tuple<bool, string> ValidateCsvRequest(HttpRequest request)
        {
            var file = request.Form.Files[0];

            if (file == null || file.Length == 0)
            {
                return new Tuple<bool, string>(false, "File is empty.");
            }

            if (!file.ContentType.Equals("text/csv", StringComparison.OrdinalIgnoreCase))
            {
                return new Tuple<bool, string>(false, "Unsupported file type. Only CSV files are allowed.");
            }

            return new Tuple<bool, string>(true, string.Empty);
        }
    }
}
