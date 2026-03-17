namespace horseappspring26.Configuration;

public class ApiSettings
{
    public string BaseUrl { get; init; } = "https://10.0.2.2:7240/";

    public int DefaultPageNumber { get; init; } = 1;

    public int DefaultPageSize { get; init; } = 50;
}
