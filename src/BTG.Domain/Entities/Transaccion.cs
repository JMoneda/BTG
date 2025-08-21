using System;

namespace BTG.Domain.Entities
{
    public class Transaccion
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ClienteId { get; set; }
        public string FondoId { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty; // "SUSCRIPCION" | "CANCELACION"
        public decimal Monto { get; set; }
        public DateTime Fecha { get; set; } = DateTime.UtcNow;
        public string Estado { get; set; } = "COMPLETADA";
    }
}
