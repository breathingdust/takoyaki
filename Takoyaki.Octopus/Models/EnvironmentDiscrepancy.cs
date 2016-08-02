namespace Takoyaki.Octopus.Models
{
    public class EnvironmentDiscrepancy
    {
        public string ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string ReleaseOneId { get; set; }
        public string ReleaseOneName { get; set; }
        public string ReleaseOneDeployDate { get; set; }
        public string ReleaseTwoId { get; set; }
        public string ReleaseTwoName { get; set; }
        public string ReleaseTwoDeployDate { get; set; }
        public string AgeInDays { get; set; }
    }
}