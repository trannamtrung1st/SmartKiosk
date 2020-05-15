using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SK.Business.Models
{
    public class ValidationResult
    {
        public bool Valid { get; set; }
        public AppResultBuilder Result { get; set; }

        public static ValidationResult Pass()
        {
            return new ValidationResult
            {
                Valid = true
            };
        }

        public static ValidationResult Fail(AppResultBuilder result)
        {
            return new ValidationResult
            {
                Valid = false,
                Result = result
            };
        }
    }
}
