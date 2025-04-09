namespace RemTech.Shared.SDK.Validators;

public interface IValidator<in TValidatee>
{
    ValidationResult Validate(TValidatee validatee);
}
