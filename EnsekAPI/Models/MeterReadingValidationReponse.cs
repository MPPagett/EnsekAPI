namespace EnsekAPI.Models
{
    public class MeterReadingValidationResponse
    {
        public int ValidReadings { get; set; }
        public int InvalidReadings { get; set; }
        public List<MeterReadingValidationResult> MeterReadingValidationResults { get; set; }
    }
}
