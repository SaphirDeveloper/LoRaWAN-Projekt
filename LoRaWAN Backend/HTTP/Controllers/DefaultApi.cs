/*
 * LoRaWAN
 *
 * No description provided (generated by Swagger Codegen https://github.com/swagger-api/swagger-codegen)
 *
 * OpenAPI spec version: v1
 * 
 * Generated by: https://github.com/swagger-api/swagger-codegen.git
 */
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using LoRaWAN.HTTP.Attributes;
using LoRaWAN.BackendPackets;
using Newtonsoft.Json;

namespace LoRaWAN.HTTP.Controllers
{
    [ApiController]
    public class DefaultApiController : ControllerBase
    {
        private readonly Server _server;

        /// <summary>
        /// Default
        /// </summary>
        /// <param name="backendPacket"></param>
        /// <response code="200">Packet accepted</response>
        [HttpPost]
        [Route("/")]
        [ValidateModelState]
        [Consumes(typeof(BackendPacket), "application/json")]
        public virtual IActionResult RootPost([FromBody] string json)
        {
            Logger.LogWrite(json, _server.GetType().Name);
            Console.WriteLine($"\nJSON received:\n{json}");

            try
            {
                BackendPacket packet = JsonConvert.DeserializeObject<BackendPacket>(json);
                _server.ProcessPacket(packet);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return StatusCode(500);
            }
            
            return StatusCode(200);
        }

        [HttpGet]
        [Route("/status")]
        public virtual string GetServerStatus()
        {
            return _server.GetStatus();
        }

        public DefaultApiController(Server server)
        {
            _server = server;
        }
    }
}
