using Cafe_management_system_backend.MVC.Models;
using Cafe_management_system_backend.MVC.Models.Dtos;
using Cafe_management_system_backend.MVC.Security;
using Cafe_management_system_backend.MVC.Services;
using Cafe_management_system_backend.MVC.Services.Facades;
using Cafe_management_system_backend.MVC.Services.UserServices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;

namespace Cafe_management_system_backend.MVC.Controllers
{
    [RoutePrefix("api/bill")]
    public class BillController : ApiController
    {
        private readonly UserAuthorityService userAuthorityService;
        private readonly BillProductFacadeService billProductFacadeService;
        private readonly BillService billService;

        public BillController(UserAuthorityService userAuthorityService, BillProductFacadeService billProductFacadeService, BillService billService)
        {
            this.userAuthorityService = userAuthorityService;
            this.billProductFacadeService = billProductFacadeService;
            this.billService = billService;
        }

        /// <summary>
        /// Retrieves a list of bills based on the user's role. If the user is an admin, it returns all bills; 
        /// otherwise, it returns only the bills created by the logged-in user.
        /// </summary>
        /// <returns> A list of bills. </returns>
        [HttpGet, Route("getAllBills")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage GetAllBills()
        {
            try
            {
                // Retrieve the authorization token from the request headers & Get Principal info
                var token = Request.Headers.GetValues("authorization").First();
                PrincipalProfile principalProfile = TokenManager.GetPrincipalProfileInfo(token);
                // Retrieve bills based on Role
                List<Bill> result;
                if (principalProfile.Role == UserRoleEnum.Admin.ToString()) {
                    result = billService.FindAllBills(); // For Admins allow to return all of them
                }
                else {
                    result = billService.FindAllBillsByCreatedBy(principalProfile.Email); // Only those which the logged-in user created

                }
                // Bi-directional loop fix: Configure JSON serialization settings to ignore circular references
                var jsonSettings = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
                // Return the response with the configured settings
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(JsonConvert.SerializeObject(result, jsonSettings), Encoding.UTF8, "application/json");
                return response;
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { message = "Internal Server Error" });
            }
        }


        /// <summary> Generates a bill report in PDF format based on the provided data and user authorization. </summary>
        /// <param name="generateBillReportDto">Data needed to generate the bill report.</param>
        /// <returns>HTTP response containing the generated PDF file for download.</returns>
        [HttpPost, Route("generateBillReportPdf")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage GenerateBillReportPdf([FromBody] GenerateBillReportDto generateBillReportDto)
        {
            try
            {
                // Retrieve the authorization token from the request headers
                var token = Request.Headers.GetValues("authorization").First();
                // Get Principal info
                PrincipalProfile principalProfile = TokenManager.GetPrincipalProfileInfo(token);
                // Generate the report
                (Bill, byte[]) pdfTuple = billProductFacadeService.GenerateBillReport(principalProfile, generateBillReportDto);
                // Get Successful Response
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
                // Set response content directly with the generated PDF bytes
                response.Content = new ByteArrayContent(pdfTuple.Item2); // The byte array containing the PDF content is set as the response content. This is what the user will download.
                response.Content.Headers.ContentLength = pdfTuple.Item2.LongLength; // The content length is set based on the length of the PDF byte array.
                // Set the file name for download
                var todayDate = DateTime.Now.ToString("ddMMyyy");
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                response.Content.Headers.ContentDisposition.FileName = todayDate + "_" + pdfTuple.Item1.uuid + ".pdf";
                // Set the content type - indicate that the response contains a PDF file
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                // Return file for view and download
                return response;
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { message = "Internal Server Error" });
            }
        }

        /// <summary> Deletes a bill and associated BillProducts from the system by its UUID. </summary>
        /// <param name="billUuid"> The UUID of the bill to be deleted. </param>
        /// <returns> A response message indicating the success or failure of the operation. </returns>
        [HttpPost, Route("deleteBill")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage DeleteBill([FromUri] string billUuid)
        {
            try
            {
                // Retrieve the authorization token from the request headers
                var token = Request.Headers.GetValues("authorization").First();
                // Check if the user has the "Admin" authority based on the token
                if (!userAuthorityService.HasAuthorityAdmin(token)) { return Request.CreateResponse(HttpStatusCode.Unauthorized); }
                // Delete
                billProductFacadeService.DeleteBillProductAndBill(billUuid);
                // Return Successful Response
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Bill Deleted Successfully!" });
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { message = "Internal Server Error" });
            }
        }
    }
}
