using BlazorEcommerce.Shared;
using Blazored.LocalStorage;

namespace BlazorEcommerce.Client.Services.CartService;

public class CartService : ICartService
{
    private readonly ILocalStorageService _localStorageService;

    public CartService(ILocalStorageService localStorageService)
    {
        _localStorageService = localStorageService;
    }

    public event Action OnChage;


    public async Task AddToCart(CartItem cartItem)
    {
        var cart = await _localStorageService.GetItemAsync<List<CartItem>>("cart");

        if (cart is null) cart = new List<CartItem>();

        cart.Add(cartItem);

        await _localStorageService.SetItemAsync("cart", cart);
    }

    public async Task<List<CartItem>> GetCartItems()
    {
        var cart = await _localStorageService.GetItemAsync<List<CartItem>>("cart");

        if (cart is null) cart = new List<CartItem>();

        return cart;
    }
}
