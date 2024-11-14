using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using OrderService.Domain.Dtos;

namespace OrderService.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("AllowAllOrigin")]
    public class OrderController : ControllerBase
    {
        private readonly Application.Services.OrderService _orderHandler;

        public OrderController(Application.Services.OrderService orderHandler)
        {
            _orderHandler = orderHandler;
        }

        [HttpPost]
        public async Task<OrderCreatedResponse> CreateOrder(OrderDto orderDto)
        {
            OrderCreatedResponse response = await _orderHandler.CreateOrderAsync(orderDto);

            return response;
        }
    }
}