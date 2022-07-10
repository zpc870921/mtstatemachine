using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using mtstatemachine.Consumers;
using mtstatemachine.StateMachines;

namespace mtstatemachine.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IPublishEndpoint publishEndpoint;

        public CustomerController(IPublishEndpoint publishEndpoint)
        {
            this.publishEndpoint = publishEndpoint;
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(Guid id,string customerNumber)
        {
            await this.publishEndpoint.Publish<CustomerAccountClosed>(new
            {
                CustomerId = id,
                CustomerNumber = customerNumber
            });


            return Ok();
        }

        [HttpGet(nameof(demo))]
        public async Task<IActionResult> demo([FromServices]IRequestClient<AllocationStatusRequest> requestClient,Guid id)
        {
            var response= await requestClient.GetResponse<AllocationStatusResponse>(new
            {
                AllocationId = id
            });
            return Ok(response.Message);
        }

        [HttpPatch]
        public async Task<IActionResult> Accepted(Guid id)
        {
            await this.publishEndpoint.Publish<OrderAccepted>(new
            {
                OrderId = id,
                Timestamp = InVar.Timestamp
            });
            return Ok();
        }
    }
}
