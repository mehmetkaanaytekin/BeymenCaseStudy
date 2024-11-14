using OrderService.Domain.Entities;
using Newtonsoft.Json;
using OrderService.Application.Interfaces;
using OrderService.Infrastructure.Services;
using OrderService.Domain.Dtos;

namespace OrderService.Application.Services
{
    public class OrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly OrderMQService _orderMQService;

        public OrderService(IOrderRepository orderRepository,
                            OrderMQService orderMQService)
        {
            _orderRepository = orderRepository;
            _orderMQService = orderMQService;
        }

        public async Task<OrderCreatedResponse> CreateOrderAsync(OrderDto orderDto)
        {
            Order order = new()
            {
                OrderItems = orderDto.OrderItems.Select(x => new OrderItem
                {
                    UnitPrice = x.UnitPrice,
                    Barcode = x.Barcode,
                    LineNumber = x.LineNumber,
                    ProductName = x.ProductName,
                    ProductId = x.ProductId,
                    Quantity = x.Quantity
                }).ToList(),
                Address = orderDto.Address,
                CustomerEmail = orderDto.CustomerEmail,
                CustomerId = orderDto.CustomerId,
                CustomerPhone = orderDto.CustomerPhone,
                PaymentMethod = orderDto.PaymentMethod,
                OrderNumber = "ORD" + Guid.NewGuid().ToString(),
                OrderStatus = Domain.Enum.OrderStatus.Pending,
                TotalAmount = orderDto.OrderItems.Sum(x => x.UnitPrice * x.Quantity)
            };

            Guid orderId = await _orderRepository.CreateOrderAsync(order);

            var orderMessage = new OrderMessage
            {
                Order = order,
                State = "OrderCreated"
            };

            await _orderMQService.SendMessageAsync(JsonConvert.SerializeObject(orderMessage));

            return new OrderCreatedResponse
            {
                OrderId = orderId.ToString(),
                OrderReceived = true,
                OrderStatus = order.OrderStatus
            };
        }
    }
}