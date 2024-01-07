using DotNetInterview.API.Domain;

namespace DotNetInterview.API.Items;

public class ItemService : IItemService
{
    /// <summary>
    /// DataContext for access to the database
    /// </summary>
    private readonly DataContext _dataContext;

    public ItemService(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public Task<IReadOnlyList<Item>> GetItemsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Item> GetItemByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Item> CreateItemAsync(Item item)
    {
        throw new NotImplementedException();
    }

    public Task<Item> UpdateItemAsync(Item item)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteItemAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}