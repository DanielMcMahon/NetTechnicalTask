using DotNetInterview.API.Domain;

namespace DotNetInterview.API.Items;

public interface IItemService
{
    Task<IReadOnlyList<Item>> GetItemsAsync();
    Task<Item> GetItemByIdAsync(Guid id);
    Task<Item> CreateItemAsync(Item item);
    Task UpdateItemAsync(Item item);
    Task DeleteItemAsync(Guid id);
}