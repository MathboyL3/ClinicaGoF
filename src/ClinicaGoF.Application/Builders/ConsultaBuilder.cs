using ClinicaGoF.Application.Exceptions;
using ClinicaGoF.Domain.Entities;

namespace ClinicaGoF.Application.Builders;

public class ConsultaBuilder
{
    private Guid _id = Guid.NewGuid();
    private Guid _pacienteId;
    private Guid _medicoId;
    private DateTime _dataHora;
    private string _observacoes = string.Empty;
    private readonly List<Func<Task<ValidationResult>>> _validationRules = new();

    public ConsultaBuilder ComPaciente(Guid pacienteId)
    {
        _pacienteId = pacienteId;
        return this;
    }

    public ConsultaBuilder ComMedico(Guid medicoId)
    {
        _medicoId = medicoId;
        return this;
    }

    public ConsultaBuilder NaData(DateTime dataHora)
    {
        _dataHora = dataHora;
        return this;
    }

    public ConsultaBuilder ComObservacoes(string observacoes)
    {
        _observacoes = observacoes ?? string.Empty;
        return this;
    }

    public ConsultaBuilder AdicionarValidacao(Func<Task<ValidationResult>> validationRule)
    {
        _validationRules.Add(validationRule);
        return this;
    }

    public async Task<Consulta> ConstruirAsync()
    {
        // Executar todas as validações
        foreach (var rule in _validationRules)
        {
            var result = await rule();
            if (!result.IsValid)
            {
                throw new ConsultaInvalidaException(result.ErrorMessage);
            }
        }

        return new Consulta
        {
            Id = _id,
            PacienteId = _pacienteId,
            MedicoId = _medicoId,
            DataHora = _dataHora,
            Observacoes = _observacoes
        };
    }
}



