using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Models;
using SearchService.Services;

namespace SearchService.Data;

public static class DbInitializer
{
    public static async Task InitDb(WebApplication app)
    {
        await DB.InitAsync("SearchDb", MongoClientSettings.FromConnectionString(app.Configuration.GetConnectionString("MongoDbConnection")));

        await DB.Index<Item>()
            .Key(x => x.Make, KeyType.Text)
            .Key(x => x.Model, KeyType.Text)
            .Key(x => x.Color, KeyType.Text)
            .CreateAsync();
        
        var count = await DB.CountAsync<Item>();
        if (count > 0) return;
        
        /*Console.WriteLine("No data found - will attempt to seed");
        var itemData = await File.ReadAllTextAsync("Data/auctions.json");
        var options = new JsonSerializerOptions{PropertyNameCaseInsensitive = true};
        var items = JsonSerializer.Deserialize<List<Item>>(itemData, options);
        await DB.SaveAsync(items);*/
        
        using var scope = app.Services.CreateScope();
        var httpClient = scope.ServiceProvider.GetRequiredService<AuctionSvcHttpClient>();
        var items = await httpClient.GetItemsForSearchDbAsync();
        Console.WriteLine($"Received {items.Count} items from auction service");

        if (items.Count > 0)
        {
            await DB.SaveAsync(items);
        }
    }
}