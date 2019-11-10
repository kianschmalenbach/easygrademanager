using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyGradeManager.Models
{
    public class User
    {
        public User()
        {
            this.Roles = new HashSet<Role>();
        }
        public int Id { get; set; }
        [Required, Index(IsUnique = true), StringLength(450)]
        public string Identifier { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Name { get; set; }
        [Required, Index(IsUnique = true), StringLength(450)]
        public string Email { get; set; }
        public virtual ICollection<Role> Roles { get; set; }
        public bool Equals(User other)
        {
            return other != null && Id == other.Id;
        }
    }
}
