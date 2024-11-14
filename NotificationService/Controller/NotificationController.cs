using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotificationService.Application.Handlers;
using NotificationService.Infrastructure.Data;

namespace NotificationService.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly NotificationDBContext _context;
        private readonly NotificationHandler _notificationService;

        public NotificationController(NotificationDBContext context, NotificationHandler notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetNotifications()
        {
            var notifications = await _context.Notifications.ToListAsync();
            return Ok(notifications);
        }
    }
}