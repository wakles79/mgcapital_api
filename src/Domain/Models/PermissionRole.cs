
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MGCap.Domain.Models
{
    public class PermissionRole : Entity
    {
        [Key]
        [Required]
        public int PermissionId { get; set; }

        [Key]
        [Required]
        public int RoleId { get; set; }

        [ForeignKey("PermissionId")]
        public Permission Permission { get; set; }

        [ForeignKey("RoleId")]
        public Role Role { get; set; }
    }
}
