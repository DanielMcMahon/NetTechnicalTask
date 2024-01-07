using System.Linq.Expressions;
using DotNetInterview.API.Domain;
using Microsoft.EntityFrameworkCore;

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

    public async Task<List<Item>> GetItemsAsync()
    {
        return await _dataContext.Items.Select(x => new Item()
            {
                Id = x.Id,
                Reference = x.Reference,
                Price = x.Price,
                Name = x.Name,
                Variations = x.Variations.Select(v => new Variation()
                {
                    Id = v.Id,
                    Size = v.Size,
                    Quantity = v.Quantity
                }).ToList()
            })
            .AsNoTracking()
            .ToListAsync();
    }

    public Task<Item?> GetItemByIdAsync(Guid id)
    {
        return _dataContext.Items.Select(x => new Item()
        {
            Id = x.Id,
            Reference = x.Reference,
            Price = x.Price,
            Name = x.Name,
            Variations = x.Variations.Select(v => new Variation()
                {
                    Quantity = v.Quantity,
                    Size = v.Size,
                    Id = v.Id
                })
                .ToList()
        }).Where(x => x.Id == id).SingleOrDefaultAsync();
    }

    public async Task<Item> CreateItemAsync(Item item)
    {
        ArgumentException.ThrowIfNullOrEmpty(item.Name);
        var result = await _dataContext.Items.AddAsync(item);
        await _dataContext.SaveChangesAsync();
        return result.Entity;
    }

    public async Task<Item> UpdateItemAsync(Item item)
    {
        var itemToUpdate = await _dataContext.Items.Where(x => x.Id == item.Id).SingleOrDefaultAsync();
        if (itemToUpdate is null) throw new ArgumentNullException("Cannot find item to update");
        itemToUpdate.Name = item.Name;
        itemToUpdate.Reference = item.Reference;
        itemToUpdate.Price = item.Price;
        await _dataContext.SaveChangesAsync();
        return item;
    }

    public async Task<bool> DeleteItemAsync(Guid id)
    {
        var item = new Item() {Id = id};
        var entityEntry = _dataContext.Items.Attach(item);
        entityEntry.State = EntityState.Deleted;
        return await _dataContext.SaveChangesAsync() > 0;
    }
}