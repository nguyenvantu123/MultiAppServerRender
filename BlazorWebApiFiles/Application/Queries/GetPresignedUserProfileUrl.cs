using BlazorWebApi.Files.Constant;
using System.Runtime.Serialization;


namespace BetkingLol.Domain.Request.Queries.BankInfoByUser
{
    public class GetPresignedUserProfileUrl
    {
        public string ObjectName { get; set; }

        public string RelationType { get; set; }

        public Guid? RelationId { get; set; }

    }
}
