using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using StockTracker.Database;
using StockTracker.DTO;
using StockTracker.DTO.Sale;
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
    public class SalesController(DatabaseConnection context,
        IMapper mapper, IUriService uriService) : ControllerBase
    {
        private readonly DatabaseConnection _context = context;
        private readonly IMapper mapper = mapper;
        private readonly IUriService uriService = uriService;

        // GET: api/Sales
        [HttpGet]
        public async Task<ActionResult<PagedResponse<List<GetSalesDTO>>>> GetSales([FromQuery] PaginationDTO pagination)
        {
            string route = Request.Path.Value;
            List<Sale> pagedData = await _context.Sales
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();
            List<GetSalesDTO> mappedData = mapper.Map<List<GetSalesDTO>>(pagedData);
            int totalRecords = await _context.Sales.CountAsync();
            PagedResponse<List<GetSalesDTO>> pagedResponnse = PaginationHelper.CreatePagedResponse(mappedData, pagination, totalRecords, uriService, route);

            return Ok(pagedResponnse);
        }

        [HttpGet("filtered")]
        public async Task<ActionResult<PagedResponse<List<GetSalesDTO>>>> FilteredSales([FromQuery] FilteredSaleParamsDTO filteredSaleParams, [FromQuery] PaginationDTO pagination)
        {
            List<string> validScopes = new List<string> { "day", "month", "week", "year" };
            if(filteredSaleParams == null || !validScopes.Contains(filteredSaleParams.FilteredScope)) return BadRequest("Invalid scope. Please check valid ones: " + string.Join(", ", validScopes));

            string route = Request.Path.Value;
            List<Sale> pagedData = await FilterSales(pagination, filteredSaleParams);

            List<GetSalesDTO> mappedData = mapper.Map<List<GetSalesDTO>>(pagedData);
            int totalRecords = await _context.Sales.CountAsync();
            PagedResponse<List<GetSalesDTO>> pagedResponnse = PaginationHelper.CreatePagedResponse(mappedData, pagination, totalRecords, uriService, route);

            return Ok(pagedResponnse);
        }

        // GET: api/Sales/5
        [HttpGet("{id}", Name = "GetSale")]
        public async Task<ActionResult<GetSalesDTO>> GetSale(int id)
        {
            Sale sale = await _context.Sales.FindAsync(id);

            if (sale == null) return NotFound();
            
            return mapper.Map<GetSalesDTO>(sale); ;
        }

        // PUT: api/Sales/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSale(int id, Sale sale)
        {
            if (id != sale.Id)
            {
                return BadRequest();
            }

            _context.Entry(sale).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SaleExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Sales
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> PostSale([FromForm] PostSaleDTO sale)
        {
            Sale entity = mapper.Map<Sale>(sale);
            List<int> products = JsonConvert.DeserializeObject<List<int>>(sale.Products);

            // Check if products from list exists on DB
            List<int> invalidProducts = await ProductExists(products);

            if(invalidProducts.Count != 0) return BadRequest(new { Message = "There are invalid products", invalidProducts });

            entity.SaleDate = DateTime.Now; // ADD OPTION TO SELECT DATE LATER

            _context.Sales.Add(entity);
            await _context.SaveChangesAsync();

            GetSalesDTO dto = mapper.Map<GetSalesDTO>(entity);

            return CreatedAtRoute(nameof(GetSale), new { id = dto.Id }, dto);
        }

        // DELETE: api/Sales/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSale(int id)
        {
            Sale sale = await _context.Sales.FindAsync(id);
            if (sale == null)
            {
                return NotFound();
            }

            _context.Sales.Remove(sale);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SaleExists(int id)
        {
            return _context.Sales.Any(e => e.Id == id);
        }

        private async Task<List<Sale>> FilterSales(PaginationDTO pagination, FilteredSaleParamsDTO filteredSaleParams)
        {
            List<Sale> filteredProduct = await _context.Sales
                .Where(x =>
                filteredSaleParams.FilteredScope == "day"
                    ? x.SaleDate >= DateTime.Now.AddDays(-filteredSaleParams.FilterValue).Date
                : filteredSaleParams.FilteredScope == "week"
                    ? x.SaleDate >= DateTime.Now.AddDays(-(7 * filteredSaleParams.FilterValue)).Date
                : filteredSaleParams.FilteredScope == "month"
                    ? x.SaleDate >= DateTime.Now.AddMonths(-filteredSaleParams.FilterValue).Date
                : x.SaleDate >= x.SaleDate.AddYears(-filteredSaleParams.FilterValue)
                )
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();
            return filteredProduct;
        }

        private async Task<List<int>> ProductExists(List<int> productIds)
        {
            List<int> products = await _context.Products
                .Where(x => productIds.Contains(x.Id))
                .Select(x => x.Id)
                .ToListAsync();

            List<int> invalidIds = new List<int>();

            if (products.Count != productIds.Count)
            {
                invalidIds = productIds.Except(products).ToList();
                return invalidIds;
            } else
            {
                return invalidIds;
            }
        }
    }
}
