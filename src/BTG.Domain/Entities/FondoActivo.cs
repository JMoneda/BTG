using System;

namespace BTG.Domain.Entities
{
    public class FondoActivo
    {
        public string FondoId { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public DateTime FechaVinculacion { get; set; } = DateTime.UtcNow;
    }
}

