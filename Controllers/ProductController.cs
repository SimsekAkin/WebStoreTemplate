using Microsoft.AspNetCore.Mvc;
using WebStore.Models;
using WebStore.Repositories.Interfaces;

namespace WebStore.Controllers;

/// <summary>
/// Manages product CRUD screens used in exam scenarios.
/// </summary>
public class ProductController : Controller
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<ProductController> _logger;

    /// <summary>
    /// Creates a new <see cref="ProductController"/>.
    /// </summary>
    /// <param name="productRepository">Repository used for product operations.</param>
    /// <param name="logger">Logger instance for diagnostics.</param>
    public ProductController(IProductRepository productRepository, ILogger<ProductController> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    /// <summary>
    /// Displays all products in a single management page.
    /// </summary>
    /// <param name="cancellationToken">Cancels DB query if request is aborted.</param>
    /// <returns>Product list page.</returns>
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        // Read all products for table listing.
        var products = await _productRepository.GetAllProductsAsync(cancellationToken);
        return View(products);
    }

    /// <summary>
    /// Opens add or edit screen depending on id.
    /// id == 0 means Add mode.
    /// </summary>
    /// <param name="id">Product id. Null/0 means Add mode.</param>
    /// <param name="cancellationToken">Cancels DB query if request is aborted.</param>
    /// <returns>Upsert form in Add or Edit mode.</returns>
    [HttpGet]
    public async Task<IActionResult> Upsert(int? id, CancellationToken cancellationToken)
    {
        if (!id.HasValue || id.Value == 0)// If no id or id=0, treat as Add mode and return empty form.
        {
            // Add mode: return empty model.
            ViewData["FormMode"] = "Add";// this means that the form is in Add mode. We can use this info in the view to adjust titles, buttons, etc.
            return View(new Product());// Show empty form for adding new product.
        }

        // Edit mode: load existing product.
        var product = await _productRepository.GetProductByIdAsync(id.Value, cancellationToken);// Try to read the product to edit. If not found, show friendly message 
        if (product is null)
        {
            _logger.LogWarning("Edit form requested for missing product id: {ProductId}", id.Value);
            // Friendly UX: show message and return to listing page.
            TempData["InfoMessage"] = "Product not found.";
            return RedirectToAction(nameof(Index));// Redirect to list page if product not found.
        }

        ViewData["FormMode"] = "Edit";//Temporary data to indicate form mode (Add vs Edit) for view logic.
        return View(product);// Show edit form with existing product data.
    }

    /// <summary>
    /// Saves add/edit form data.
    /// </summary>
    /// <param name="model">Posted product form model.</param>
    /// <param name="cancellationToken">Cancels DB query if request is aborted.</param>
    /// <returns>Redirects to list page on success, returns same view on validation error.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]// Prevent CSRF (Cross-Site Request Forgery) attacks.
    public async Task<IActionResult> Upsert(Product model, CancellationToken cancellationToken)
    {
        // If validation fails, show same form with error messages.
        if (!ModelState.IsValid)
        {
            ViewData["FormMode"] = model.Id == 0 ? "Add" : "Edit";//Temporary data to indicate form mode (Add vs Edit) for view logic.
            return View(model);// Show form again with validation errors.
        }

        if (model.Id == 0)// Add mode: create new product.
        {
            // Add flow.
            await _productRepository.AddProductAsync(model, cancellationToken);// Repository will set the new id back to model.Id after saving.
            _logger.LogInformation("Product added. Name: {ProductName}", model.Name);
            TempData["InfoMessage"] = "Product added successfully.";//show success message after adding new product.
            return RedirectToAction(nameof(Index));// Redirect to list page after successful add.
        }

        // Edit flow: read tracked product first.
        var existing = await _productRepository.GetProductByIdAsync(model.Id, cancellationToken);//retrieve the existing product from the database to ensure it's being 
        // tracked by the context. This allows us to detect changes and update only if necessary.
        if (existing is null)// If product to edit does not exist, show friendly message 
        {
            _logger.LogWarning("Update attempted for missing product id: {ProductId}", model.Id);
            TempData["InfoMessage"] = "Product not found.";// Show user-friendly message if product to update is missing instead of failing silently or crashing.
            return RedirectToAction(nameof(Index));// Redirect to list page if product to update is missing.
        }

        // Copy edited fields to tracked entity. This allows the repository to detect changes and only update if something was actually modified.
        existing.Name = model.Name;
        existing.Description = model.Description;
        existing.Price = model.Price;
        existing.Stock = model.Stock;
        existing.ImageUrl = model.ImageUrl;

        // Repository returns false when no data changed.
        var changed = await _productRepository.UpdateProductAsync(existing, cancellationToken);// Update will return false if no changes 
        //were detected, which lets us show a "Nothing was changed." message to the user instead of always saying "Product updated successfully." even when they didn't modify anything.
        if (!changed)// if there is no changes
        {
            _logger.LogInformation("Update skipped. No changes detected for product id: {ProductId}", model.Id);
            TempData["InfoMessage"] = "No changes detected.";// Show message when user submitted the form without making any changes.
            return RedirectToAction(nameof(Index));// Redirect to list page 
        }

        _logger.LogInformation("Product updated. Id: {ProductId}", model.Id);
        TempData["InfoMessage"] = "Product updated successfully.";// Show success message after updating product.
        return RedirectToAction(nameof(Index));// Redirect to list page after successful update.
    }

    /// <summary>
    /// Deletes a product by id.
    /// </summary>
    /// <param name="id">Product id to delete.</param>
    /// <param name="cancellationToken">Cancels DB query if request is aborted.</param>
    /// <returns>Redirects back to list with result message.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]// Prevent CSRF (Cross-Site Request Forgery) attacks.
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        // Repository returns false when product does not exist.
        var deleted = await _productRepository.DeleteProductByIdAsync(id, cancellationToken);//retrieve the product to delete first 
        // to ensure it exists and is tracked by the context. This allows the repository to return false if the product was not found, 
        // which lets us show a user-friendly message instead of failing silently or crashing.
        if (!deleted)// if there is no product with the specified id
        {
            _logger.LogWarning("Delete attempted for missing product id: {ProductId}", id);
            TempData["InfoMessage"] = "Product not found.";// Show user-friendly message if product to delete is missing instead of failing silently or crashing.
            return RedirectToAction(nameof(Index));// Redirect to list page if product to delete is missing.
        }

        _logger.LogInformation("Product deleted. Id: {ProductId}", id);
        TempData["InfoMessage"] = "Product deleted successfully.";// Show success message after deleting product.

        return RedirectToAction(nameof(Index));// Redirect to list page after successful deletion.
    }
}
