﻿namespace ClinicaGoF.Domain.Entities;

public class Paciente : BaseEntity
{
    public string Nome { get; set; } = string.Empty;
    public string Documento { get; set; } = string.Empty;
    public DateTime DataNascimento { get; set; }
}