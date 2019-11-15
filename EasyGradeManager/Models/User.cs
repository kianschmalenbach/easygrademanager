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
            foreach (Role role in Roles)
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
        public bool DeleteRoles(EasyGradeManagerContext db)
        {
            if (GetTeacher() != null)
            {
                if (GetTeacher().Courses.Count != 0)
                    return false;
                db.Teachers.Remove(GetTeacher());
            }
            if (GetTutor() != null)
            {
                if (GetTutor().Lessons.Count != 0)
                    return false;
                db.Tutors.Remove(GetTutor());
            }
            if (GetStudent() != null)
            {
                if (GetStudent().GroupMemberships.Count != 0)
                    return false;
                db.Students.Remove(GetStudent());
            }
            return true;
        }
    }

    public class UserListDTO
    {
        public UserListDTO(User user)
        {
            if (user != null)
            {
                Id = user.Id;
                Identifier = user.Identifier;
                Name = user.Name;
            }
        }
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
        public UserDetailDTO(User user) : base(user)
        {
            if (user != null)
            {
                Email = user.Email;
                if (user.GetTeacher() != null)
                    Teacher = new TeacherDTO(user.GetTeacher());
                if (user.GetTutor() != null)
                    Tutor = new TutorDTO(user.GetTutor());
                if (user.GetStudent() != null)
                    Student = new StudentDTO(user.GetStudent());
            }
        }
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
        public bool Validate(bool create)
        {
            bool updateProof =
                Email != null && Identifier != null && Name != null && !Identifier.Contains("&") &&
                (NewPassword == null || !NewPassword.Contains("&")) &&
                (NewRole == null || NewRole.Equals("Teacher") || NewRole.Equals("Tutor") || NewRole.Equals("Student"));
            bool createProof =
                updateProof && NewPassword != null && NewRole != null;
            return create ? createProof : updateProof;
        }
        public bool Update(User user)
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
        public User Create()
        {
            User user = new User();
            Update(user);
            return user;
        }
        public void UpdateRole(User user, EasyGradeManagerContext db)
        {

            bool[] roles = { false, false, false };
            foreach (Role role in user.Roles)
            {
                if (role.Name == "Teacher")
                    roles[0] = true;
                else if (role.Name == "Tutor")
                    roles[1] = true;
                else if (role.Name == "Student")
                    roles[2] = true;
            }
            switch (NewRole)
            {
                case "Teacher":
                    if (!roles[0])
                        db.Teachers.Add(new Teacher()
                        {
                            UserId = user.Id
                        });
                    break;
                case "Tutor":
                    if (!roles[1])
                        db.Tutors.Add(new Tutor()
                        {
                            UserId = user.Id
                        });
                    break;
                case "Student":
                    if (!roles[2])
                        db.Students.Add(new Student()
                        {
                            UserId = user.Id
                        });
                    break;
            }
        }
    }
}
