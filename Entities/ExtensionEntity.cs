using System.Linq.Expressions;
using RMS.Contants;

namespace RMS.Entities;

public static class ExtensionEntity
{
    public static ValidationResult[] StrongValidation<T>(this T entity) where T : class
    {
        switch (entity)
        {
            case Recipe recipe:
                break;
            
            case Ingredient ingredient:
                
                break;

            default:
                throw new NotSupportedException($"Validation not implemented for type {typeof(T).Name}");
        }
        return Array.Empty<ValidationResult>();
    }
}
