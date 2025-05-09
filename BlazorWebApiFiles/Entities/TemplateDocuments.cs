namespace BlazorIdentity.Files.Entities
{
    public class TemplateDocuments
    {

        public Guid DocumentTypeId { get; set; }

        public virtual DocumentsType DocumentsType { get; set; }

        public string FileUrl { get; set; }

    }
}
