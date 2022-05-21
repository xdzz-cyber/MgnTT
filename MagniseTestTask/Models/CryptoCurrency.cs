namespace MagniseTestTask.Models;

public class CryptoCurrency
{
    public Guid Id { get; set; }
    public string ShortName { get; set; }
    public string FullName { get; set; }
    public double Price { get; set; }
    public string AssetIdQuote { get; set; }
    public DateTime? UpdatedAt { get; set; }
}