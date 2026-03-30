using Microsoft.AspNetCore.Mvc;
using WebStore.Extensions;
using WebStore.Models;
using WebStore.Repositories.Interfaces;

namespace WebStore.Controllers;

/// <summary>
/// Handles session-based shopping cart actions for public storefront.
/// </summary>
public class CartController : Controller
{
    private const string CartSessionKey = "Cart";

    private readonly IProductRepository _productRepository;
    private readonly ILogger<CartController> _logger;

    /// <summary>
    /// Creates a new <see cref="CartController"/>.
    /// </summary>
    public CartController(IProductRepository productRepository, ILogger<CartController> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    /// <summary>
    /// Displays current cart items stored in session.
    /// </summary>
    public IActionResult Index()
    {
        var cart = HttpContext.Session.GetObject<List<CartItem>>(CartSessionKey) ?? new List<CartItem>();
        return View(cart);
    }

    /// <summary>
    /// Displays checkout summary page for current session cart.
    /// </summary>
    [HttpGet]
    public IActionResult Checkout()
    {
        var cart = HttpContext.Session.GetObject<List<CartItem>>(CartSessionKey) ?? [];//retriene all cart items from session 
        if (!cart.Any())//If cart is empty, show message and redirect to homepage
        {
            TempData["InfoMessage"] = "Your cart is empty.";
            return RedirectToAction("Index", "Home");
        }

        return View(cart);//Show checkout summary page with cart items
    }

    /// <summary>
    /// Adds a product to cart or increases quantity when already exists.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int productId, int quantity = 1, CancellationToken cancellationToken = default)
    {
        if (quantity < 1)//If quantity is not provided or less than 1, default to 1
            quantity = 1;

        var product = await _productRepository.GetProductByIdAsync(productId, cancellationToken);
        if (product is null)// Checks if product exists in DB before adding to cart
        {
            TempData["InfoMessage"] = "Product not found.";//Show enduser a message if product is missing instead of failing silently or crashing
            return RedirectToAction("Index", "Home");// Redirect to homepage if product is missing
        }

        var cart = HttpContext.Session.GetObject<List<CartItem>>(CartSessionKey) ?? [];
        var item = cart.FirstOrDefault(c => c.ProductId == productId);// Check if product is already in cart

        if (item is null)//If product is not in cartbut exists in DB, add new item to cart
        {
            cart.Add(new CartItem
            {
                ProductId = product.Id,
                Name = product.Name,
                Price = product.Price,
                Quantity = quantity,
                ImageUrl = product.ImageUrl
            });
        }
        else //Otherwise if product is already in cart, update quantity and price snapshot to latest from DB
        {
            item.Quantity += quantity;
            item.Price = product.Price;
            item.Name = product.Name;
            item.ImageUrl = product.ImageUrl;
        }

        HttpContext.Session.SetObject(CartSessionKey, cart);//Save updated cart back to session
        _logger.LogInformation("Product added to cart. ProductId: {ProductId}, Quantity: {Quantity}", productId, quantity);
        TempData["InfoMessage"] = $"'{product.Name}' added to cart.";//Enduser feedback message

        return RedirectToAction("Details", "Home", new { id = productId });//return to product details page
    }

    /// <summary>
    /// Removes an item from cart by product id.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Increase(int productId)
    {
        var cart = HttpContext.Session.GetObject<List<CartItem>>(CartSessionKey) ?? [];//Retrieves cart items from session
        var item = cart.FirstOrDefault(c => c.ProductId == productId);//Finds the cart item matching the given product id
        if (item is not null)//If item exists in cart, increase quantity 
        {
            item.Quantity += 1;//increase quantity by one
            HttpContext.Session.SetObject(CartSessionKey, cart);//Save updated cart back to session
            _logger.LogInformation("Cart quantity increased. ProductId: {ProductId}, NewQuantity: {Quantity}", productId, item.Quantity);
        }
        TempData["InfoMessage"] = "Cart updated.";//Enduser feedback message
        return RedirectToAction(nameof(Index));//return to cart page to reflect changes
    }

    /// <summary>
    /// Decreases quantity by one. Removes item when quantity reaches zero.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Decrease(int productId)
    {
        var cart = HttpContext.Session.GetObject<List<CartItem>>(CartSessionKey) ?? [];//Retrieve cart from session
        var item = cart.FirstOrDefault(c => c.ProductId == productId);//Finds the cart item matching the given product id
        if (item is not null)//If item exists in cart, decrease quantity
        {
            item.Quantity -= 1;//decrease quantity by one

            if (item.Quantity <= 0)
                cart.RemoveAll(c => c.ProductId == productId);//Remove item if quantity is zero or less

            HttpContext.Session.SetObject(CartSessionKey, cart);//Save updated cart back to session
            _logger.LogInformation("Cart quantity decreased. ProductId: {ProductId}", productId);
        }
        TempData["InfoMessage"] = "Cart updated.";//Enduser feedback message

        return RedirectToAction(nameof(Index));//return to cart page to reflect changes
    }

    /// <summary>
    /// Removes an item from cart by product id.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Remove(int productId)
    {
        var cart = HttpContext.Session.GetObject<List<CartItem>>(CartSessionKey) ?? [];//REtrieve cart from session
        cart.RemoveAll(c => c.ProductId == productId);//Remove all items with matching product id (should be only one)
        HttpContext.Session.SetObject(CartSessionKey, cart);//Save updated cart back to session

        _logger.LogInformation("Product removed from cart. ProductId: {ProductId}", productId);
        TempData["InfoMessage"] = "Item removed from cart.";
        return RedirectToAction(nameof(Index));//return to cart page to reflect changes
    }

    /// <summary>
    /// Clears all items from cart.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Clear()
    {
        HttpContext.Session.Remove(CartSessionKey);// remove entire cart from session to clear it
        _logger.LogInformation("Cart cleared.");
        TempData["InfoMessage"] = "Cart cleared.";
        return RedirectToAction(nameof(Index));//return to cart page to reflect changes
    }
}
