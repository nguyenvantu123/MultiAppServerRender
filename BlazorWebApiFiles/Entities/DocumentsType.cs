using BlazorIdentityFiles.Entity._base;
using BlazorIdentityFiles.SeedWork;

namespace BlazorIdentity.Files.Entities
{
    public class DocumentsType : EntityBase, IAggregateRoot
    {
        public string Name { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }
    }
}
