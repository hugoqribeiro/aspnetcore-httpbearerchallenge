using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ragnar.RestApi.Controllers
{
    /// <summary>
    /// Provides actions on cars.
    /// </summary>
    /// <seealso cref="Controller" />
    public partial class CarsController : Controller
    {
        #region Public Methods

        /// <summary>
        /// Retrieves all the cars.
        /// </summary>
        /// <returns>
        /// The action result.
        /// </returns>
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
