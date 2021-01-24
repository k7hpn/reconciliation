using Spectre.Console;
using Spectre.Console.Cli;

namespace Reconciliation.Commands.Validation
{
    internal sealed class ValidateStringAttribute : ParameterValidationAttribute
    {
        public const int MinimumLength = 3;

#nullable disable
        public ValidateStringAttribute() : base(errorMessage: null)
        {
        }
#nullable enable

        public override ValidationResult Validate(ICommandParameterInfo info, object? value)
            => (value as string) switch
            {
                { Length: >= MinimumLength }
                    => ValidationResult.Success(),

                { Length: < MinimumLength }
                    => ValidationResult.Error($"{info?.PropertyName} ({value}) needs to be at least {MinimumLength} characters long."),

                _ => ValidationResult.Error($"Invalid {info?.PropertyName} ({value}) specified.")
            };
    }
}