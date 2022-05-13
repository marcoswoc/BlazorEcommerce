using BlazorEcommerce.Shared;

namespace BlazorEcommerce.Client.Services.CartService;

public interface ICartService
{
    event Action OnChage;
    Task AddToCart(CartItem cartItem);
    Task<List<CartItem>> GetCartItems();
}
