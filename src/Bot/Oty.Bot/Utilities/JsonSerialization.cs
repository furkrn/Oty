namespace Oty.Bot.Utilities;

public static class JsonSerialization
{
    public static async Task<string> SerializeAsync<TValue>(TValue value)
    {
        await using var memoryStream = new MemoryStream();
        await JsonSerializer.SerializeAsync(memoryStream, value);

        memoryStream.Position = 0;
        using var streamReader = new StreamReader(memoryStream);
        return await streamReader.ReadToEndAsync();
    }
}