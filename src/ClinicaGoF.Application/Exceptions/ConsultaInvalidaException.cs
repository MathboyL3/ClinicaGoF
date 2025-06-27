using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicaGoF.Application.Exceptions
{
    public class ConsultaInvalidaException : Exception
    {
        public ConsultaInvalidaException(string message) : base(message)
        {
        }
    }
}
