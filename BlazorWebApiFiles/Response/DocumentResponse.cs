using BlazorIdentityFiles.Entity._base;

namespace BlazorIdentity.Files.Response
{
    public class DocumentResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public bool IsActive { get; set; }

        public string? InsertedBy { get; set; }

        public string? UpdatedBy { get; set; }

        public DateTime? InsertedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

    }
}
    