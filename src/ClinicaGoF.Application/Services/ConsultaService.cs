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
        var consulta = new Consulta
        {
            Id = Guid.NewGuid(),
            PacienteId = input.PacienteId,
            MedicoId = input.MedicoId,
            DataHora = input.DataHora,
            Observacoes = input.Observacoes
        };

        await _consultaRepo.AddAsync(consulta);
    }
}