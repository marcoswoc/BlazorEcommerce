using BlazorEcommerce.Shared;
using System.Net.Http.Json;

namespace BlazorEcommerce.Client.Services.ProductService;
public class ProductService : IProductService
{
    private readonly HttpClient _http;
    private const string _api = "api/Product/";

    public event Action ProductsChanged;

    public ProductService(HttpClient http)
    {
        _http = http;
    }

    public List<Product> Products { get; set; } = new();
    public string Message { get; set; } = "Loading products...";

    public async Task<ServiceResponse<Product>> GetProduct(int productId)
    {
        var result = await _http.GetFromJsonAsync<ServiceResponse<Product>>($"{_api}{productId}");
        return result;
    }

    public async Task GetProducts(string? categoryUrl = null)
    {
        var requestUrl = categoryUrl is null ? $"{_api}featured" : $"{_api}category/{categoryUrl}";

        var result = await _http.GetFromJsonAsync<ServiceResponse<List<Product>>>(requestUrl);

        if (result is not null && result.Data is not null)
            Products = result.Data;

        ProductsChanged.Invoke();
    }

    public async Task SearchProducts(string searchText)
    {
        var result = await _http.GetFromJsonAsync<ServiceResponse<List<Product>>>($"{_api}search/{searchText}");

        if (result is not null && result.Data is not null)
            Products = result.Data;

        if (Products.Count == 0) Message = "No Product found.";

        ProductsChanged?.Invoke();
    }

    public async Task<List<string>> GetProductSearchSuggestions(string searchText)
    {
        var result = await _http.GetFromJsonAsync<ServiceResponse<List<string>>>($"{_api}searchsuggestions/{searchText}");

        return result.Data;
    }
}

