using TechFlow.Domain.Common.Results;

namespace TechFlow.Domain.Tasks.Attachments;

public static class AttachmentErrors
{
    public static readonly Error TaskIdRequired =
        Error.Validation("Attachment.TaskIdRequired", "Task ID is required.");

    public static readonly Error UploadedByRequired =
        Error.Validation("Attachment.UploadedByRequired", "Uploaded by user ID is required.");

    public static readonly Error FileNameRequired =
        Error.Validation("Attachment.FileNameRequired", "File name is required.");

    public static readonly Error FileUrlRequired =
        Error.Validation("Attachment.FileUrlRequired", "File URL is required.");

    public static readonly Error FileTooLarge =
        Error.Validation("Attachment.FileTooLarge", "File size cannot exceed 20 MB.");

    public static readonly Error NotFound =
        Error.NotFound("Attachment.NotFound", "Attachment was not found.");
}
