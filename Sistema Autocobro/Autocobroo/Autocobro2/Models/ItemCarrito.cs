using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Autocobro.Models
{
    public class ItemCarrito
    {
        [Key]
        public int Id { get; set; }

        public int ProductoId { get; set; }

        [ForeignKey(nameof(ProductoId))]
        public Producto Producto { get; set; } = new Producto();

        public int Cantidad { get; set; }

        [NotMapped]
        public decimal Subtotal => Producto?.Precio * Cantidad ?? 0;

        public int TicketId { get; set; }

        [ForeignKey(nameof(TicketId))]
        public Ticket? Ticket { get; set; }
    }
}
