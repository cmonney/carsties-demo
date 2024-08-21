using System.Globalization;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Services;

public class AuctionSvcHttpClient(HttpClient httpClient, IConfiguration configuration)
{
    public async Task<List<Item>> GetItemsForSearchDbAsync()
    {
        var lastUpdated = await DB.Find<Item, string>()
            .Sort(x => x.Descending(item => item.UpdatedAt))
            .Project(x => x.UpdatedAt.ToString())
            .ExecuteFirstAsync();
        
        var uri = new Uri($"{configuration["AuctionServiceUrl"]}/api/auctions?date={lastUpdated}");
        return await httpClient.GetFromJsonAsync<List<Item>>(uri.ToString());
    }
}