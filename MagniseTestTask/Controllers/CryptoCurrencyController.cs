using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using MagniseTestTask.Constants;
using MagniseTestTask.Database;
using MagniseTestTask.WebSocketDataModels;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace MagniseTestTask.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class CryptoCurrencyController : ControllerBase
{
    private readonly ApplicationDbContext _ctx;
    private readonly IConfiguration _config;

    public CryptoCurrencyController(ApplicationDbContext ctx, IConfiguration config)
    {
        _ctx = ctx;
        _config = config;
    }

    [HttpGet]
    public ActionResult<List<string>> GetCryptoCurrencies() =>
        _ctx.CryptoCurrencies.Select(x => $"{x.FullName}-{x.ShortName}").ToList();

    [HttpGet]
    public async Task<ActionResult<List<string>>> GetCryptoCurrenciesInformation([FromQuery] string[] cryptoCurrencies)
    {
        try
        {
            using (ClientWebSocket client = new ClientWebSocket())
            {
                var result = new List<string>();
                Uri serviceUri = new Uri(_config.GetSection("WebSocketBasedServiceUri").Value ??
                                         throw new InvalidOperationException());
                var cts = new CancellationTokenSource();
                cts.CancelAfter(WebSocketConnectionTimeInterval.MaxConnectionTimeInterval);

                if (cryptoCurrencies.Any(x => _ctx.CryptoCurrencies.FirstOrDefault(y => y.ShortName == x) is null))
                {
                    throw new Exception(WebSocketExceptionMessages.BadGivenDataError);
                }

                await client.ConnectAsync(serviceUri, cts.Token);
                while (client.State == WebSocketState.Open &&
                       result.Count < WebSocketMaximumLengthOfResultData.MaxResultDataSets)
                {
                    string message = JsonSerializer.Serialize(new HelloMessage
                    {
                        apikey = _config.GetSection("CoinApiKey").Value ?? throw new InvalidOperationException(),
                        heartbeat = false,
                        subscribe_data_type = new List<string> {WebSocketMessagesSubscribeDataTypes.ExchangeRate},
                        type = WebSocketMessagesTypes.HelloMessageType,
                        subscribe_filter_asset_id =
                            cryptoCurrencies.Select(x => $"{x}/{AssetIdQuotes.UsDollars}").ToList()
                    });
                    if (!string.IsNullOrEmpty(message))
                    {
                        ArraySegment<byte> bytesToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));
                        await client.SendAsync(bytesToSend, WebSocketMessageType.Text, true, cts.Token);
                        var responseBuffer = new byte[WebSocketResponsePacketSize.ResponseMaxPacketSize *
                                                      cryptoCurrencies.Length];
                        var offset = 0;
                        var packet = WebSocketResponsePacketSize.ResponseMaxPacketSize;
                        while (true)
                        {
                            ArraySegment<byte> bytesReceived = new ArraySegment<byte>(responseBuffer, offset, packet);
                            WebSocketReceiveResult response = await client.ReceiveAsync(bytesReceived, cts.Token);
                            var responseMessage = Encoding.UTF8.GetString(responseBuffer, offset, response.Count);
                            Console.WriteLine(responseMessage);

                            if (response.EndOfMessage)
                            {
                                result.Add(Regex.Replace(Regex.Replace(responseMessage,
                                        $",\"type\":\"{WebSocketMessagesSubscribeDataTypes.ExchangeRate}\"", ""), "\"",
                                    ""));
                                break;
                            }
                        }
                    }
                }


                return Ok(JsonSerializer.SerializeToDocument(result));
            }
        }
        catch (WebSocketException webSocketException)
        {
            Log.Fatal(webSocketException.Message);
            return BadRequest("Internal Server Error");
        }
        catch (Exception e)
        {
            Log.Fatal(e.Message);
            return BadRequest("Please, enter the correct cryptocurrency(s)");
        }
    }
}