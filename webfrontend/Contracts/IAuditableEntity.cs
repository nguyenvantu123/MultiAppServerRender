using System.ComponentModel.DataAnnotations;

namespace BlazorWeb.Contracts
{

    public interface IAuditableEntity
    {
        [Key]
        public Guid Id { get; set; }
        string CreatedBy { get; set; }

        DateTime CreatedOn { get; set; }

        string LastModifiedBy { get; set; }

        DateTime? LastModifiedOn { get; set; }
    }
}