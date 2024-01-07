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
    public Task ItemService_CreateItemAsync_Returns_NewCreateItem()
    {
        return Task.FromResult(true);
    }

    [Test]
    public Task ItemService_UpdateItemAsync_Returns_UpdatedItem()
    {
        return Task.FromResult(true);
    }

    [Test]
    public Task ItemService_GetItemsAsync_Returns_ReadonlyList()
    {
        return Task.FromResult(true);
    }

    [Test]
    public Task ItemService_GetItemByIdAsync_Returns_ItemSpecifiedById()
    {
        return Task.FromResult(true);
    }

    [Test]
    public Task ItemsService_DeleteItemAsync_Returns_True_ForFoundItem()
    {
        return Task.FromResult(true);
    }



}