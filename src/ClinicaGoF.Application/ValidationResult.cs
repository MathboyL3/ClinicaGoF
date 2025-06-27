using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicaGoF.Application
{
    public record ValidationResult(bool IsValid, string ErrorMessage = "")
    {
        public static ValidationResult Success() => new(true);
        public static ValidationResult Failure(string errorMessage) => new(false, errorMessage);
    }
}
