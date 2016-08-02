using System;
using System.Collections.Generic;
using Octopus.Client.Model;
using Takoyaki.Octopus.Models;

namespace Takoyaki.Octopus
{
    public interface IOctopusService
    {
        EnvironmentComparisonModel HighlightDeployedVersionDifferences(string environment1, string environment2);
        List<AvailableEnvironment> AvailableEnvironments();
        string Deploy(string releaseId, string deploymentId);
        string GetServerTaskStatus(string taskUrl);
    }
}