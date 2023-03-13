using Mango.Web.Models;
using Mango.Web.Services.IServices;

namespace Mango.Web.Services
{
    public class ProductService : BaseService, IProductService
    {
        private readonly IHttpClientFactory _clientFactory;

        public ProductService(IHttpClientFactory clientFactory) : base(clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<T> CreateProductAsync<T>(ProductDto productDto, string token)
        {
            return await SendAsync<T>(new ApiRequest()
            {
                ApiType = Mango.Web.SD.ApiType.POST,
                Data = productDto,
                Url = Mango.Web.SD.ProductAPIBase + "/api/products",
                AccessToken = token
            });
        }

        public async Task<T> DeleteProductAsync<T>(int id, string token)
        {
            return await SendAsync<T>(new ApiRequest()
            {
                ApiType = Mango.Web.SD.ApiType.DELETE,
                Url = Mango.Web.SD.ProductAPIBase + "/api/products/" + id,
                AccessToken = token
            });
        }

        public async Task<T> GetAllProductsAsync<T>(string token)
        {
            return await SendAsync<T>(new ApiRequest()
            {
                ApiType = Mango.Web.SD.ApiType.GET,
                Url = Mango.Web.SD.ProductAPIBase + "/api/products/",
                AccessToken = token
            });
        }

        public async Task<T> GetProductByIdAsync<T>(int id, string token)
        {
            return await SendAsync<T>(new ApiRequest()
            {
                ApiType = Mango.Web.SD.ApiType.GET,
                Url = Mango.Web.SD.ProductAPIBase + "/api/products/" + id,
                AccessToken = token
            });
        }

        public async Task<T> UpdateProductAsync<T>(ProductDto productDto, string token)
        {
            return await SendAsync<T>(new ApiRequest()
            {
                ApiType = Mango.Web.SD.ApiType.PUT,
                Data = productDto,
                Url = Mango.Web.SD.ProductAPIBase + "/api/products",
                AccessToken = token
            });
        }
    }
}