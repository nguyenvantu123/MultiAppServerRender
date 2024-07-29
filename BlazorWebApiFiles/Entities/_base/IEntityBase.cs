using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BlazorWebApiFiles.Entity._base
{
    public interface IEntityBase
    {
        [Key]
        Guid Id { get; set; }

        bool IsDeleted { get; set; }

        Guid? InsertedById { get; set; }

        DateTime InsertedAt { get; set; }

        Guid? UpdatedById { get; set; }

        DateTime UpdatedAt { get; set; }

        public string UserNameInserted { get; set; }

        public string UserNameUpdated { get; set; }
    }
}
