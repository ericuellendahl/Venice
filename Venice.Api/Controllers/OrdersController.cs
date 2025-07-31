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
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto createOrderDto)
    {
        try
        {
            var order = await _orderService.CreateOrderAsync(createOrderDto);
            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return BadRequest("Erro interno do servidor");
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CreateOrderDto), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetOrder(Guid id)
    {
        try
        {
            var order = await _orderService.GetOrderByIdAsync(id);
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
