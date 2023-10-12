using System.ComponentModel.DataAnnotations;

namespace QA_checks.Controllers
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
       AllowMultiple = false)]
    public class MinValue: ValidationAttribute
    {
        private readonly int _minValue;

        public MinValue(int minValue)
        {
            _minValue = minValue;
        }

        public override bool IsValid(object value)
        {
            if(Convert.ToInt32(value) > _minValue)
            {
                return true;
            }

            return false;
        }

        public override string FormatErrorMessage(string message)
        {
            return "Value should be more then '0'";
        }
    }
}
