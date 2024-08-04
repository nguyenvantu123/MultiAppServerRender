using BlazorWebApiFiles.Entity._base;
using BlazorWebApiFiles.SeedWork;

namespace BlazorWebApi.Files.Entities
{
    public class FileMapWithEntity : EntityBase, IAggregateRoot
    {

        public string RelationType { get; set; }

        public Guid RelationId { get; set; }

        public string FileName { get; set; }

        public Guid FileId { get; set; }
    }
}
