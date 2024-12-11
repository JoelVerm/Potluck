using System.Text.Json;
using Microsoft.IdentityModel.Tokens;

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

    public static string Capital(string value)
    {
        if (value.IsNullOrEmpty())
            return "";
        if (value.Length == 1)
            return value.ToUpper();
        return value[0].ToString().ToUpper() + value[1..];
    }
}