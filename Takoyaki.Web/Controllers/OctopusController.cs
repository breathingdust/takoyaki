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

        [System.Web.Http.HttpGet]
        public JsonResult Compare(string environmentOne, string environmentTwo)
        {
            return Json(_octopusService.HighlightDeployedVersionDifferences(environmentOne, environmentTwo), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult AvailableEnvironments()
        {
            return Json(_octopusService.AvailableEnvironments(), JsonRequestBehavior.AllowGet);
        }
    }
}
