namespace Backend_Example
{
    public static class Weather
    {
        public static void SetupWeather(this WebApplication app)
        {
            app.MapGet("/weatherforecast", (string location) =>
            {
                var forecast = Enumerable.Range(1, 5).Select(index => {
                    var degrees = location.Length + index;
                    return $"Temp: {degrees}°C";
                });
                return forecast.ToArray();
            })
            .WithName("GetWeatherForecast")
            .WithOpenApi();
        }
    }
}
