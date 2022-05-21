namespace MagniseTestTask.Models;

public class Asset
{
    public string asset_id { get; set; }

    public string name { get; set; }

    public int type_is_crypto { get; set; }

    public double price_usd { get; set; }

    public DateTime data_end { get; set; }
}