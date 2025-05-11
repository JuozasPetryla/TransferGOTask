using TransferGOTask.NotificationService.Application.DTOs;
using TransferGOTask.NotificationService.Application.Exceptions;

namespace TransferGOTask.NotificationService.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using Domain.ValueObjects;
using Models;
using Application.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly INotificationRequestMapper _mapper;

    public NotificationController(INotificationService notificationService, INotificationRequestMapper mapper)
    {
        _notificationService = notificationService;
        _mapper = mapper;
    }
    
    [HttpPost]
    public async Task<IActionResult> Send([FromBody] NotificationRequest req, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var requestDto = _mapper.Map(req);
        var result = await _notificationService.SendNotificationAsync(requestDto, cancellationToken);
        return AcceptedAtAction(nameof(GetStatus), new { id = result.NotificationId }, new { id = result.NotificationId });
    }

    [HttpGet("{id:guid}/status")]
    public async Task<IActionResult> GetStatus(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var status = await _notificationService.GetStatusAsync(id, cancellationToken);
            return Ok(new { id, status });
        }
        catch (NotificationNotFoundException)
        {
            return NotFound();
        }
    }
}

