using TechFlow.Domain.Common;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Domain.Tasks.Attachments;

public sealed class Attachment : Entity
{
    public Guid TaskId { get; private set; }
    public Guid UploadedByUserId { get; private set; }
    public string FileName { get; private set; } = string.Empty;
    public string FileUrl { get; private set; } = string.Empty;
    public string FileType { get; private set; } = string.Empty;
    public long FileSizeBytes { get; private set; }
    public DateTimeOffset UploadedAt { get; private set; }

    private static readonly long MaxFileSizeBytes = 20 * 1024 * 1024; // 20 MB

    private Attachment() { }

    private Attachment(Guid id, Guid taskId, Guid uploadedByUserId, string fileName, string fileUrl, string fileType, long fileSizeBytes)
        : base(id)
    {
        TaskId = taskId;
        UploadedByUserId = uploadedByUserId;
        FileName = fileName;
        FileUrl = fileUrl;
        FileType = fileType;
        FileSizeBytes = fileSizeBytes;
        UploadedAt = DateTimeOffset.UtcNow;
    }

    // ── Factory ────────────────────────────────────────────────────────────────

    internal static Result<Attachment> Create(
        Guid taskId,
        Guid uploadedByUserId,
        string fileName,
        string fileUrl,
        string fileType,
        long fileSizeBytes)
    {
        if (!IsValidId(taskId))
            return AttachmentErrors.TaskIdRequired;

        if (!IsValidId(uploadedByUserId))
            return AttachmentErrors.UploadedByRequired;

        if (string.IsNullOrWhiteSpace(fileName))
            return AttachmentErrors.FileNameRequired;

        if (string.IsNullOrWhiteSpace(fileUrl))
            return AttachmentErrors.FileUrlRequired;

        if (!IsValidFileSize(fileSizeBytes))
            return AttachmentErrors.FileTooLarge;

        return new Attachment(
            id: Guid.NewGuid(),
            taskId: taskId,
            uploadedByUserId: uploadedByUserId,
            fileName: fileName.Trim(),
            fileUrl: fileUrl.Trim(),
            fileType: fileType.Trim().ToLower(),
            fileSizeBytes: fileSizeBytes
        );
    }

    // ── Private Validation ─────────────────────────────────────────────────────

    private static bool IsValidId(Guid id) => id != Guid.Empty;
    private static bool IsValidFileSize(long bytes) => bytes > 0 && bytes <= MaxFileSizeBytes;
}