using System.Collections.Generic;

namespace Takoyaki.Octopus.Models
{
    public class EnvironmentComparisonModel
    {
        public EnvironmentComparisonModel()
        {
            Discrepancies = new List<EnvironmentDiscrepancy>();
        }
        public string EnvironmentOneId { get; set; }
        public string EnvironmentOneName { get; set; }
        public string EnvironmentTwoId { get; set; }
        public string EnvironmentTwoName { get; set; }
        public int TotalProjects { get; set; }

        public int ProjectsWithDiscrepancies => Discrepancies.Count;

        public List<EnvironmentDiscrepancy> Discrepancies { get; set; }
    }
}