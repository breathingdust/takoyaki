using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using log4net;
using Octopus.Client;
using Octopus.Client.Model;
using Takoyaki.Octopus.Models;

namespace Takoyaki.Octopus
{
    public class OctopusService : IOctopusService
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly OctopusRepository _repository;

        public OctopusService()
        {
            var server = ConfigurationManager.AppSettings["Octopus.Server"];
            var apiKey = ConfigurationManager.AppSettings["Octopus.ApiKey"];
            var endpoint = new OctopusServerEndpoint(server, apiKey);
            _repository = new OctopusRepository(endpoint);
        }

        public List<AvailableEnvironment> AvailableEnvironments()
        {
            var environments = _repository.Environments.FindAll();

            var environmentsToIgnore = ConfigurationManager.AppSettings["Octopus.EnvironmentsToIgnore"].Split(',');

            environments = environments.Where(o => !environmentsToIgnore.Contains(o.Name)).ToList();

            return environments.Select(o => new AvailableEnvironment
            {
                Id = o.Id,
                Name = o.Name
            }).ToList();
        }

        public string Deploy(string releaseId, string environmentId)
        {
            GuardAgainstDeploysToDisabledEnvironments(environmentId);

            var deploymentResource = _repository.Deployments.Create(new DeploymentResource
            {
                Comments = "Automated deploy to consolidate environments",
                EnvironmentId = environmentId,
                ReleaseId = releaseId
            });

            return deploymentResource.TaskId;
        }

        public string GetServerTaskStatus(string taskId)
        {
            return _repository.Tasks.Get(taskId).State.ToString();
        }

        private void GuardAgainstDeploysToDisabledEnvironments(string environmentId)
        {
            var disabledEnvironmentNames =
                ConfigurationManager.AppSettings["Octopus.DisableConsolidationToEnvironments"].Split(',');

            var targetEnvironment = _repository.Environments.Get(environmentId);

            if (targetEnvironment == null)
                throw new Exception($"No environment found with id: {environmentId}");

            if (disabledEnvironmentNames.Contains(targetEnvironment.Name))
                throw new Exception($"Creating deployments targeting the {targetEnvironment.Name} environment is disabled.");
        }


        public EnvironmentComparisonModel HighlightDeployedVersionDifferences(string environment1, string environment2)
        {
            var environmentOneDescriptor = _repository.Environments.FindByName(environment1);
            var environmentTwoDescriptor = _repository.Environments.FindByName(environment2);

            var projects = _repository.Projects.FindAll();

            var model = new EnvironmentComparisonModel
            {
                EnvironmentOneId = environmentOneDescriptor.Id,
                EnvironmentOneName = environmentOneDescriptor.Name,
                EnvironmentTwoId = environmentTwoDescriptor.Id,
                EnvironmentTwoName = environmentTwoDescriptor.Name,
                TotalProjects = projects.Count
            };

            foreach (var projectResource in projects)
            {
                var deployedOne = _repository.Deployments.FindAll(new[] { projectResource.Id },
                    new[] { environmentOneDescriptor.Id }).Items.FirstOrDefault();
                var deployedTwo = _repository.Deployments.FindAll(new[] { projectResource.Id },
                    new[] { environmentTwoDescriptor.Id }).Items.FirstOrDefault();

                if ((deployedOne != null && deployedTwo != null)
                    && deployedOne.ReleaseId != deployedTwo.ReleaseId)
                {
                    var releaseOne = _repository.Releases.Get(deployedOne.ReleaseId);
                    var releaseTwo = _repository.Releases.Get(deployedTwo.ReleaseId);

                    model.Discrepancies.Add(new EnvironmentDiscrepancy
                    {
                        ProjectId = projectResource.Id,
                        ProjectName = projectResource.Name,
                        ReleaseOneId = releaseOne.Id,
                        ReleaseOneName = releaseOne.Version,
                        ReleaseOneDeployDate = deployedOne.Created.ToString("g"),
                        ReleaseTwoId = releaseTwo.Id,
                        ReleaseTwoName = releaseTwo.Version,
                        ReleaseTwoDeployDate = deployedTwo.Created.ToString("g"),
                        AgeInDays = (deployedTwo.Created - deployedOne.Created).Days.ToString()
                    });
                }
            }
            return model;
        }
    }
}
