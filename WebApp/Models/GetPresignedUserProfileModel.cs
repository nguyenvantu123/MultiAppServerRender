namespace WebApp.Models
{
    public class GetPresignedUserProfileModel
    {
        public string ObjectName { get; set; }

        public string RelationType { get; set; }

        public Guid? RelationId { get; set; }
    }
}
