using System.Globalization;

namespace EnsekAPI.Services
{
    public class ValidatorService : IValidatorService
    {
        public Tuple<bool, int> ValidateInt(string input, int? min = null)
        {
            if (!int.TryParse(input, out int inputAsInt))
            {
                return new Tuple<bool, int>(false, inputAsInt);
            }

            if (min != null && inputAsInt < min)
            {
                return new Tuple<bool, int>(false, inputAsInt);
            }

            return new Tuple<bool, int>(true, inputAsInt);
        }
        
        public Tuple<bool, DateTime> ValidateDateTime(string input)
        {
            if (!DateTime.TryParseExact(input, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime))
            {
                return new Tuple<bool, DateTime>(false, dateTime);
            }
            else
            {
                return new Tuple<bool, DateTime>(true, dateTime);
            }
        }
    }
}
