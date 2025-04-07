namespace WebApp.Models
{
    public class DocumentsTypes
    {

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public string LinkUrl { get; set; }

        public bool FileActive { get; set; } = true;
        public string? InsertedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? InsertedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
