namespace EntwineBackend
{
    public class Utils
    {
        public static int YearsSince(DateOnly date)
        {
            DateOnly currentDate = DateOnly.FromDateTime(DateTime.Now);
            int yearsDifference = currentDate.Year - date.Year;

            return currentDate < date.AddYears(yearsDifference) ? yearsDifference - 1 : yearsDifference;
        }
    }
}
