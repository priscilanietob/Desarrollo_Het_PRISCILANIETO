using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Autocobro.Models
{
    public class Ticket
    {
        [Key]
        public int Id { get; set; }

        public DateTime Fecha { get; set; } = DateTime.Now;

        public List<ItemCarrito> Items { get; set; } = new();

        public string MetodoPago { get; set; } = "";

        [NotMapped]
        public decimal Total => Items?.Sum(i => i.Subtotal) ?? 0;

        public decimal Cambio { get; set; }
    }
}
