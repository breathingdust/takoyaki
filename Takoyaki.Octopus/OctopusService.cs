using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using log4net;
using Octopus.Client;
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

        public EnvironmentComparisionModel HighlightDeployedVersionDifferences(string environment1, string environment2)
        {
            var environmentDescriptor = _repository.Environments.FindByName(environment1);
            var environmentDescriptor2 = _repository.Environments.FindByName(environment2);

            var projects = _repository.Projects.FindAll();

            var model = new EnvironmentComparisionModel
            {
                One = environment1,
                Two = environment2,
                TotalProjects = projects.Count
            };

            foreach (var projectResource in projects)
            {
                var deployedOne = _repository.Deployments.FindAll(new[] { projectResource.Id },
                    new[] { environmentDescriptor.Id }).Items.FirstOrDefault();
                var deployedTwo = _repository.Deployments.FindAll(new[] { projectResource.Id },
                    new[] { environmentDescriptor2.Id }).Items.FirstOrDefault();

                if ((deployedOne != null && deployedTwo != null)
                    && deployedOne.ReleaseId != deployedTwo.ReleaseId)
                {
                    var releaseOne = _repository.Releases.Get(deployedOne.ReleaseId);
                    var releaseTwo = _repository.Releases.Get(deployedTwo.ReleaseId);

                    model.Discrepancies.Add(new EnvironmentDiscrepancy
                    {
                        Project = projectResource.Name,
                        VersionOne = releaseOne.Version,
                        VersionOneDeployDate = deployedOne.Created.ToString("g"),
                        VersionTwo = releaseTwo.Version,
                        VersionTwoDeployDate = deployedTwo.Created.ToString("g"),
                        AgeInDays = (deployedTwo.Created - deployedOne.Created).Days.ToString()
                    });
                }
            }
            return model;
        }
    }
}
