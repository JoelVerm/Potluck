using System.Text.Json;

namespace Potluck.Helpers;

public static class RestHelpers
{
    private static readonly JsonSerializerOptions jsonSerializerOptions =
        new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

    public static IResult JSON<T>(T value)
    {
        return Results.Json(value, jsonSerializerOptions);
    }

    public static string GetUserName()
    {
        var ctx = new HttpContextAccessor().HttpContext;
        var user = ctx?.User.Identity?.Name;
        return user ?? "";
    }
}