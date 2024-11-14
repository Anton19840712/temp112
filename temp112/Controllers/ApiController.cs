using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace temp112.Controllers
{

    [Route("/api")]
    public class ApiController : ControllerBase
    {

        public IActionResult Index()
        {
            return Ok();
        }


        [HttpPost]
        [Route("message")]
        public async Task<IActionResult> Message()
        {

            byte[] buf = new byte[1024];
            var requestBody = HttpContext.Request.Body.ReadAsync(buf);
            var requestBodyAsString = System.Text.Encoding.Default.GetString(buf);

            string payload = "<?xml version='1.0' encoding='utf-8'?>\r\n<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\"><soapenv:Body>\r\n<card112ChangedResponse xmlns=\"http://www.protei.ru/emergency/integration\">\r\n<errorCode>0</errorCode>\r\n<errorMessage></errorMessage>\r\n</card112ChangedResponse>\r\n</soapenv:Body></soapenv:Envelope>";

            //HttpContext.Response.Headers.Add("Vary", "Accept-Encoding");
            //HttpContext.Response.Headers.Add("Connection", "Keep-Alive");
            //HttpContext.Response.ContentType = "text/xml; charset=utf-8";

            //(xml to json)requestBodyAsString send to RabbitMQ



            await HttpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(payload));

            return Ok();
        }
    }
}
