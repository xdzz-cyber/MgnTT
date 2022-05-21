using System.Text.Json;
using MagniseTestTask.Models;
using Serilog;

namespace MagniseTestTask.Database;

public static class DbInitializer
{
    public static async Task Seed(ApplicationDbContext ctx, IConfiguration config)
    {
        try
        {
            if (!ctx.CryptoCurrencies.Any())
            {
                var assets = await GetAssetsFromApi(config);
                var cryptoCurrencies = assets.Where(x => x.type_is_crypto == 1).Select(x => new CryptoCurrency()
                {
                    Id = Guid.NewGuid(),
                    ShortName = x.asset_id,
                    FullName = x.name,
                    Price = x.price_usd,
                    AssetIdQuote = "USD",
                    UpdatedAt = x.data_end
                }).Take(15).ToList();

                await ctx.CryptoCurrencies.AddRangeAsync(cryptoCurrencies);
                await ctx.SaveChangesAsync();
            }
        }
        catch(InvalidOperationException e)
        {
            Log.Fatal(e.Message);
        }
        
    }

    private static async Task<List<Asset>> GetAssetsFromApi(IConfiguration configuration)
    {
        using (var response = await new HttpClient().GetAsync(
                   $"{configuration.GetSection("GetAssetsUrl").Value}{configuration.GetSection("CoinApiKey").Value}",
                   HttpCompletionOption.ResponseHeadersRead))
        {
            response.EnsureSuccessStatusCode();
            var stream = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<List<Asset>>(stream) ?? throw new InvalidOperationException();
        }
    }
}