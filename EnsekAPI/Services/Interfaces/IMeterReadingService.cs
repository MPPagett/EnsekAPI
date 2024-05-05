using EnsekAPI.Data.Models;
using EnsekAPI.Models;

namespace EnsekAPI.Services
{
    public interface IMeterReadingService
    {
        Task<List<MeterReadingValidationResult>> ValidateUpload(IFormFile file);

        Task CreateMany(List<MeterReading> readings);

    }
}
