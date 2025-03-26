using BlazorIdentity.Files.Constant;
using BlazorIdentityFiles.Entity._base;
using BlazorIdentityFiles.SeedWork;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorIdentity.Files.Entities
{
    public class FileData : EntityBase, IAggregateRoot
    {

        public string Name { get; set; }

        public string? AlternativeText { get; set; }

        public string? Caption { get; set; }

        public int? Width { get; set; }

        public int? Height { get; set; }

        public string? Format { get; set; }

        public string? Hash { get; set; }

        public string? Ext { get; set; }

        public string? Mime { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal? Size { get; set; }

        public string? PreviewUrl { get; set; }

        public string? Provider { get; set; }

        public string? Provider_Metadata { get; set; }

        public string? FolderPath { get; set; }

        public Guid? FolderId { get; set; }

        public string? TenantId { get; set; }

        public FileType FileTypeData { get; set; }

        public string? FileUrl { get; set; }
    }
}
