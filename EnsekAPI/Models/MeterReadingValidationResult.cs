using EnsekAPI.Data.Models;

namespace EnsekAPI.Models
{
    public class MeterReadingValidationResult
    {
        public MeterReadingValidationResult(bool valid) 
        {
            Valid = valid;
        }
        public bool Valid { get; set; }
        public string Input { get; set; }
        public string Error { get; set; }
        public MeterReading MeterReading { get; set; }
    }
}
