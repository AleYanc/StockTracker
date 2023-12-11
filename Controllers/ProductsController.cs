using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using StockTracker.Database;
using StockTracker.DTO;
using StockTracker.DTO.Product;
using StockTracker.Helpers;
using StockTracker.Models;
using StockTracker.Services;
using StockTracker.Wrappers;

namespace StockTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(DatabaseConnection context, 
        IMapper mapper, IFileSaver fileSaver, IUriService uriService, IWebHostEnvironment webHostEnvironment) : ControllerBase
    {
        private readonly DatabaseConnection _context = context;
        private readonly IMapper mapper = mapper;
        private readonly IFileSaver fileSaver = fileSaver;
        private readonly IUriService uriService = uriService;
        private readonly IWebHostEnvironment _webHostEnvironment = webHostEnvironment;
        private readonly string container = "products";

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<PagedResponse<List<GetProductDTO>>>> GetProducts([FromQuery] PaginationDTO pagination)
        {
            string route = Request.Path.Value;
            List<Product> pagedData = await _context.Products
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();
            List<GetProductDTO> mappedData = mapper.Map<List<GetProductDTO>>(pagedData);
            int totalRecords = await _context.Products.CountAsync();
            PagedResponse<List<GetProductDTO>> pagedResponse = PaginationHelper.CreatePagedResponse(mappedData, pagination, totalRecords, uriService, route);
            return Ok(pagedResponse);
        }

        // GET: api/Products/5
        [HttpGet("{id}", Name = "GetProduct")]
        public async Task<ActionResult<GetProductDTO>> GetProduct(int id)
        {
            Product product = await _context.Products
                .Include(x => x.Stock)
                .Include(x => x.Category)
                .Include(x => x.ProductCode)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (product == null) return NotFound();

            return mapper.Map<GetProductDTO>(product);
        }

        [HttpGet("{id}/images", Name = "GetProductImages")]
        public async Task<ActionResult<List<string>>> GetProductImages(int id)
        {
            Product product = await _context.Products.FindAsync(id);
            List<string> images = JsonConvert.DeserializeObject<List<string>>(product.ImagesUrl);
            return Ok(images);
        }

        [HttpDelete("{id}/images/delete", Name = "DeleteProductImage")]
        public async Task<IActionResult> DeleteImageFromProduct([FromForm] string imageRoute, int id)
        {
            Product product = await _context.Products.FindAsync(id);

            if (product.ImagesUrl == "[]") return BadRequest("The current product does not have images!");

            List<string> images = JsonConvert.DeserializeObject<List<string>>(product.ImagesUrl);

            if (!images.Contains(imageRoute)) return BadRequest("Image route does not exist!");

            images.Remove(imageRoute);

            await fileSaver.DeleteFile(container, imageRoute);

            product.ImagesUrl = JsonConvert.SerializeObject(images);
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Image deleted from product!" });
        }

        [HttpPost("{id}/images/add", Name = "AddProductImage")]
        public async Task<IActionResult> AddImageToProduct(int id, [FromForm] AddProductImageDTO productImage)
        {
            Product product = await _context.Products.FindAsync(id);
            List<string> images = product.ImagesUrl == "[]" || product.ImagesUrl == "" ? new List<string>() : JsonConvert.DeserializeObject<List<string>>(product.ImagesUrl);

            if(productImage.Image != null) 
            {
                using(MemoryStream memoryStream = new())
                {
                    await productImage.Image.CopyToAsync(memoryStream);
                    byte[] content = memoryStream.ToArray();
                    string extension = Path.GetExtension(productImage.Image.FileName);
                    string url = await fileSaver.SaveFile(content, extension, container, productImage.Image.ContentType);
                    images.Add(url);
                }

                product.ImagesUrl = JsonConvert.SerializeObject(images);
                await _context.SaveChangesAsync();
                return Ok(new { Message = "Image added to product!" });
            }

            return BadRequest("Please select an image to upload!");
        }

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> PostProduct([FromForm] PostProductDTO product)
        {
            Product entity = mapper.Map<Product>(product);
            List<string> imageUrls = new List<string>();

            if(product.Images != null)
            {
                using(MemoryStream memoryStream = new())
                {
                    foreach (var image in product.Images)
                    {
                        await image.CopyToAsync(memoryStream);
                        byte[] content = memoryStream.ToArray();
                        string extension = Path.GetExtension(image.FileName);
                        string url = await fileSaver.SaveFile(content, extension, container, image.ContentType);
                        imageUrls.Add(url);
                    }
                }
            }

            entity.ImagesUrl = JsonConvert.SerializeObject(imageUrls);

            _context.Products.Add(entity);
            await _context.SaveChangesAsync();

            var dto = mapper.Map<GetProductDTO>(entity);

            ProductCodeHelper productCodeHelper = new();
            string code = await productCodeHelper.BuildCode(_context);

            ProductCode productCode = new ProductCode()
            {
                ProductId = dto.Id,
                Code = code,
            };

            _context.ProductCodes.Add(productCode);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduct), new { id = dto.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> PutProduct(int id, [FromForm] PutProductDTO product) 
        {
            Product dbEntity = await _context.Products.FindAsync(id);

            if (dbEntity == null) return NotFound();
            _ = mapper.Map(product, dbEntity);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Excel import
        [HttpPost("import")]
        public async Task<IActionResult> ImportFromExcelFile([FromForm] ImportFromExcelDTO excel)
        {
            if (excel.ExcelFile == null) return BadRequest("Must provide an excel file");

            //string filePath = SaveFile(excel.ExcelFile);
            string filePath = await fileSaver.LocalFileSave(excel.ExcelFile, _webHostEnvironment);

            List<ImportProductFromExcelDTO> importedProducts = ExcelImportHelper.Import<ImportProductFromExcelDTO>(filePath);
            List<Product> products = [];
            int productCount = importedProducts.Count;

            foreach(ImportProductFromExcelDTO product in importedProducts)
            {
                Product entity = mapper.Map<Product>(product);
                await _context.Products.AddAsync(entity);
                await _context.SaveChangesAsync();
                products.Add(entity);
            }

            ProductCodeHelper productCodeHelper = new();

            foreach(Product product in products)
            {

                ProductCode productCode = new ProductCode()
                {
                    ProductId = product.Id,
                    Code = await productCodeHelper.BuildCode(_context),
                };
                _context.Add(productCode);
                await _context.SaveChangesAsync();
            }

            await _context.ImportHistories
                .AddAsync(ExcelImportHelper.BuildNewHistoryRegister(filePath, excel.ExcelFile.FileName, "Product import"));

            return Ok(new { Message = $"{productCount} products imported from excel!" });
        }

        // TO DO: PATCH ACTION

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            Product product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private string _selectQuery = "SELECT * FROM [" + "Hoja1" + "$]";

        // save the uploaded file into wwwroot/uploads folder
        private string SaveFile(IFormFile file)
        {
            if (file.Length == 0)
            {
                throw new BadHttpRequestException("File is empty.");
            }

            string extension = Path.GetExtension(file.FileName);

            string webRootPath = _webHostEnvironment.WebRootPath;
            if (string.IsNullOrWhiteSpace(webRootPath))
            {
                webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            }

            string folderPath = Path.Combine(webRootPath, "uploads");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string fileName = $"{Guid.NewGuid()}.{extension}";
            string filePath = Path.Combine(folderPath, fileName);
            using FileStream stream = new FileStream(filePath, FileMode.Create);
            file.CopyTo(stream);

            return filePath;
        }
    }
}
