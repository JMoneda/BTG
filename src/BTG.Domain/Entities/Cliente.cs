using System;
using System.Collections.Generic;

namespace BTG.Domain.Entities
{
    public class Cliente
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public decimal Saldo { get; set; } 
        public PreferenciaNotificacion PreferenciaNotificacion { get; set; } = PreferenciaNotificacion.Email;
        public List<FondoActivo> FondosActivos { get; set; } = new();
    }
}
