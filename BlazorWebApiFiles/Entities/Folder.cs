using BlazorIdentityFiles.Entity._base;
using BlazorIdentityFiles.SeedWork;
using Finbuckle.MultiTenant;

namespace BlazorIdentity.Files.Entities
{
    public class Folder : EntityBase, IAggregateRoot
    {
        public string Name { get; set; }

        public string FolderPath { get; set; }

        public Guid? ParentId { get; set; }

        public ICollection<FileData> Files { get; set; }

        public string TenantId { get; set; }

    }
}
