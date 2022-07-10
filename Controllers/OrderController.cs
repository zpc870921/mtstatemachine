using MassTransit;
using Microsoft.AspNetCore.Mvc;
using mtstatemachine.Consumers;

namespace mtstatemachine.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<OrderController> _logger;
        private readonly IRequestClient<CheckOrderRequest> checkOrderRequest;
        private readonly IRequestClient<SubmitOrder> submitOrderRequest;

        public OrderController(ILogger<OrderController> logger,IRequestClient<CheckOrderRequest> checkOrderRequest
            ,IRequestClient<SubmitOrder> submitOrderRequest)
        {
            _logger = logger;
            this.checkOrderRequest = checkOrderRequest;
            this.submitOrderRequest = submitOrderRequest;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IActionResult> Get(Guid orderId)
        {
            var (accepted,notfound) = await this.checkOrderRequest.GetResponse<CheckOrderStatus, OrderNotFound>(new
            {
                OrderId = orderId
            });

            if(accepted.IsCompletedSuccessfully)
            {
                var response = await accepted;
                return Ok(response.Message);
            }
            else
            {
                var response = await notfound;
                return Ok(response.Message);
            }
        }


        [HttpPost]
        public async Task<IActionResult> Post(Guid orderid,string customernumber)
        {
            var response = await this.submitOrderRequest.GetResponse<SubmitOrderAccepted>(new
            {
                OrderId = orderid,
                Timestamp = InVar.Timestamp,
                CustomerNumber = customernumber
            });
            return Ok(response.Message);
        }
    }
}