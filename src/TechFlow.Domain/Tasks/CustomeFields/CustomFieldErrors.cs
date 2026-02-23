using TechFlow.Domain.Common.Results;

namespace TechFlow.Domain.Tasks.CustomeFields;

public static class CustomFieldErrors
{
    public static readonly Error ProjectIdRequired =
        Error.Validation("CustomField.ProjectIdRequired", "Project ID is required.");

    public static readonly Error TaskIdRequired =
        Error.Validation("CustomField.TaskIdRequired", "Task ID is required.");

    public static readonly Error DefinitionIdRequired =
        Error.Validation("CustomField.DefinitionIdRequired", "Custom field definition ID is required.");

    public static readonly Error FieldNameRequired =
        Error.Validation("CustomField.FieldNameRequired", "Field name is required.");

    public static readonly Error ValueRequired =
        Error.Validation("CustomField.ValueRequired", "Field value is required.");

    public static readonly Error OptionsRequiredForDropdown =
        Error.Validation("CustomField.OptionsRequiredForDropdown", "Dropdown fields must have at least one option.");

    public static readonly Error OptionsOnlyForDropdown =
        Error.Validation("CustomField.OptionsOnlyForDropdown", "Options can only be set for Dropdown fields.");

    public static Error InvalidFieldType(string type) =>
        Error.Validation("CustomField.InvalidFieldType",
            $"'{type}' is not valid. Valid types: Text, Number, Dropdown, Url, Date.");

    public static readonly Error NotFound =
        Error.NotFound("CustomField.NotFound", "Custom field was not found.");

    public static readonly Error DefinitionNotFound =
        Error.NotFound("CustomField.DefinitionNotFound", "Custom field definition was not found.");
}