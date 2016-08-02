using System.Collections.Generic;

namespace Takoyaki.Octopus.Models
{
    public class EnvironmentComparisionModel
    {
        public EnvironmentComparisionModel()
        {
            Discrepancies = new List<EnvironmentDiscrepancy>();
        }
        public string One { get; set; }
        public string Two { get; set; }
        public int TotalProjects { get; set; }

        public int ProjectsWithDiscrepancies => Discrepancies.Count;

        public List<EnvironmentDiscrepancy> Discrepancies { get; set; }
    }
}