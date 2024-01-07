using DotNetInterview.API.Domain;

namespace DotNetInterview.API.Items;

public interface IItemService
{
    Task<List<ItemDTO>> GetItemsAsync();
    Task<Item?> GetItemByIdAsync(Guid id);
    Task<Item> CreateItemAsync(Item item);
    Task<Item> UpdateItemAsync(Item item);
    Task<bool> DeleteItemAsync(Guid id);
}