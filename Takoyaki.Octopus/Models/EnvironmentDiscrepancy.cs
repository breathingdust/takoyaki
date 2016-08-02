namespace Takoyaki.Octopus.Models
{
    public class EnvironmentDiscrepancy
    {
        public string Project { get; set; }
        public string VersionOne { get; set; }
        public string VersionOneDeployDate { get; set; }
        public string VersionTwo { get; set; }
        public string VersionTwoDeployDate { get; set; }
        public string AgeInDays { get; set; }
    }
}