using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockService.Domain.Entities;
using StockService.Infrastructure.Data;

namespace StockService.Controller
{
    // Controllers/ProductController.cs
    [ApiController]
    [Route("api/[controller]")]
    public class StockController : ControllerBase
    {
        private readonly StockDBContext _context;

        public StockController(StockDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetStockMovements()
        {
            var stockMovements = await _context.StockMovements.ToListAsync();
            return Ok(stockMovements);
        }

        [HttpPost]
        public async Task<IActionResult> CreateStockMovement(StockMovement stockMovement)
        {
            _context.StockMovements.Add(stockMovement);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }

}