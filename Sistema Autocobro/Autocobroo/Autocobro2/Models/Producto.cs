using CsvHelper.Configuration.Attributes;
using Autocobro.Converters; 

namespace Autocobro.Models
{
    public class Producto
    {
        [Ignore]
        public int Id { get; set; }
        
        public string CodigoDeBarras { get; set; } = "";
        
        public string Nombre { get; set; } = "";


        public string Categoria { get; set; } = "";
        
        [TypeConverter(typeof(PrecioConverter))]
        public decimal Precio { get; set; } 

        public string Marca { get; set; } = ""; 
    }
}