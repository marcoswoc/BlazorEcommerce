using BlazorEcommerce.Shared;
using Blazored.LocalStorage;
using System.Net.Http.Json;

namespace BlazorEcommerce.Client.Services.CartService;

public class CartService : ICartService
{
    private readonly ILocalStorageService _localStorageService;
    private readonly HttpClient _http;
    private const string _api = "api/cart/";

    public CartService(ILocalStorageService localStorageService, HttpClient http)
    {
        _localStorageService = localStorageService;
        _http = http;
    }

    public event Action OnChange;


    public async Task AddToCart(CartItem cartItem)
    {
        var cart = await _localStorageService.GetItemAsync<List<CartItem>>("cart");

        if (cart is null) cart = new List<CartItem>();

        var sameItem = cart.Find(x => x.ProductId == cartItem.ProductId && x.ProductTypeId == cartItem.ProductTypeId);

        if (sameItem is null)
            cart.Add(cartItem);
        else
            sameItem.Quantity += cartItem.Quantity;

        await _localStorageService.SetItemAsync("cart", cart);
        OnChange.Invoke();
    }

    public async Task<List<CartItem>> GetCartItems()
    {
        var cart = await _localStorageService.GetItemAsync<List<CartItem>>("cart");

        if (cart is null) cart = new List<CartItem>();

        return cart;
    }

    public async Task<List<CartProductResponse>> GetCartProducts()
    {
        var cartItems = await _localStorageService.GetItemAsync<List<CartItem>>("cart");
        var response = await _http.PostAsJsonAsync($"{_api}products", cartItems);
        var cartProducts = await response.Content.ReadFromJsonAsync<ServiceResponse<List<CartProductResponse>>>();

        return cartProducts.Data;

    }

    public async Task RemoveProductFromCart(int productId, int productTypeId)
    {
        var cart = await _localStorageService.GetItemAsync<List<CartItem>>("cart");

        if (cart is null)
            return;

        var cartItem = cart.Find(x => x.ProductId == productId && x.ProductTypeId == productTypeId);

        if (cartItem is not null)
        {
            cart.Remove(cartItem);
            await _localStorageService.SetItemAsync("cart", cart);
            OnChange.Invoke();
        }
            
    }

    public async Task UpdateQuantity(CartProductResponse product)
    {
        var cart = await _localStorageService.GetItemAsync<List<CartItem>>("cart");

        if (cart is null)
            return;

        var cartItem = cart.Find(x => x.ProductId == product.ProductId && x.ProductTypeId == product.ProductTypeId);

        if (cartItem is not null)
        {
            cartItem.Quantity = product.Quantity;
            await _localStorageService.SetItemAsync("cart", cart);            
        }
    }
}
