using ClinicaGoF.Application.Builders;
using ClinicaGoF.Application.DTOs.InputModels;
using ClinicaGoF.Application.DTOs.ViewModels;
using ClinicaGoF.Application.Services.Interfaces;
using ClinicaGoF.Domain.Entities;
using ClinicaGoF.Domain.Repository.Interfaces;

namespace ClinicaGoF.Application.Services;
public class ConsultaService : IConsultaService
{
    private readonly IRepository<Consulta> _consultaRepo;
    private readonly IRepository<Paciente> _pacienteRepo;
    private readonly IRepository<Medico> _medicoRepo;

    public ConsultaService(IRepository<Consulta> consultaRepo, IRepository<Paciente> pacienteRepo, IRepository<Medico> medicoRepo)
    {
        _consultaRepo = consultaRepo;
        _pacienteRepo = pacienteRepo;
        _medicoRepo = medicoRepo;
    }

    public async Task<IEnumerable<ConsultaViewModel>> ListarAsync()
    {
        var consultas = await _consultaRepo.GetAllAsync();
        var pacientes = await _pacienteRepo.GetAllAsync();
        var medicos = await _medicoRepo.GetAllAsync();

        return consultas.Select(c =>
        {
            var paciente = pacientes.FirstOrDefault(p => p.Id == c.PacienteId);
            var medico = medicos.FirstOrDefault(m => m.Id == c.MedicoId);

            return new ConsultaViewModel(
                c.Id,
                c.PacienteId,
                paciente?.Nome ?? "",
                medico?.Nome ?? "",
                c.DataHora,
                c.Observacoes
            );
        });
    }

    public async Task<IEnumerable<ConsultaViewModel>> ListarPorPacienteAsync(Guid pacienteId)
    {
        var todas = await ListarAsync();
        return todas.Where(c => c.PacienteId == pacienteId);
    }

    public async Task<IEnumerable<ConsultaViewModel>> ListarPorDocumentoPacienteAsync(string documento)
    {
        var pacientes = await _pacienteRepo.GetAllAsync();
        var paciente = pacientes.FirstOrDefault(p => p.Documento == documento);
        return paciente == null ? Enumerable.Empty<ConsultaViewModel>() : await ListarPorPacienteAsync(paciente.Id);
    }

    public async Task<IEnumerable<ConsultaViewModel>> ListarPorCrmMedicoAsync(string crm)
    {
        var medicos = await _medicoRepo.GetAllAsync();
        var medico = medicos.FirstOrDefault(m => m.CRM == crm);
        var todas = await ListarAsync();
        return medico == null ? Enumerable.Empty<ConsultaViewModel>() : todas.Where(c => c.NomeMedico == medico.Nome);
    }

    public async Task<IEnumerable<ConsultaViewModel>> ListarPorIntervaloAsync(DateTime inicio, DateTime fim)
    {
        var todas = await ListarAsync();
        return todas.Where(c => c.DataHora >= inicio && c.DataHora <= fim);
    }

    public async Task AgendarAsync(ConsultaInputModel input)
    {

        var consultaBuilder = new ConsultaBuilder()
            .ComPaciente(input.PacienteId)
            .ComMedico(input.MedicoId)
            .NaData(input.DataHora)
            .ComObservacoes(input.Observacoes)
            .AdicionarValidacao(async () =>
            {
                // Validar se o paciente existe
                var paciente = await _pacienteRepo.GetByIdAsync(input.PacienteId);
                return paciente != null
                    ? ValidationResult.Success()
                    : ValidationResult.Failure("Paciente não encontrado");
            })
            .AdicionarValidacao(async () =>
            {
                // Validar se o médico existe
                var medico = await _medicoRepo.GetByIdAsync(input.MedicoId);
                return medico != null
                    ? ValidationResult.Success()
                    : ValidationResult.Failure("Médico não encontrado");
            })
            .AdicionarValidacao(async () =>
            {
                // Validar se data é futura
                return input.DataHora > DateTime.Now
                    ? ValidationResult.Success()
                    : ValidationResult.Failure("A data da consulta deve ser futura");
            })
            .AdicionarValidacao(async () =>
            {
                // Verificar se o médico já tem consulta no horário
                var consultas = await _consultaRepo.GetAllAsync();
                var conflito = consultas.Any(c =>
                    c.MedicoId == input.MedicoId &&
                    c.DataHora == input.DataHora);

                return !conflito
                    ? ValidationResult.Success()
                    : ValidationResult.Failure("Médico já possui consulta agendada neste horário");
            });

        var consulta = await consultaBuilder.ConstruirAsync();

        await _consultaRepo.AddAsync(consulta);
    }

    public async Task<Guid> ReagendarConsultaAsync(Guid consultaId, DateTime novaDataHora)
    {
        // Obtém a consulta original
        var consultaOriginal = await _consultaRepo.GetByIdAsync(consultaId);
        if (consultaOriginal == null)
        {
            throw new KeyNotFoundException("Consulta não encontrada");
        }

        // Valida se a nova data é futura
        if (novaDataHora <= DateTime.Now)
        {
            throw new ArgumentException("A nova data da consulta deve ser futura");
        }

        // Verifica se o médico já tem consulta no novo horário
        var consultas = await _consultaRepo.GetAllAsync();
        var conflito = consultas.Any(c =>
            c.MedicoId == consultaOriginal.MedicoId &&
            c.DataHora == novaDataHora);

        if (conflito)
        {
            throw new InvalidOperationException("Médico já possui consulta agendada neste horário");
        }

        // Aplica o padrão Prototype para criar uma cópia da consulta com a nova data
        var novaConsulta = consultaOriginal.CloneWithNewDateTime(novaDataHora);

        // Salva a nova consulta no repositório
        await _consultaRepo.AddAsync(novaConsulta);

        return novaConsulta.Id;
    }
}