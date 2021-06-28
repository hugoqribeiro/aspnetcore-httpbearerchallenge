using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ragnar.RestApi.Controllers
{
    public partial class CarsController : Controller
    {
        #region Public Methods

        [HttpGet("/cars", Name = "CarsController.GetCars")]
        [Authorize("read-cars")]
        public IActionResult GetCars()
        {
            List<string> result = new List<string>()
            {
                "Car 1",
                "Car 2",
                "Car 3"
            };

            return this.Ok(result);
        }

        #endregion
    }
}
