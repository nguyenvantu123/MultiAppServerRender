using BlazorWebApi.Files.Constant;
using System.Runtime.Serialization;


namespace BetkingLol.Domain.Request.Queries.BankInfoByUser
{
    [DataContract]
    public class GetPresignedUserProfileUrl
    {
        [DataMember]
        public string ObjectName { get; set; }

        [DataMember]

        public string RelationType { get; set; }

        [DataMember]

        public Guid? RelationId { get; set; }

    }
}
