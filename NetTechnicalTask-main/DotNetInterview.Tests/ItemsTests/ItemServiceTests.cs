using DotNetInterview.API;
using DotNetInterview.API.Domain;
using DotNetInterview.API.Items;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DotNetInterview.Tests.ItemsTests;

[TestFixture]
public class ItemServiceTests
{
    private DataContext _dataContext;
    private ItemService _itemService;

    private Item ItemToCreate => new()
    {
        Id = Guid.Parse("90884127-b12c-4a58-85f3-e99fcdbd5b2b"),
        Name = "CreatedItem",
    };

    [SetUp]
    public void Setup()
    {
        var connection = new SqliteConnection("Data Source=DotNetInterview;Mode=Memory;Cache=Shared");
        connection.Open();
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseSqlite(connection)
            .Options;
        _dataContext = new DataContext(options);
        _itemService = new ItemService(_dataContext);
    }

    [Test]
    public async Task ItemService_CreateItemAsync_Returns_NewCreateItem()
    {
        var result = await _itemService.CreateItemAsync(ItemToCreate);
        Assert.Equals(result.Name, "CreatedItem");
    }

    [Test]
    public async Task ItemService_UpdateItemAsync_Returns_UpdatedItem()
    {
        var itemToUpdate = ItemToCreate;
        itemToUpdate.Reference = "001";
        var result = await _itemService.UpdateItemAsync(itemToUpdate);
        Assert.Equals(result.Reference, "001");
    }

    [Test]
    public async Task ItemService_GetItemsAsync_Returns_ReadonlyList()
    {
        var results = await _itemService.GetItemsAsync();
        Assert.IsAssignableFrom<IReadOnlyList<Item>>(results);
        Assert.Equals(results.Count, 4);
    }

    [Test]
    public async Task ItemService_GetItemByIdAsync_Returns_ItemSpecifiedById()
    {
        var requestedItem = await _itemService.GetItemByIdAsync(new Guid("90884127-b12c-4a58-85f3-e99fcdbd5b2b"));
        Assert.Equals(requestedItem.Name, "CreatedItem");
        Assert.Equals(requestedItem.Reference, "001");
    }

    [Test]
    public async Task ItemsService_DeleteItemAsync_Returns_True_ForFoundItem()
    {
        var isDeleted = await _itemService.DeleteItemAsync(new Guid("90884127-b12c-4a58-85f3-e99fcdbd5b2b"));
        Assert.True(isDeleted);
    }
}