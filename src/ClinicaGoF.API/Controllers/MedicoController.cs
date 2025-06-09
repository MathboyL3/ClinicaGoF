using ClinicaGoF.Application.DTOs.InputModels;
using ClinicaGoF.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ClinicaGoF.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MedicoController : ControllerBase
{
    private readonly IMedicoService _medicoService;

    public MedicoController(IMedicoService medicoService)
    {
        _medicoService = medicoService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var medicos = await _medicoService.ListarAsync();
        return Ok(medicos);
    }

    [HttpGet("{crm}")]
    public async Task<IActionResult> GetByCrm(string crm)
    {
        var medico = await _medicoService.ObterPorCrmAsync(crm);
        return medico is null ? NotFound() : Ok(medico);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] MedicoInputModel medico)
    {
        await _medicoService.CadastrarAsync(medico);
        return CreatedAtAction(nameof(GetByCrm), new { medico.CRM }, medico);
    }
}