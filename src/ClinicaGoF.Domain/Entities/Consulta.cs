namespace ClinicaGoF.Domain.Entities;

public class Consulta
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PacienteId { get; set; }
    public Guid MedicoId { get; set; }
    public DateTime DataHora { get; set; }
    public string Observacoes { get; set; } = string.Empty;
}