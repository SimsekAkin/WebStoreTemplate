namespace WebStore.Models;

/// <summary>
/// Represents a single product line stored in session-based shopping cart.
/// </summary>
public class CartItem
{
    /// <summary>
    /// Product id.
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Product display name snapshot.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Product unit price snapshot.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Quantity selected by user.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Optional product image URL snapshot.
    /// </summary>
    public string? ImageUrl { get; set; }
}
