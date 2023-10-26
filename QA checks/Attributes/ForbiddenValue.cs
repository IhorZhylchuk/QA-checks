using System.ComponentModel.DataAnnotations;

namespace QA_checks.Controllers
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
        AllowMultiple = false)]
    public class ForbiddenValue : ValidationAttribute
    {
        private readonly string _forbiddenValue;

        public ForbiddenValue(string forbiddenValue)
        {
            _forbiddenValue = forbiddenValue;
        }

        public override bool IsValid(object value)
        {

            if (string.Equals(value.ToString(), _forbiddenValue, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return true;
        }

        public override string FormatErrorMessage(string name)
        {
            return "Please, make sure you entered correct data!";
        }
    }
}

