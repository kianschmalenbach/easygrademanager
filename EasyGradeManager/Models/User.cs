using EasyGradeManager.Static;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyGradeManager.Models
{
    public class User
    {
        public User()
        {
            Roles = new HashSet<Role>();
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
        public virtual ICollection<Role> Roles { get; }
        public override bool Equals(object other)
        {
            return other != null && other is User && Id == ((User)other).Id;
        }
        public override int GetHashCode()
        {
            return Id;
        }
        public Teacher GetTeacher()
        {
            foreach(Role role in Roles)
            {
                if (role.Name == "Teacher")
                    return (Teacher)role;
            }
            return null;
        }
        public Tutor GetTutor()
        {
            foreach (Role role in Roles)
            {
                if (role.Name == "Tutor")
                    return (Tutor)role;
            }
            return null;
        }
        public Student GetStudent()
        {
            foreach (Role role in Roles)
            {
                if (role.Name == "Student")
                    return (Student)role;
            }
            return null;
        }
    }

    public class UserListDTO
    {
        public int Id { get; set; }
        public string Identifier { get; set; }
        public string Name { get; set; }
        public override bool Equals(object other)
        {
            return other != null && other is UserListDTO && Id == ((UserListDTO)other).Id;
        }
        public override int GetHashCode()
        {
            return Id;
        }
    }

    public class UserDetailDTO : UserListDTO
    {
        public string Email { get; set; }
        public string NewPassword { get; set; }
        public string NewRole { get; set; }
        public TeacherDTO Teacher { get; set; }
        public TutorDTO Tutor { get; set; }
        public StudentDTO Student { get; set; }
        public override bool Equals(object other)
        {
            return other != null && other is UserDetailDTO && Id == ((UserDetailDTO)other).Id;
        }
        public override int GetHashCode()
        {
            return Id;
        }

        internal bool Update(User user)
        {
            bool logoutNecessary = false;
            user.Email = Email;
            if (NewPassword != null) 
            {
                logoutNecessary = true;
                user.Password = SecurePasswordHasher.Hash(NewPassword);
            }
            if (user.Identifier != Identifier)
            {
                logoutNecessary = true;
                user.Identifier = Identifier;
            }
            user.Name = Name;
            return logoutNecessary;
        }
    }
}
