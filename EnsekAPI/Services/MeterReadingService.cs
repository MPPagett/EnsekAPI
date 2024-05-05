using EnsekAPI.Data;
using EnsekAPI.Data.Models;
using EnsekAPI.Models;

namespace EnsekAPI.Services
{
    public class MeterReadingService : IMeterReadingService
    {
        private readonly EnsekDbContext _dbContext;
        private IValidatorService _validator;
        private IAccountValidatorService _accountValidator;

        public MeterReadingService(EnsekDbContext dbContext, IValidatorService validator, IAccountValidatorService accountValidator)
        {
            _dbContext = dbContext;
            _validator = validator;
            _accountValidator = accountValidator;
        }

        public async Task<List<MeterReadingValidationResult>> ValidateUpload(IFormFile file)
        {
            var validationResults = new List<MeterReadingValidationResult>();
            var validReadings = new List<MeterReading>();

            var existingReadings = _dbContext.MeterReadings.ToList();

            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                reader.ReadLine();
                while (!reader.EndOfStream)
                {
                    var validationResult = new MeterReadingValidationResult(true);
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    if (values.Length != Constants.General.NumOfMeterReadingUploadFields)
                    {
                        SetValidationResult(validationResult, false, line, "Invalid format. Too many values entered. ");
                    }

                    var accountValidation = _accountValidator.ValidateAccount(values[0]);
                    var meterReadingDateValidation = _validator.ValidateDateTime(values[1]);
                    var meterReadingValidation = _validator.ValidateInt(values[2], Constants.General.MinMeterReading);

                    if (!accountValidation.Item1)
                    {
                        SetValidationResult(validationResult, false, line, "Invalid Account ID. Must be a whole number and account must exist.");
                    }

                    if (!meterReadingDateValidation.Item1)
                    {
                        SetValidationResult(validationResult, false, line, "Invalid Meter Reading Date Time. Must be in the format DD/MM/YYYY HH:MM. ");
                    }

                    if (!meterReadingValidation.Item1)
                    {
                        SetValidationResult(validationResult, false, line, $"Invalid Meter Read Value. Must be a whole number and greater than {Constants.General.MinMeterReading}. ");
                    }

                    if (validationResult.Valid)
                    {
                        var moreRecentReadings = existingReadings.Where(x => x.AccountId == accountValidation.Item2 && x.DateTime > meterReadingDateValidation.Item2);
                        var existingReading = existingReadings.FirstOrDefault(x => x.AccountId == accountValidation.Item2 && x.Value == meterReadingValidation.Item2);

                        if (moreRecentReadings.Any())
                        {
                            SetValidationResult(validationResult, false, line, "More recent reading already exists. ");
                        } 
                        else if (existingReading != null)
                        {
                            SetValidationResult(validationResult, false, line, "Reading already exists. ");
                        }
                        else
                        {
                            validationResult.MeterReading = new MeterReading
                            {
                                AccountId = accountValidation.Item2,
                                DateTime = meterReadingDateValidation.Item2,
                                Value = meterReadingValidation.Item2
                            };
                        }
                    }

                    validationResults.Add(validationResult);
                }
            }

            return validationResults;
        }

        private void SetValidationResult(MeterReadingValidationResult validationResult, bool valid, string line, string error)
        {
            validationResult.Valid = valid;
            validationResult.Input = line;
            validationResult.Error += error;
        }

        public async Task CreateMany(List<MeterReading> readings)
        {
            await _dbContext.AddRangeAsync(readings);
            await _dbContext.SaveChangesAsync();
        }
    }
}
