using Microsoft.AspNetCore.Mvc;
using EnsekAPI.Services;
using EnsekAPI.Models;

namespace EnsekAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MeterReadingController : ControllerBase
    {
        private readonly IMeterReadingService _meterReadingService;
        private readonly IRequestValidatorService _requestValidatorService;


        public MeterReadingController(
            IMeterReadingService meterReadingService,
            IRequestValidatorService requestValidatorService)
        {
            _meterReadingService = meterReadingService;
            _requestValidatorService = requestValidatorService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadCsv()
        {

            var requestValidation = _requestValidatorService.ValidateCsvRequest(Request);

            if (!requestValidation.Item1)
            {
                return BadRequest(requestValidation.Item2);
            }

            var file = Request.Form.Files[0];
            var validationResult = await _meterReadingService.ValidateUpload(file);
            var validReadings = validationResult.Where(x => x.Valid)?.Select(x => x.MeterReading).ToList();
            var invalidReadings = validationResult.Where(x => !x.Valid)?.Select(x => x.MeterReading).ToList();

            if (validReadings != null && validReadings.Any())
            {
                await _meterReadingService.CreateMany(validReadings);
            }

            return Ok(new MeterReadingValidationResponse
            {
                MeterReadingValidationResults = validationResult,
                ValidReadings = validReadings != null ? validReadings.Count() : 0,
                InvalidReadings = invalidReadings != null ? invalidReadings.Count() : 0
            });
        }
    }
}
