using System.Web.Mvc;
using Takoyaki.Octopus;

namespace Takoyaki.Web.Controllers
{
    public class OctopusController : Controller
    {
        private readonly IOctopusService _octopusService;

        public OctopusController(IOctopusService octopusService)
        {
            _octopusService = octopusService;
        }

        [HttpGet]
        public JsonResult Compare(string environmentOne, string environmentTwo)
        {
            return Json(_octopusService.HighlightDeployedVersionDifferences(environmentOne, environmentTwo), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult AvailableEnvironments()
        {
            return Json(_octopusService.AvailableEnvironments(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Deploy(DeployModel deployModel)
        {
            return Json(_octopusService.Deploy(deployModel.ReleaseId, deployModel.EnvironmentId));
        }

        [HttpGet]
        public JsonResult GetServerTaskStatus(string taskId)
        {
            return Json(_octopusService.GetServerTaskStatus(taskId), JsonRequestBehavior.AllowGet);
        }

        public class DeployModel
        {
            public string ReleaseId { get; set; }
            public string EnvironmentId { get; set; }
        }
    }
}
