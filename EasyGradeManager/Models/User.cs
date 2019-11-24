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
        public UserDetailDTO(User user, ICollection<Course> courses) : base(user)
        {
            Courses = new HashSet<CourseListDTO>();
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
            if (courses != null)
            {
                foreach (Course course in courses)
                    Courses.Add(new CourseListDTO(course));
            }
        }
        public string Email { get; set; }
        public string NewPassword { get; set; }
        public string NewRole { get; set; }
        public int NewUserId { get; set; }
        public string NewUserIdentifier { get; set; }
        public TeacherDTO Teacher { get; set; }
        public TutorDTO Tutor { get; set; }
        public StudentDTO Student { get; set; }
        public ICollection<CourseListDTO> Courses { get; }
        public override bool Equals(object other)
        {
            return other != null && other is UserDetailDTO && Id == ((UserDetailDTO)other).Id;
        }
        public override int GetHashCode()
        {
            return Id;
        }
        public bool Validate(bool create, User authorizedUser)
        {
            if (authorizedUser != null && authorizedUser.Id != Id)
                return authorizedUser.GetTeacher() != null && (NewRole.Equals("Teacher") || NewRole.Equals("Tutor"));
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
        public void UpdateRole(User user)
        {
            bool[] roles = { false, false, false };
            foreach (Role role in user.Roles)
            {
                if (role.Name.Equals("Teacher"))
                    roles[0] = true;
                else if (role.Name.Equals("Tutor"))
                    roles[1] = true;
                else if (role.Name.Equals("Student"))
                    roles[2] = true;
            }
            switch (NewRole)
            {
                case "Teacher":
                    if (!roles[0])
                        user.Roles.Add(new Teacher());
                    break;
                case "Tutor":
                    if (!roles[1])
                        user.Roles.Add(new Tutor());
                    break;
                case "Student":
                    if (!roles[2])
                        user.Roles.Add(new Student());
                    break;
            }
        }
    }
}
