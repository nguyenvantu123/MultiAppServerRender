using System.ComponentModel.DataAnnotations.Schema;
using Finbuckle.MultiTenant;

namespace BlazorWebApi.Users.Models
{
    [Permissions(Actions.CRUD)]
    [MultiTenant]
    public partial class TenantSetting
    {
        [Column(TypeName = "nvarchar(64)")]
        public string TenantId { get; set; }

        [Column(TypeName = "nvarchar(128)")]
        public SettingKey Key { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string Value { get; set; }

        [Required(ErrorMessage = "FieldRequired")]
        public SettingType Type { get; set; }
    }
}
