using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockTracker.Database;
using StockTracker.DTO;
using StockTracker.DTO.PriceLists;
using StockTracker.DTO.Product;
using StockTracker.Helpers;
using StockTracker.Models;
using StockTracker.Services;
using StockTracker.Wrappers;

namespace StockTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PriceListsController(DatabaseConnection context, IMapper mapper,
        IUriService uriService, IFileSaver fileSaver, IWebHostEnvironment webHostEnvironment) : ControllerBase
    {
        private readonly DatabaseConnection _context = context;
        private readonly IMapper mapper = mapper;
        private readonly IUriService uriService = uriService;
        private readonly IFileSaver fileSaver = fileSaver;
        private readonly IWebHostEnvironment webHostEnvironment = webHostEnvironment;

        // GET: api/PriceLists
        [HttpGet]
        public async Task<ActionResult<PagedResponse<List<GetPriceListDTO>>>> GetPriceLists([FromQuery] PaginationDTO pagination)
        {
            string route = Request.Path.Value;
            List<PriceList> pagedData = await _context.PriceLists
                .Include(p => p.Product)
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();
            List<GetPriceListDTO> mappedData = mapper.Map<List<GetPriceListDTO>>(pagedData);
            int totalRecords = await _context.PriceLists.CountAsync();
            PagedResponse<List<GetPriceListDTO>> pagedResponse = PaginationHelper.CreatePagedResponse(mappedData, pagination, totalRecords, uriService, route);
            return Ok(pagedResponse);
        }

        // GET: api/PriceLists/5
        [HttpGet("{id}", Name = "GetPriceList")]
        public async Task<ActionResult<GetPriceListDTO>> GetPriceList(int id)
        {
            PriceList priceList = await _context.PriceLists
                .Include(p => p.Product)
                .FirstOrDefaultAsync(x => x.Id == id);

            if(priceList == null) return NotFound();
            return mapper.Map<GetPriceListDTO>(priceList);
        }

        // PUT: api/PriceLists/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPriceList(int id, [FromForm] PutPriceListDTO priceList)
        {
            PriceList dbEntity = await _context.PriceLists.FindAsync(id);

            if(dbEntity == null) return NotFound();

            _ = mapper.Map(priceList, dbEntity);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("UpdateAllPrices/{amount}")]
        public async Task<IActionResult> UpdateAllPrices(double amount, [FromQuery] string priceType)
        {
            int modifiedRows;

            if(amount < -100.0) return BadRequest(new GenericResponseMessageDTO { Message = "Can't decrease more than 100% of the total price", Error = true });

            if(amount > 0)
            {
                modifiedRows = await _context.PriceLists
                    .Where(x => x.PriceType == priceType)
                    .ExecuteUpdateAsync(x =>
                        x.SetProperty
                        (p => p.Price,
                            amount == 100.0 ? p => p.Price + p.Price
                            : p => p.Price + (p.Price * amount / 100.0))
                        );
            } else
            {
                modifiedRows = await _context.PriceLists
                    .Where(x => x.PriceType == priceType)
                    .ExecuteUpdateAsync(x =>
                         x.SetProperty
                         (p => p.Price,
                             -amount == 100.0 ? p => p.Price - p.Price
                             : p => p.Price - (p.Price * (-amount / 100.0))
                         ));
            }

            return Ok(new GenericResponseMessageDTO { Message = $"{modifiedRows} products changed price."});
        }

        // POST: api/PriceLists
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PriceList>> PostPriceList([FromForm] PostPriceListDTO priceList)
        {
            PriceList entity = mapper.Map<PriceList>(priceList);

            _context.Add(entity);
            await _context.SaveChangesAsync();

            GetPriceListDTO dto = mapper.Map<GetPriceListDTO>(entity);

            return CreatedAtAction("GetPriceList", new { id = dto.Id }, dto);
        }

        // Excel import
        [HttpPost("import")]
        public async Task<IActionResult> ImportFromExcelFile([FromForm] ImportFromExcelDTO excel)
        {
            if (excel.ExcelFile == null) return BadRequest("Must provide an excel file.");

            string filePath = await fileSaver.LocalFileSave(excel.ExcelFile, webHostEnvironment);

            List<ImportPriceListFromExcelDTO> importedPriceLists = ExcelImportHelper.Import<ImportPriceListFromExcelDTO>(filePath);
            List<PriceList> priceLists = new List<PriceList>();

            foreach(ImportPriceListFromExcelDTO priceList in importedPriceLists)
            {
                ProductCode product = await _context.ProductCodes
                    .FirstOrDefaultAsync(x => x.Code == priceList.ProductCode);

                if (product == null) return BadRequest($"Product with code {priceList.ProductCode} does not exist.");

                PriceList entity = mapper.Map<PriceList>(priceList);
                entity.ProductId = product.ProductId;
                priceLists.Add(entity);
            }

            foreach (PriceList priceList in priceLists)
            {
                await _context.PriceLists.AddAsync(priceList);
            }

            await _context.ImportHistories
                .AddAsync(ExcelImportHelper.BuildNewHistoryRegister(filePath, excel.ExcelFile.FileName, "PriceList import"));
            await _context.SaveChangesAsync();

            return Ok($"{priceLists.Count} price lists imported from excel!");
        }

        // DELETE: api/PriceLists/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePriceList(int id)
        {
            PriceList priceList = await _context.PriceLists.FindAsync(id);
            if (priceList == null)
            {
                return NotFound();
            }

            _context.PriceLists.Remove(priceList);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
