using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockTracker.Database;
using StockTracker.DTO;
using StockTracker.DTO.Stock;
using StockTracker.Helpers;
using StockTracker.Models;
using StockTracker.Services;

namespace StockTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StocksController(DatabaseConnection context,
        IMapper mapper, IUriService uriService) : ControllerBase
    {
        private readonly DatabaseConnection _context = context;
        private readonly IMapper mapper = mapper;
        private readonly IUriService uriService = uriService;

        // GET: api/Stocks
        [HttpGet]
        public async Task<IActionResult> GetStocks([FromQuery] PaginationDTO pagination)
        {
            var route = Request.Path.Value;
            var pagedData = await _context.Stocks
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();
            var mappedData = mapper.Map<List<GetStockDTO>>(pagedData);
            var totalRecords = await _context.Stocks.CountAsync();
            var pagedResponse = PaginationHelper.CreatePagedResponse(mappedData, pagination, totalRecords, uriService, route);
            return Ok(pagedResponse);
        }

        // GET: api/Stocks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GetStockDTO>> GetStock(int id)
        {
            var stock = await _context.Stocks.FindAsync(id);
            if (stock == null) return NotFound();

            return mapper.Map<GetStockDTO>(stock);
        }

        [HttpGet("byProduct/{id}")]
        public async Task<ActionResult<GetStockDTO>> GetStockByProduct(int id)
        {
            var stock = await _context.Stocks.FirstOrDefaultAsync(x => x.ProductId == id);
            if (stock == null) return NotFound();
            return mapper.Map<GetStockDTO>(stock);
        }

        // PUT: api/Stocks/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStock(int id, PostStockDTO stock)
        {
            var stockDb = await _context.Stocks
                .FirstOrDefaultAsync(x => x.Id == id);
            if (stockDb == null) return NotFound();

            stockDb = mapper.Map(stock, stockDb);

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // POST: api/Stocks
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> PostStock(PostStockDTO stock)
        {
            var entity = mapper.Map<Stock>(stock);
            _context.Stocks.Add(entity);
            await _context.SaveChangesAsync();

            var dto = mapper.Map<GetStockDTO>(entity);

            return CreatedAtAction(nameof(GetStock), new { id = dto.Id }, dto);
        }

        // DELETE: api/Stocks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStock(int id)
        {
            var stock = await _context.Stocks.FindAsync(id);
            if (stock == null)
            {
                return NotFound();
            }

            _context.Stocks.Remove(stock);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool StockExists(int id)
        {
            return _context.Stocks.Any(e => e.Id == id);
        }
    }
}
