using MediatR;

namespace TicketAnnd.Application.Weather;

public record GetWeatherForecastQuery : IRequest<IEnumerable<WeatherForecast>>;

