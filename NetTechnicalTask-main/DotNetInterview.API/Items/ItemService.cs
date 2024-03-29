﻿using System.Linq.Expressions;
using DotNetInterview.API.Domain;
using DotNetInterview.API.Providers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Internal;

namespace DotNetInterview.API.Items;

public class ItemService : IItemService
{
    /// <summary>
    /// DataContext for access to the database
    /// </summary>
    private readonly DataContext _dataContext;

    private static IDateTimeProvider _dateTimeProvider;

    public ItemService(DataContext dataContext, IDateTimeProvider dateTimeProvider)
    {
        _dataContext = dataContext;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<List<ItemDTO>> GetItemsAsync()
    {
        return await _dataContext.Items.Select(x => new ItemDTO()
            {
                Id = x.Id,
                Ref = x.Reference,
                OriginalPrice = $"{x.Price:C}",
                CurrentPrice = CalculateCurrentPrice(x.Price, x.Variations.Sum(v => v.Quantity)),
                ItemName = x.Name,
                Status = x.Variations.Any() ? $"In Stock ({x.Variations.Sum(v => v.Quantity)})" : "Sold out",
            })
            .AsNoTracking()
            .ToListAsync();
    }

    /// <summary>
    /// Calculates the current prices set from rules in the task requirements.
    /// As this is a separate method, this will run in memory and iterate through each record. and will not generate SQL on the server.
    /// Pro: Being able to calculate this in code
    /// Con: Subject to slow performance once the data set gets too big. 
    /// </summary>
    /// <param name="originalPrice"></param>
    /// <param name="qty"></param>
    /// <returns></returns>
    private static string CalculateCurrentPrice(decimal originalPrice, int qty)
    {
        if (_dateTimeProvider.Now is {DayOfWeek: DayOfWeek.Monday, Hour: >= 12 and < 17} && qty > 0)
            return $"{(originalPrice * 0.5m):C}";

        return qty switch
        {
            (> 5) and (<= 10) => $"{(originalPrice * 0.9m):C}",
            (> 10) => $"{(originalPrice * 0.8m):C}",
            _ => $"{originalPrice:C}"
        };
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
                        Id = v.Id,
                        ItemId = v.ItemId
                    })
                    .ToList()
            }).Where(x => x.Id == id)
            .SingleOrDefaultAsync();
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
        var item = new Item {Id = id};
        
        var entityEntry = _dataContext.Items.Attach(item);
        entityEntry.State = EntityState.Deleted;
        
        return await _dataContext.SaveChangesAsync() > 0;
    }
}