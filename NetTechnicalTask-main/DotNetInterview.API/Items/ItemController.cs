using DotNetInterview.API.Domain;
using Microsoft.AspNetCore.Mvc;

namespace DotNetInterview.API.Items;

[ApiController]
[Route("/api/v1/[controller]")]
[Produces("application/json")]
public class ItemController : ControllerBase
{
    private readonly IItemService _itemService;
    public ItemController(IItemService itemService)
    {
        _itemService = itemService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        try
        {
            return Ok(await _itemService.GetItemsAsync());
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        try
        {
            return Ok(await _itemService.GetItemByIdAsync(id));
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
        }
    }
    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody]Item item)
    {
        try
        {
            var result = await _itemService.CreateItemAsync(item);
            return Created(new Uri($"api/v1/Item/{result.Id}"), result);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
        }
    }
    
    [HttpPut]
    public async Task<IActionResult> Update([FromBody]Item item)
    {
        try
        {
            return Ok(await _itemService.UpdateItemAsync(item));
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
        }
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var isDeleted = await _itemService.DeleteItemAsync(id);
            return isDeleted ? Ok("Deleted") : BadRequest("Could not delete item");
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
        }
    }
}