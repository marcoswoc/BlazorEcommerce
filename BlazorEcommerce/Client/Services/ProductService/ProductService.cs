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
    public int CurrentPage { get; set; } = 1;
    public int PageCount { get; set; } = 0;
    public string LastSearchText { get; set; } = string.Empty;

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

        CurrentPage = 1;
        PageCount = 0;

        if (Products.Count == 0) Message = "No Product found.";

        ProductsChanged.Invoke();
    }

    public async Task SearchProducts(string searchText, int page)
    {
        LastSearchText = searchText;

        var result = await _http.GetFromJsonAsync<ServiceResponse<ProductSearchResult>>($"{_api}search/{searchText}/{page}");

        if (result is not null && result.Data is not null)
        {
            Products = result.Data.Products;
            CurrentPage = result.Data.CurrentPage;
            PageCount = result.Data.Pages;
        }

        if (Products.Count == 0) Message = "No Product found.";

        ProductsChanged?.Invoke();
    }

    public async Task<List<string>> GetProductSearchSuggestions(string searchText)
    {
        var result = await _http.GetFromJsonAsync<ServiceResponse<List<string>>>($"{_api}searchsuggestions/{searchText}");

        return result.Data;
    }
}

