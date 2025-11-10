using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyInvoiceApp_Shared.DTO
{
    public class Validation
    {
        public class ErrorResponse
        {
            public string Message { get; set; }
            public List<ValidationError> Errors { get; set; } = new();
        }

        public class ValidationError
        {
            public string Property { get; set; }
            public string Error { get; set; }
        }
    }
}
