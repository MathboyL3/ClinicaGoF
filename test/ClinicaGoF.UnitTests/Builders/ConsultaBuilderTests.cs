using Bogus;
using ClinicaGoF.Application.Builders;
using ClinicaGoF.Application.Exceptions;
using Shouldly;

namespace ClinicaGoF.UnitTests.Builders
{
    public class ConsultaBuilderTests
    {
        private readonly Faker _faker;

        public ConsultaBuilderTests()
        {
            _faker = new Faker("pt_BR");
        }

        [Fact]
        public async Task ConstruirAsync_WithValidData_ShouldCreateConsultaObject()
        {
            // Arrange
            var pacienteId = Guid.NewGuid();
            var medicoId = Guid.NewGuid();
            var dataHora = DateTime.Now.AddDays(1);
            var observacoes = _faker.Lorem.Sentence();

            var builder = new ConsultaBuilder()
                .ComPaciente(pacienteId)
                .ComMedico(medicoId)
                .NaData(dataHora)
                .ComObservacoes(observacoes);

            // Act
            var consulta = await builder.ConstruirAsync();

            // Assert
            consulta.ShouldNotBeNull();
            consulta.PacienteId.ShouldBe(pacienteId);
            consulta.MedicoId.ShouldBe(medicoId);
            consulta.DataHora.ShouldBe(dataHora);
            consulta.Observacoes.ShouldBe(observacoes);
            consulta.Id.ShouldNotBe(Guid.Empty);
        }

        [Fact]
        public async Task ConstruirAsync_WithFailedValidation_ShouldThrowException()
        {
            // Arrange
            var pacienteId = Guid.NewGuid();
            var medicoId = Guid.NewGuid();
            var dataHora = DateTime.Now.AddDays(1);
            
            var errorMessage = "Médico não disponível neste horário";
            
            var builder = new ConsultaBuilder()
                .ComPaciente(pacienteId)
                .ComMedico(medicoId)
                .NaData(dataHora)
                .AdicionarValidacao(() => Task.FromResult(
                    new Application.ValidationResult(false, errorMessage)));

            // Act & Assert
            var exception = await Should.ThrowAsync<ConsultaInvalidaException>(
                async () => await builder.ConstruirAsync());
            
            exception.Message.ShouldBe(errorMessage);
        }

        [Fact]
        public async Task ConstruirAsync_WithMultipleValidations_ShouldCreateConsultaWhenAllPass()
        {
            // Arrange
            var pacienteId = Guid.NewGuid();
            var medicoId = Guid.NewGuid();
            var dataHora = DateTime.Now.AddDays(1);
            var observacoes = _faker.Lorem.Sentence();

            var builder = new ConsultaBuilder()
                .ComPaciente(pacienteId)
                .ComMedico(medicoId)
                .NaData(dataHora)
                .ComObservacoes(observacoes)
                .AdicionarValidacao(() => Task.FromResult(
                    new Application.ValidationResult (true)))
                .AdicionarValidacao(() => Task.FromResult(
                    new Application.ValidationResult(true)));

            // Act
            var consulta = await builder.ConstruirAsync();

            // Assert
            consulta.ShouldNotBeNull();
            consulta.PacienteId.ShouldBe(pacienteId);
            consulta.MedicoId.ShouldBe(medicoId);
        }
    }
}