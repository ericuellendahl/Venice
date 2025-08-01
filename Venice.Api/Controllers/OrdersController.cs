using Microsoft.AspNetCore.Mvc;
using Venice.Application.DTOs;
using Venice.Application.Interfaces;

namespace Venice.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController(IOrderService _orderService) : ControllerBase
{

    [HttpPost]
    [ProducesResponseType(typeof(CreateOrderDto), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto createOrderDto, CancellationToken cancellationToken)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var order = await _orderService.CreateOrderAsync(createOrderDto, cancellationToken);
            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest($"Erro interno do servidor {ex.Message}");
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CreateOrderDto), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetOrder(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var order = await _orderService.GetOrderByIdAsync(id, cancellationToken);
            if (order is null)
                return NotFound();

            return Ok(order);
        }
        catch (Exception)
        {
            return BadRequest("Erro interno do servidor");
        }
    }
}
