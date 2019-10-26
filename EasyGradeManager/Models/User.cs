using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EasyGradeManager.Models
{
    public class User
    {
        public User()
        {
            this.Roles = new HashSet<Role>();
        }
        public int Id { get; set; }
        [Required]
        public string Identifier { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        public virtual ICollection<Role> Roles { get; set; }
    }
}
