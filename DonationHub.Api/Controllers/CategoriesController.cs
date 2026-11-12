using DonationHub.Application.Features.Categories.Create;
using DonationHub.Application.Features.Categories.Delete;
using DonationHub.Application.Features.Categories.GetAll;
using DonationHub.Application.Features.Categories.GetById;
using DonationHub.Application.Features.Categories.Update;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DonationHub.Api.Controllers;

[ApiController]
[Route("api/v1/categories")]
public class CategoriesController : ControllerBase
{
    private readonly CreateCategoryHandler _createHandler;
    private readonly UpdateCategoryHandler _updateHandler;
    private readonly DeleteCategoryHandler _deleteHandler;
    private readonly GetCategoryByIdHandler _getByIdHandler;
    private readonly GetAllCategoriesHandler _getAllHandler;

    public CategoriesController(
        CreateCategoryHandler createHandler,
        UpdateCategoryHandler updateHandler,
        DeleteCategoryHandler deleteHandler,
        GetCategoryByIdHandler getByIdHandler,
        GetAllCategoriesHandler getAllHandler)
    {
        _createHandler = createHandler;
        _updateHandler = updateHandler;
        _deleteHandler = deleteHandler;
        _getByIdHandler = getByIdHandler;
        _getAllHandler = getAllHandler;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _getAllHandler.HandleAsync();
        return Ok(categories);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id)
    {
        var category = await _getByIdHandler.HandleAsync(id);
        if (category == null)
        {
            return NotFound();
        }
        return Ok(category);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request)
    {
        var category = await _createHandler.HandleAsync(request);
        if (category == null)
        {
            return Conflict();
        }
        return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryRequest request)
    {
        var existing = await _getByIdHandler.HandleAsync(id);
        if (existing == null)
        {
            return NotFound();
        }

        var category = await _updateHandler.HandleAsync(id, request);
        if (category == null)
        {
            return Conflict();
        }

        return Ok(category);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var existing = await _getByIdHandler.HandleAsync(id);
        if (existing == null)
        {
            return NotFound();
        }

        var deleted = await _deleteHandler.HandleAsync(id);
        if (!deleted)
        {
            return Conflict();
        }

        return NoContent();
    }
}
