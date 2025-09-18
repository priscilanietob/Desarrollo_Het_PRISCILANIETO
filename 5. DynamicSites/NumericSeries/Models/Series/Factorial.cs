namespace NumericSeries.Models.Series
{
    public class Factorial
    {
        public static long GetValue(int n)
        {
            long result = 1;
            for (int i = 1; i <= n; i++) result *= i;
            return result;
        }
    }
}
