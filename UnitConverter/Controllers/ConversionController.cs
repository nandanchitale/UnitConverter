using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using Services;
using System;
using System.ComponentModel;
using UnitConversionApi.Models;
using UnitConversionApi.Services;

namespace UnitConversionApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")] // Informs OpenAPI that all endpoints return JSON
public class ConversionController : ControllerBase
{
    private readonly IConversionService _conversionService;

    public ConversionController(IConversionService conversionService)
    {
        _conversionService = conversionService;
    }

    [HttpGet]
    [EndpointSummary("Convert a numerical value from one unit to another")]
    [EndpointDescription("Performs a case-insensitive, normalized unit conversion utilizing a graph-based pathfinding engine. Automatically resolves transitive relationships (e.g., meter to kilometer via intermediary steps) and isolates distinct measurement categories (islands) to prevent invalid operations.")]
    [ProducesResponseType(typeof(ConversionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public IActionResult GetConversion(
        [FromQuery, Description("The source unit to convert from (e.g., 'meter', 'cm', 'fahrenheit')")] string from,
        [FromQuery, Description("The destination unit to convert to (e.g., 'kilometer', 'ft', 'celsius')")] string to,
        [FromQuery, Description("The raw numerical magnitude to scale")] double value)
    {
        if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to))
        {
            return BadRequest(new { Error = "Both 'from' and 'to' query parameters are required." });
        }

        try
        {
            var response = _conversionService.Convert(from, to, value);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { Error = "An unexpected error occurred while processing your conversion request." });
        }
    }
}