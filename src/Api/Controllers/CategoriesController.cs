using Api.Models.Category;
using Application.UseCases.Category.Common;
using Application.UseCases.Category.CreateCategory;
using Application.UseCases.Category.DeleteCategory;
using Application.UseCases.Category.GetCategory;
using Application.UseCases.Category.ListCategories;
using Application.UseCases.Category.UpdateCategory;
using Domain.SeedWork.SearchableRepository;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;
    public CategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(typeof(CategoryModelOutput), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create([FromBody] CreateCategoryInput input, CancellationToken cancellationToken)
    {
        var output = await _mediator.Send(input, cancellationToken);

        return CreatedAtAction(nameof(Create), new { output.Id }, output);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CategoryModelOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var output = await _mediator.Send(new GetCategoryInput(id), cancellationToken);

        return Ok(output);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteCategoryInput(id), cancellationToken);
        return NoContent();
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(CategoryModelOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateCategoryApiInput apiInput, CancellationToken cancellationToken)
    {
        var input = new UpdateCategoryInput(id, apiInput.Name, apiInput.Description, apiInput.IsActive);
        var output = await _mediator.Send(input, cancellationToken);
        return Ok(output);
    }

    [HttpGet]
    [ProducesResponseType(typeof(CategoryModelOutput), StatusCodes.Status200OK)]
    public async Task<IActionResult> List(
        CancellationToken cancellationToken,
        [FromQuery] int? page = null,
        [FromQuery] int? perPage = null,
        [FromQuery] string? search = null,
        [FromQuery] string? sort = null,
        [FromQuery] SearchOrder? dir = null)
    {
        var input = new ListCategoriesInput();
        if (page is not null) input.Page = page.Value;
        if (perPage is not null) input.PerPage = perPage.Value;
        if (!string.IsNullOrEmpty(search)) input.Search = search;
        if (!string.IsNullOrEmpty(sort)) input.Sort = sort;
        if (dir is not null) input.Dir = dir.Value;

        var output = await _mediator.Send(input, cancellationToken);

        return Ok(output);
    }
}