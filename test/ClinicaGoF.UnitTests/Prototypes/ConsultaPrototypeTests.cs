using System;
using ClinicaGoF.Domain.Entities;
using Shouldly;
using Xunit;

namespace ClinicaGoF.UnitTests.Prototypes
{
    public class ConsultaPrototypeTests
    {
        [Fact]
        public void Clone_ShouldCreateNewInstance_WithSameProperties()
        {
            // Arrange
            var original = new Consulta
            {
                PacienteId = Guid.NewGuid(),
                MedicoId = Guid.NewGuid(),
                DataHora = DateTime.Now.AddDays(1),
                Observacoes = "Consulta original"
            };
            
            var originalId = original.Id;

            // Act
            var clone = original.Clone();

            // Assert
            clone.ShouldNotBeNull();
            clone.ShouldNotBeSameAs(original);  // Different reference
            clone.Id.ShouldNotBe(originalId);   // Different ID
            
            // Same properties
            clone.PacienteId.ShouldBe(original.PacienteId);
            clone.MedicoId.ShouldBe(original.MedicoId);
            clone.DataHora.ShouldBe(original.DataHora);
            clone.Observacoes.ShouldBe(original.Observacoes);
        }

        [Fact]
        public void CloneWithNewDateTime_ShouldCreateNewInstance_WithUpdatedDateTime()
        {
            // Arrange
            var original = new Consulta
            {
                PacienteId = Guid.NewGuid(),
                MedicoId = Guid.NewGuid(),
                DataHora = DateTime.Now.AddDays(1),
                Observacoes = "Consulta original"
            };
            
            var originalId = original.Id;
            var newDateTime = DateTime.Now.AddDays(7);

            // Act
            var clone = original.CloneWithNewDateTime(newDateTime);

            // Assert
            clone.ShouldNotBeNull();
            clone.ShouldNotBeSameAs(original);       // Different reference
            clone.Id.ShouldNotBe(originalId);        // Different ID
            clone.DataHora.ShouldBe(newDateTime);    // Updated date/time
            
            // Same other properties
            clone.PacienteId.ShouldBe(original.PacienteId);
            clone.MedicoId.ShouldBe(original.MedicoId);
            clone.Observacoes.ShouldBe(original.Observacoes);
        }

        [Fact]
        public void MultipleClones_ShouldAllHaveDifferentIds()
        {
            // Arrange
            var original = new Consulta
            {
                PacienteId = Guid.NewGuid(),
                MedicoId = Guid.NewGuid(),
                DataHora = DateTime.Now,
                Observacoes = "Original"
            };

            // Act
            var clone1 = original.Clone();
            var clone2 = original.Clone();
            var clone3 = original.CloneWithNewDateTime(DateTime.Now.AddDays(3));

            // Assert
            var allIds = new[] { original.Id, clone1.Id, clone2.Id, clone3.Id };
            allIds.ShouldAllBe(id => id != Guid.Empty);
            allIds.Length.ShouldBe(allIds.Distinct().Count());  // All IDs should be unique
        }
    }
}