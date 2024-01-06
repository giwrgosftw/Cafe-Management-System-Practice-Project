using Cafe_management_system_backend.MVC.Services.Facades;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Cafe_management_system_backend.MVC.Controllers
{
    [RoutePrefix("api/dashboard")]
    public class DashboardController : ApiController
    {
        private readonly DashboardFacadeService dashboardFacadeService;

        public DashboardController(DashboardFacadeService dashboardFacadeService)
        {
            this.dashboardFacadeService = dashboardFacadeService;
        }

        /// <summary>Retrieves dashboard details, including counts for categories, products, bills, and users.</summary>
        /// <returns>HTTP response with dashboard details.</returns>
        [HttpGet, Route("getDashboardDetails")]
        public HttpResponseMessage GetDashboardDetails()
        {
            try
            {
                var data = dashboardFacadeService.getDashboardCounts();
                return Request.CreateResponse(HttpStatusCode.OK, data);
            }
            catch 
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { message = "Internal Server Error" });
            }
        }

    }
}
