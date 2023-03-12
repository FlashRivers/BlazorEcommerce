﻿using Blazored.LocalStorage;

namespace BlazorEcommerce.Client.Services.CartService
{
    public class CartService : ICartService
    {
        private readonly ILocalStorageService _localStorage;
        private readonly HttpClient _http;
        private readonly AuthenticationStateProvider _authStateProvider;

        public CartService(ILocalStorageService localStorage, HttpClient http,
            AuthenticationStateProvider authStateProvider)
        {
            _localStorage = localStorage;
            _http = http;
            _authStateProvider = authStateProvider;
        }

        public event Action OnChange;

        public async Task AddToCart(CartItem cartItem)
        {
            if ((await _authStateProvider.GetAuthenticationStateAsync()).User.Identity.IsAuthenticated)
            {
                Console.WriteLine("user is authenticated");
            }
            else
            {
                Console.WriteLine("user is NOT authenticated");
            }

            var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
            if (cart == null)
            {
                cart = new List<CartItem>();
            }

            var sameItem = cart.Find(x => x.ProductId == cartItem.ProductId &&
                x.ProductTypeId == cartItem.ProductTypeId);
            if (sameItem == null)
            {
                cart.Add(cartItem);
            }
            else
            {
                sameItem.Quantity += cartItem.Quantity;
            }

            await _localStorage.SetItemAsync("cart", cart);
            OnChange.Invoke();
        }

        public async Task<List<CartItem>> GetCartItems()
        {
            var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
            if (cart == null)
            {
                cart = new List<CartItem>();
            }
            return cart;
        }

        public async Task<List<CartProductResponse>> GetCartProducts()
        {
            var cartItems = await _localStorage.GetItemAsync<List<CartItem>>("cart");
            var response = await _http.PostAsJsonAsync("api/cart/products", cartItems);
            var cartProducts = await response.Content.ReadFromJsonAsync<ServiceResponse<List<CartProductResponse>>>();
            return cartProducts.Data;
        }

        public async Task RemoveProductFromCart(int productId, int productTypeId)
        {
            var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
            if (cart == null)
            {
                return;
            }

            var cartItem = cart.Find(x => x.ProductId == productId
                && x.ProductTypeId == productTypeId);
            if (cartItem != null)
            {
                cart.Remove(cartItem);
                await _localStorage.SetItemAsync("cart", cart);
                OnChange.Invoke();
            }
        }

        public async Task StoreCartItems(bool emptyLocalCart = false)
        {
            var localCart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
            if (localCart == null)
            {
                return;
            }

            await _http.PostAsJsonAsync("api/cart", localCart);

            if (emptyLocalCart)
            {
                await _localStorage.RemoveItemAsync("cart");
            }
        }

        public async Task UpdateQuantity(CartProductResponse product)
        {
            var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
            if (cart == null)
            {
                return;
            }

            var cartItem = cart.Find(x => x.ProductId == product.ProductId
                && x.ProductTypeId == product.ProductTypeId);
            if (cartItem != null)
            {
                cartItem.Quantity = product.Quantity;
                await _localStorage.SetItemAsync("cart", cart);
            }
        }
    }
}
