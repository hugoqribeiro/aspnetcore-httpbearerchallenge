using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ragnar.RestApi.Controllers
{
    public partial class TrucksController : Controller
    {
        #region Public Methods

        [HttpGet("/trucks", Name = "TrucksController.GetTrucks")]
        [Authorize("read-trucks")]
        public IActionResult GetTrucks()
        {
            List<string> result = new List<string>()
            {
                "Truck 1",
                "Truck 2"
            };

            return this.Ok(result);
        }

        #endregion
    }
}
