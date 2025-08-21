namespace BTG.Domain.Entities;

public class Fondo
{
    public string Id { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public decimal MontoMinimo { get; set; }
    public string Categoria { get; set; } = string.Empty;
}
