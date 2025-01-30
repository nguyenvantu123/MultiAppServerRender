using BlazorIdentity.Files.Constant;
using System.Runtime.Serialization;


namespace BlazorIdentityFiles.Application.Queries
{
    public class GetPresignedUserProfileUrl
    {
        public string ObjectName { get; set; }

        public string RelationType { get; set; }

        public Guid? RelationId { get; set; }

    }
}
