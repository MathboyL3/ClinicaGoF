using ClinicaGoF.Application.DTOs.InputModels;
using ClinicaGoF.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ClinicaGoF.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConsultaController : ControllerBase
{
    private readonly IConsultaService _consultaService;

    public ConsultaController(IConsultaService consultaService)
    {
        _consultaService = consultaService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var consultas = await _consultaService.ListarAsync();
        return Ok(consultas);
    }

    [HttpGet("paciente/{pacienteId}")]
    public async Task<IActionResult> GetByPaciente(Guid pacienteId)
    {
        var consultas = await _consultaService.ListarPorPacienteAsync(pacienteId);
        return Ok(consultas);
    }

    [HttpGet("paciente/documento/{documento}")]
    public async Task<IActionResult> GetByDocumento(string documento)
    {
        var consultas = await _consultaService.ListarPorDocumentoPacienteAsync(documento);
        return Ok(consultas);
    }

    [HttpGet("medico/crm/{crm}")]
    public async Task<IActionResult> GetByCrm(string crm)
    {
        var consultas = await _consultaService.ListarPorCrmMedicoAsync(crm);
        return Ok(consultas);
    }

    [HttpGet("intervalo")]
    public async Task<IActionResult> GetByIntervalo([FromQuery] DateTime inicio, [FromQuery] DateTime fim)
    {
        var consultas = await _consultaService.ListarPorIntervaloAsync(inicio, fim);
        return Ok(consultas);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] ConsultaInputModel input)
    {
        await _consultaService.AgendarAsync(input);
        return Ok();
    }
}
