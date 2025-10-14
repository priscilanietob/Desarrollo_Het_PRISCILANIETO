using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace Autocobro.Converters 
{
    public class PrecioConverter : DefaultTypeConverter
    {
        public override object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return 0m;
            }

            // 1. Limpia la cadena de texto
            string cleanedText = text.Replace("$", "").Replace("MXN", "").Trim();

            // 2. Intenta la conversión
            if (decimal.TryParse(cleanedText, NumberStyles.Currency, CultureInfo.InvariantCulture, out decimal result))
            {
                return result;
            }

            throw new CsvHelperException(row.Context, $"No se pudo convertir '{text}' a decimal después de limpiar. Texto limpio: '{cleanedText}'");
        }
    }
}