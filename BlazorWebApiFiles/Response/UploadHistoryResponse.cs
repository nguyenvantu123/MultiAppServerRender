using BlazorIdentityFiles.Entity._base;

namespace BlazorIdentity.Files.Response
{
    public class UploadHistoryResponse
    {

        public Guid Id { get; set; }
        public DateTime InsertedAt { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }

    }
}
