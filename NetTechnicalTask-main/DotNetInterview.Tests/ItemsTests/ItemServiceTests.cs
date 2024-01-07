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
        Reference = "000"
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
        Assert.AreEqual(result.Name, "CreatedItem");
    }

    [Test]
    public async Task ItemService_UpdateItemAsync_Returns_UpdatedItem()
    {
        var itemToUpdate = ItemToCreate;
        itemToUpdate.Reference = "001";
        var result = await _itemService.UpdateItemAsync(itemToUpdate);
        Assert.AreEqual("001", result.Reference);
    }

    [Test]
    public async Task ItemService_GetItemsAsync_Returns_ReadonlyList()
    {
        var results = await _itemService.GetItemsAsync();
        Assert.AreEqual(4, results.Count);
    }

    [Test]
    public async Task ItemService_GetItemByIdAsync_Returns_ItemSpecifiedById()
    {
        var requestedItem = await _itemService.GetItemByIdAsync(new Guid("90884127-b12c-4a58-85f3-e99fcdbd5b2b"));
        Assert.AreEqual("CreatedItem", requestedItem.Name);
    }

    [Test]
    public async Task ItemsService_DeleteItemAsync_Returns_True_ForFoundItem()
    {
        var isDeleted = await _itemService.DeleteItemAsync(new Guid("90884127-b12c-4a58-85f3-e99fcdbd5b2b"));
        Assert.True(isDeleted);
    }

    [Test]
    public async Task ItemService_UpdateItemAsync_Throws_ArgumentNullException_WhenNoItemFound()
    {
        Assert.ThrowsAsync(typeof(ArgumentNullException),
            () => _itemService.UpdateItemAsync(new Item() {Id = new Guid("70486387-a541-4248-a996-7bc93f8cbb6a")}));
    }
}