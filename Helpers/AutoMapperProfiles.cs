using AutoMapper;
using StockTracker.DTO.PriceLists;
using StockTracker.DTO.Product;
using StockTracker.DTO.Sale;
using StockTracker.DTO.Stock;
using StockTracker.Models;

namespace StockTracker.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles() 
        {
            // Product
            CreateMap<Product, GetProductDTO>()
                .ReverseMap();
            CreateMap<PostProductDTO, Product>();
            CreateMap<PutProductDTO, Product>();
            CreateMap<ImportProductFromExcelDTO, Product>();

            // Category
            CreateMap<Category, SimpleCategoryDTO>()
                .ReverseMap();

            // Stock
            CreateMap<Stock, GetStockDTO>()
                .ReverseMap();
            CreateMap<Stock, SimpleStockDTO>()
                .ReverseMap();
            CreateMap<PostStockDTO, Stock>();

            // Sale
            CreateMap<Sale, GetSalesDTO>()
                .ReverseMap();
            CreateMap<PostSaleDTO, Sale>();

            // PriceList
            CreateMap<PriceList, GetPriceListDTO>()
                .ReverseMap();
            CreateMap<PostPriceListDTO, PriceList>();
            CreateMap<PutPriceListDTO, PriceList>();
            CreateMap<ImportPriceListFromExcelDTO, PriceList>();

            // Product Code
            CreateMap<ProductCode, SimpleCodeDTO>()
                .ReverseMap();
        }
    }
}
