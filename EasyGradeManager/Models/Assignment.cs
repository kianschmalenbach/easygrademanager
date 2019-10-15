using System.Collections.ObjectModel;

namespace EasyGradeManager.Models
{
    public class Assignment
    {
        public int Id { get; set; }
        public Collection<EvaluationCriterion> EvaluationCriteria { get; set; }

    }
}
