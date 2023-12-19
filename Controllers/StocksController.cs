using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockTracker.Database;
using StockTracker.DTO;
using StockTracker.DTO.Stock;
using StockTracker.Helpers;
using StockTracker.Models;
using StockTracker.Services;
using StockTracker.Wrappers;

namespace StockTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
            string route = Request.Path.Value;
            List<Stock> pagedData = await _context.Stocks
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();
            List<GetStockDTO> mappedData = mapper.Map<List<GetStockDTO>>(pagedData);
            int totalRecords = await _context.Stocks.CountAsync();
            PagedResponse<List<GetStockDTO>> pagedResponse = PaginationHelper.CreatePagedResponse(mappedData, pagination, totalRecords, uriService, route);
            return Ok(pagedResponse);
        }

        // GET: api/Stocks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GetStockDTO>> GetStock(int id)
        {
            Stock stock = await _context.Stocks.FindAsync(id);
            if (stock == null) return NotFound();

            return mapper.Map<GetStockDTO>(stock);
        }

        [HttpGet("byProduct/{id}")]
        public async Task<ActionResult<GetStockDTO>> GetStockByProduct(int id)
        {
            Stock stock = await _context.Stocks.FirstOrDefaultAsync(x => x.ProductId == id);
            if (stock == null) return NotFound();
            return mapper.Map<GetStockDTO>(stock);
        }

        // PUT: api/Stocks/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStock(int id, PostStockDTO stock)
        {
            Stock stockDb = await _context.Stocks
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
            Stock entity = mapper.Map<Stock>(stock);
            _context.Stocks.Add(entity);
            await _context.SaveChangesAsync();

            GetStockDTO dto = mapper.Map<GetStockDTO>(entity);

            return CreatedAtAction(nameof(GetStock), new { id = dto.Id }, dto);
        }

        // DELETE: api/Stocks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStock(int id)
        {
            Stock stock = await _context.Stocks.FindAsync(id);
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
