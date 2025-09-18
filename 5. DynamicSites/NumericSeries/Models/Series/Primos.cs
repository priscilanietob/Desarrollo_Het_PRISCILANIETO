namespace NumericSeries.Models.Series
{
    public class Primos
    {
        public static int GetValue(int n)
        {
            int count = -1;
            int number = 1;

            while (count < n)
            {
                number++;
                if (EsPrimo(number))
                {
                    count++;
                }
            }

            return number;
        }

        private static bool EsPrimo(int num)
        {
            if (num < 2) return false;
            for (int i = 2; i * i <= num; i++)
            {
                if (num % i == 0) return false;
            }
            return true;
        }
    }
}
