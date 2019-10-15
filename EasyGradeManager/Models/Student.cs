namespace EasyGradeManager.Models
{
    //[Table("Étudiant")]
    public class Student:Person
    {
        //[Required] //next property is NOT NULL
        public string FirstName { get; set; }
        public int Age { get; set; }

    }
}
