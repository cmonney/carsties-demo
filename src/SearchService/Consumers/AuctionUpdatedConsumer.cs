using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Consumers;

public class AuctionUpdatedConsumer : IConsumer<AuctionUpdated>
{
    public async Task Consume(ConsumeContext<AuctionUpdated> context)
    {
        Console.WriteLine("---> Consuming auction updated: " + context.Message.Id);

        var item = new Item
        {
            Make = context.Message.Make,
            Model = context.Message.Model,
            Year = context.Message.Year,
            Color = context.Message.Color,
            Mileage = context.Message.Mileage,
        };

        var result = await DB.Update<Item>()
            .MatchID(context.Message.Id)
            .ModifyOnly(b => new { b.Make, b.Model, b.Year, b.Color, b.Mileage }, item)
            .ExecuteAsync();

        if (!result.IsAcknowledged)
        {
            throw new MessageException(typeof(AuctionUpdated), "Problem updating auction");
        }
    }
}