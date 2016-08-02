using System.Collections.Generic;
using Takoyaki.Octopus.Models;

namespace Takoyaki.Octopus
{
    public interface IOctopusService
    {
        EnvironmentComparisionModel HighlightDeployedVersionDifferences(string environment1, string environment2);
        List<AvailableEnvironment> AvailableEnvironments();
    }
}