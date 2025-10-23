using FluentValidation;

namespace NcpAdminBlazor.Client.Models;

public class FormModelValidator<TModel> : AbstractValidator<TModel>
{
    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var result =
            await ValidateAsync(
                ValidationContext<TModel>.CreateWithOptions((TModel)model, x => x.IncludeProperties(propertyName)));
        return result.IsValid ? [] : result.Errors.Select(e => e.ErrorMessage);
    };
}