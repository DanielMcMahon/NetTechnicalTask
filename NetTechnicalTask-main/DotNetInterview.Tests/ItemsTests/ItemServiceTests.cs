using DotNetInterview.API;
using DotNetInterview.API.Domain;
using DotNetInterview.API.Items;
using DotNetInterview.API.Providers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DotNetInterview.Tests.ItemsTests;

[TestFixture]
public class ItemServiceTests
{
    private DataContext _dataContext;
    private ItemService _itemService;
    private IDateTimeProvider _dateTimeProvider;
    
    private Item ItemToCreate => new()
    {
        Id = Guid.Parse("90884127-b12c-4a58-85f3-e99fcdbd5b2b"),
        Name = "CreatedItem",
        Reference = "000"
    };

    private DateTime TheDate => new(2024, 01, 02);

    [SetUp]
    public void Setup()
    {
        var connection = new SqliteConnection("Data Source=DotNetInterview;Mode=Memory;Cache=Shared");
        connection.Open();
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseSqlite(connection)
            .Options;
        _dataContext = new DataContext(options);
    }

    [Test]
    public async Task ItemService_CreateItemAsync_Returns_NewCreateItem()
    {
        _dateTimeProvider = new MockDateTimeProvider(TheDate);
        _itemService = new ItemService(_dataContext, _dateTimeProvider);

        var result = await _itemService.CreateItemAsync(ItemToCreate);
        Assert.That("CreatedItem", Is.EqualTo(result.Name));
    }

    [Test]
    public async Task ItemService_UpdateItemAsync_Returns_UpdatedItem()
    {
        _dateTimeProvider = new MockDateTimeProvider(TheDate);
        _itemService = new ItemService(_dataContext, _dateTimeProvider);

        var itemToUpdate = ItemToCreate;
        itemToUpdate.Reference = "001";
        var result = await _itemService.UpdateItemAsync(itemToUpdate);
        Assert.That(result.Reference, Is.EqualTo("001"));
    }

    [Test]
    public async Task ItemService_GetItemsAsync_Returns_ReadonlyList()
    {
        _dateTimeProvider = new MockDateTimeProvider(TheDate);
        _itemService = new ItemService(_dataContext, _dateTimeProvider);

        var results = await _itemService.GetItemsAsync();
        Assert.That(results.Count, Is.EqualTo(4));
    }

    [Test]
    public async Task ItemService_GetItemsAsync_PricesCalculatedCorrectly()
    {
        _dateTimeProvider = new MockDateTimeProvider(TheDate);
        _itemService = new ItemService(_dataContext, _dateTimeProvider);

        var results = await _itemService.GetItemsAsync();
        
        var shorts = results.FirstOrDefault(x => x.Ref == "A123");
        Assert.That(shorts.CurrentPrice, Is.EqualTo("£31.50"));

        var tie = results.FirstOrDefault(x => x.Ref == "B456");
        Assert.That(tie.CurrentPrice, Is.EqualTo("£15.00"));

        var shoes = results.FirstOrDefault(x => x.Ref == "C789");
        Assert.That(shoes.CurrentPrice, Is.EqualTo("£56.00"));
    }

    [Test]
    public async Task ItemService_GetItemsAsync_Prices_CalculatedCorrectly_ForMondayBetweenTimes()
    {
        DateTime monday = new DateTime(2024,01,01,12,00,00);
        
        _dateTimeProvider = new MockDateTimeProvider(monday);
        _itemService = new ItemService(_dataContext, _dateTimeProvider);
        
        var results = await _itemService.GetItemsAsync();
        
        var shorts = results.FirstOrDefault(x => x.Ref == "A123");
        Assert.That(shorts.CurrentPrice, Is.EqualTo("£17.50"));

        var tie = results.FirstOrDefault(x => x.Ref == "B456");
        Assert.That(tie.CurrentPrice, Is.EqualTo("£15.00"));

        var shoes = results.FirstOrDefault(x => x.Ref == "C789");
        Assert.That(shoes.CurrentPrice, Is.EqualTo("£35.00"));
    }
    

    [Test]
    public async Task ItemService_GetItemByIdAsync_Returns_ItemSpecifiedById()
    {
        _dateTimeProvider = new MockDateTimeProvider(TheDate);
        _itemService = new ItemService(_dataContext, _dateTimeProvider);
        
        var requestedItem = await _itemService.GetItemByIdAsync(new Guid("90884127-b12c-4a58-85f3-e99fcdbd5b2b"));
        Assert.AreEqual("CreatedItem", requestedItem.Name);
    }

    [Test]
    public async Task ItemsService_DeleteItemAsync_Returns_True_ForFoundItem()
    {
        _dateTimeProvider = new MockDateTimeProvider(TheDate);
        _itemService = new ItemService(_dataContext, _dateTimeProvider);

        var isDeleted = await _itemService.DeleteItemAsync(new Guid("90884127-b12c-4a58-85f3-e99fcdbd5b2b"));
        Assert.True(isDeleted);
    }

    [Test]
    public async Task ItemService_UpdateItemAsync_Throws_ArgumentNullException_WhenNoItemFound()
    {
        _dateTimeProvider = new MockDateTimeProvider(TheDate);
        _itemService = new ItemService(_dataContext, _dateTimeProvider);

        Assert.ThrowsAsync(typeof(ArgumentNullException),
            () => _itemService.UpdateItemAsync(new Item() {Id = new Guid("70486387-a541-4248-a996-7bc93f8cbb6a")}));
    }
}