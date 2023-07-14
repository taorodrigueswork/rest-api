using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Entities.Entity;

[ExcludeFromCodeCoverage]
public class BaseEntity
{
    public string Validate()
    {
        ValidationContext context = new(this, serviceProvider: null, items: null);
        List<ValidationResult> results = new();
        var isValid = Validator.TryValidateObject(this, context, results, true);

        if (!isValid)
        {
            StringBuilder sbrErrors = new();
            foreach (var validationResult in results)
            {
                sbrErrors.AppendLine(validationResult.ErrorMessage);
            }
            return sbrErrors.ToString();
        }

        return string.Empty;
    }
}
