namespace EasyGradeManager.Models
{
    public class Grade
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double MinPercentage { get; set; }
        public int GradingSchemeId { get; set; }
        public virtual GradingScheme GradingScheme { get; set; }
        public override bool Equals(object other)
        {
            return other != null && other is Grade && Id == ((Grade)other).Id && 
                (Id != 0 || MinPercentage == ((Grade)other).MinPercentage);
        }
        public override int GetHashCode()
        {
            return Id;
        }
    }

    public class GradeDTO
    {
        public GradeDTO(Grade grade)
        {
            if (grade != null)
            {
                Id = grade.Id;
                Name = grade.Name;
                MinPercentage = grade.MinPercentage;
            }
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public double MinPercentage { get; set; }
        public override bool Equals(object other)
        {
            return other != null && other is GradeDTO && Id == ((GradeDTO)other).Id;
        }
        public override int GetHashCode()
        {
            return Id;
        }
        public bool Validate()
        {
            return Name != null && MinPercentage >= 0 && MinPercentage <= 1;
        }
        private void Update(Grade grade)
        {
            grade.MinPercentage = MinPercentage;
            grade.Name = Name;
        }
        public Grade Create()
        {
            Grade grade = new Grade();
            Update(grade);
            return grade;
        }
    }
}
