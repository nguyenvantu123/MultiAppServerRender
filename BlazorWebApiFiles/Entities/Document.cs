using BlazorWebApi.Files.Constant;
using BlazorWebApiFiles.Entity._base;
using BlazorWebApiFiles.Seedwork;
using OpenTelemetry.Metrics;

namespace BlazorWebApi.Files.Entities
{
    public class Document : EntityBase, IAggregateRoot
    {

        public string Name { get; set; }

        public string Description { get; set; }

        public DocumentType DocumentType { get; set; }

        public DocumentStatus DocumentStatus { get; set; }

        public int Version { get; set; }
    }
}
