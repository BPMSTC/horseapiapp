namespace horseappspring26.Services;

public interface ITokenStore
{
    string? AccessToken { get; set; }
    void Clear();
}
