using MediatR;
using Microsoft.AspNetCore.Mvc;
using TicketAnnd.Application.Weather;

namespace TicketAnnd.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly IMediator _mediator;

    public WeatherForecastController(IMediator mediator) => _mediator = mediator;

    [HttpGet(Name = "GetWeatherForecast")]
    public async Task<IEnumerable<WeatherForecast>> Get() =>
        await _mediator.Send(new GetWeatherForecastQuery());
}
