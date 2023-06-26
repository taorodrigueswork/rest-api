using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Persistence.Entities;

[ExcludeFromCodeCoverage]
public class BaseEntity
{
    public string Validate()
    {
        ValidationContext context = new ValidationContext(this, serviceProvider: null, items: null);
        List<ValidationResult> results = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(this, context, results, true);

        if (!isValid)
        {
            StringBuilder sbrErrors = new StringBuilder();
            foreach (var validationResult in results)
            {
                sbrErrors.AppendLine(validationResult.ErrorMessage);
            }
            return sbrErrors.ToString();
        }

        return string.Empty;
    }
}
