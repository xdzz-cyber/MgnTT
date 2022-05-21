namespace MagniseTestTask.WebSocketDataModels;

public class HelloMessage
{
    public string type { get; set; }
    
    public bool heartbeat { get; set; }
    
    public string apikey { get; set; }
    
    public List<string> subscribe_data_type { get; set; }

    public List<string> subscribe_filter_asset_id { get; set; }
}