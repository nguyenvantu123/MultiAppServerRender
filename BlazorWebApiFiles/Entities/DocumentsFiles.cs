using BlazorIdentityFiles.Entity._base;
using BlazorIdentityFiles.SeedWork;
using Microsoft.Identity.Client;

namespace BlazorIdentity.Files.Entities
{
    public class DocumentsFiles : EntityBase, IAggregateRoot
    {
        public string Name { get; set; }

        public Guid DocumentsTypeId { get; set; }

        public virtual DocumentsType DocumentsType { get; set; }

        public string FilePath { get; set; }
    }
}
