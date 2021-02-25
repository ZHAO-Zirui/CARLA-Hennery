using System.Linq;
using CARLA_Hennery.Master.Managers;
using CARLA_Hennery.Master.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CARLA_Hennery.Master.Controllers
{
    public class VersionController : Controller
    {
        private readonly VersionManager _version;

        public VersionController(VersionManager version)
        {
            _version = version;
        }

        public IActionResult Carla()
        {
            var model = new CarlaVersionViewModel {KnownCarlaVersions = _version.KnownCarlaVersions.ToList()};
            model.KnownCarlaVersions.Sort();
            model.KnownCarlaVersions.Reverse();
            return View(model);
        }
    }
}