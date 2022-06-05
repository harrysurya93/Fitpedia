using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Fitpedia.Controllers
{
    public class IoT_WeightController : ApiController
    {

        // POST api/IoT_Weight
        [HttpPost]
        public HttpResponseMessage Post([FromBody] Models.Structure.IoT_Message oIoT_Message)
        {
            try
            {
                string Result = Models.Connection.ExecuteEditQuery(String.Format("INSERT INTO iot_weight (createdate, deviceid, value) VALUES ( NOW(), '{0}', {1})", oIoT_Message.DeviceId, oIoT_Message.DeviceValue));
                if (Result.Length > 0)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, Result);
                }
                else
                {
                    var message = Request.CreateResponse(HttpStatusCode.Created, oIoT_Message);
                    message.Headers.Location = new Uri(Request.RequestUri + oIoT_Message.DeviceId + oIoT_Message.DeviceValue);
                    return message;
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

    }
}