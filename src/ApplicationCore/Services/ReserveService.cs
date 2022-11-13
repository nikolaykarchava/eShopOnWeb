using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Microsoft.eShopWeb.ApplicationCore.Services;

public class ReserveService
{
    private readonly string _SBConnectionString;
    private readonly string _DOPConnectionString;
    public ReserveService(string SBConnectionString, string DOPConnectionString)
    {
        _SBConnectionString = SBConnectionString;
        _DOPConnectionString = DOPConnectionString;
    }
    
    public async Task SendToReserver(Order order)
    {
        var body = JsonSerializer.Serialize(order.OrderItems.Select(x => new {item_id = x.Id, quantity = x.Units}).ToList());
        var queueName = "orders-queue";
        await using var client = new ServiceBusClient(_SBConnectionString);
        ServiceBusSender sender = client.CreateSender(queueName);

        var message = new ServiceBusMessage(body);
        
        await sender.SendMessageAsync(message);
    }

    public async Task SendToDelivery(Order order)
    {
        var body = JsonSerializer.Serialize(new
        {
            address = order.ShipToAddress, items = order.OrderItems, total = order.Total()
        });
        HttpClient client = new HttpClient();
        await client.PostAsync(_DOPConnectionString,
            new StringContent(body));
    }
}
