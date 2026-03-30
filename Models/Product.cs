using System.ComponentModel.DataAnnotations;

namespace WebStore.Models;

/// <summary>
/// Represents a product that can be listed and sold in the store.
/// </summary>
public class Product
{
    /// <summary>
    /// Primary key.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Display name of the product.
    /// </summary>
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(120, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 120 characters.")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Optional product details shown on listing and detail pages.
    /// </summary>
    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
    public string? Description { get; set; }

    /// <summary>
    /// Unit price. Must be greater than zero.
    /// </summary>
    [Range(0.01, 999999.99, ErrorMessage = "Price must be between 0.01 and 999,999.99.")]
    public decimal Price { get; set; }

    /// <summary>
    /// Current stock quantity in inventory.
    /// </summary>
    [Range(0, 1000000, ErrorMessage = "Stock must be between 0 and 1,000,000.")]
    public int Stock { get; set; }

    /// <summary>
    /// Optional URL or relative path of the product image.
    /// </summary>
    [StringLength(500, ErrorMessage = "Image URL cannot exceed 500 characters.")]
    public string? ImageUrl { get; set; }
}
