namespace BTG.Application.DTOs
{
    public class CancelarRequest
    {
        public Guid ClienteId { get; set; }
        public string FondoId { get; set; } = string.Empty;
    }
}
