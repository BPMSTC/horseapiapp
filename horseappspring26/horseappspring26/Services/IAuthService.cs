namespace horseappspring26.Services;

public interface IAuthService
{
    Task<bool> LoginAsync(string email, string password, CancellationToken cancellationToken = default);
    bool IsLoggedIn { get; }
    bool IsAdmin { get; }
    void Logout();
}
