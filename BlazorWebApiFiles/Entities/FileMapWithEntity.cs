using BlazorIdentityFiles.Entity._base;
using BlazorIdentityFiles.SeedWork;

namespace BlazorIdentity.Files.Entities
{
    public class FileMapWithEntity : EntityBase, IAggregateRoot
    {

        public string RelationType { get; set; }

        public Guid RelationId { get; set; }

        public string FileName { get; set; }

        public Guid FileId { get; set; }
    }
}
