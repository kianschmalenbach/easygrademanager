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
        public override bool Equals(object other)
        {
            return other != null && other is GroupMembership && Id == ((GroupMembership)other).Id;
        }
        public override int GetHashCode()
        {
            return Id;
        }
    }
}
