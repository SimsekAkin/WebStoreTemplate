using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebStore.Models;
using WebStore.Repositories.Interfaces;

namespace WebStore.Controllers;

/// <summary>
/// Handles public storefront pages such as homepage listing and product details.
/// </summary>
public class HomeController : Controller
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<HomeController> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="HomeController"/>.
    /// </summary>
    /// <param name="productRepository">Repository used to read product data.</param>
    /// <param name="logger">Logger instance for diagnostics.</param>
    public HomeController(IProductRepository productRepository, ILogger<HomeController> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    /// <summary>
    /// Displays the storefront homepage with all products or search results.
    /// </summary>
    /// <param name="searchTerm">Optional search text for product name.</param>
    /// <param name="cancellationToken">Request cancellation token propagated to EF queries.</param>
    /// <returns>The homepage view with product cards.</returns>
    public async Task<IActionResult> Index(string? searchTerm, CancellationToken cancellationToken)
    {
        // If user entered search text, filter by name; otherwise show all products.
        List<Product> products;
        if (!string.IsNullOrWhiteSpace(searchTerm))// If there is a search term, perform a search query.
                                                // Otherwise, read all products for homepage listing.
        {
            _logger.LogInformation("Homepage search requested. Term: {SearchTerm}", searchTerm);
            products = await _productRepository.SearchByNameAsync(searchTerm, cancellationToken);// Search products by name 
        }
        else
        {
            _logger.LogInformation("Homepage requested without search term.");
            products = await _productRepository.GetAllProductsAsync(cancellationToken);// Read all products for homepage listing.
        }

        ViewData["SearchTerm"] = searchTerm;//if there was a search term, we pass it to the view using ViewData so that we can display the user what they searched for.
        return View(products);// Show homepage with product cards.
    }

    /// <summary>
    /// Displays a single product details page.
    /// </summary>
    /// <param name="id">Product identifier.</param>
    /// <param name="cancellationToken">Request cancellation token propagated to EF queries.</param>
    /// <returns>
    /// Product details view when found; otherwise redirects to homepage with a user-friendly message.
    /// </returns>
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetProductByIdAsync(id, cancellationToken);//retrieve the product details by id.
        if (product is null)// If product not found, show friendly message and redirect to homepage 
        {
            _logger.LogWarning("Product details requested for missing id: {ProductId}", id);
            TempData["InfoMessage"] = "Product not found.";// Show user-friendly message if product is missing 
            return RedirectToAction(nameof(Index));// Redirect to homepage if product is missing 
        }

        return View(product);// Show product details page when found.
    }

    /// <summary>
    /// Displays the privacy information page.
    /// </summary>
    public IActionResult Privacy()
    {
        return View();
    }

    /// <summary>
    /// Displays an application error page with the current request id.
    /// </summary>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
