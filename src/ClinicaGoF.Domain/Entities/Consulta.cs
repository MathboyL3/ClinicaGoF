using ClinicaGoF.Domain.Interfaces;

namespace ClinicaGoF.Domain.Entities;

public class Consulta : IPrototype<Consulta>
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PacienteId { get; set; }
    public Guid MedicoId { get; set; }
    public DateTime DataHora { get; set; }
    public string Observacoes { get; set; } = string.Empty;

    /// <summary>
    /// Creates a clone of the current consultation with a new ID and maintaining other data
    /// </summary>
    /// <returns>A new Consulta object with copied properties</returns>
    public Consulta Clone()
    {
        return new Consulta
        {
            // Generate a new ID for the cloned consultation
            Id = Guid.NewGuid(),
            // Maintain the same patient, doctor and observations
            PacienteId = this.PacienteId,
            MedicoId = this.MedicoId,
            Observacoes = this.Observacoes,
            // The DataHora will need to be set separately for the rescheduled appointment
            DataHora = this.DataHora
        };
    }

    /// <summary>
    /// Creates a clone of the current consultation with a specified new date/time
    /// </summary>
    /// <param name="novaDataHora">The new date and time for the consultation</param>
    /// <returns>A new Consulta object with copied properties and updated date/time</returns>
    public Consulta CloneWithNewDateTime(DateTime novaDataHora)
    {
        var clone = this.Clone();
        clone.DataHora = novaDataHora;
        return clone;
    }
}