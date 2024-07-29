using BlazorWebApiFiles.Entity._base;
using Finbuckle.MultiTenant;

namespace BlazorWebApi.Files.Entities
{
    [MultiTenant]
    public class Folder : EntityBase
    {
        public string Name { get; set; }

        public string FolderPath { get; set; }

        public Guid? ParentId { get; set; }

        public ICollection<FileData> Files { get; set; }
    }
}
