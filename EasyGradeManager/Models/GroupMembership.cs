using System.ComponentModel.DataAnnotations;

namespace EasyGradeManager.Models
{
    public class GroupMembership
    {
        public int Id { get; set; }
        [Required]
        public int StudentId { get; set; }
        public virtual Student Student { get; set; }
        [Required]
        public int GroupId { get; set; }
        public virtual Group Group { get; set; }
    }
}
