namespace Autocobro.Models
{
    public class Reporte
    {
        public int VentasTotales { get; set; }
        public decimal IngresosTotales { get; set; }
        public decimal IngresosEfectivo { get; set; }
        public decimal IngresosTarjeta { get; set; }
        public List<string> TopProductos { get; set; } = new();
        public List<string> TopCategorias { get; set; } = new();
    }
}
