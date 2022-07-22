using MassTransit;
using Microsoft.AspNetCore.Mvc;
using mtstatemachine.Consumers;

namespace mtstatemachine.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrashController:ControllerBase
    {
        private readonly IPublishEndpoint publishEndpoint;

        public TrashController(IPublishEndpoint publishEndpoint)
        {
            this.publishEndpoint = publishEndpoint;
        }

        [HttpPost]
        public async Task<IActionResult> Post(string binNumber)
        {
            await this.publishEndpoint.Publish<TrashBin>(new
            {
                BinNumber = binNumber
            });
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Get(int duration)
        {
            await this.publishEndpoint.Publish<Doit>(new { 
                Duration=TimeSpan.FromSeconds(duration)
            });
            return Ok();
        }
    }
}
