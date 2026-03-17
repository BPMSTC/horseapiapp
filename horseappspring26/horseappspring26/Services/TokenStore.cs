namespace horseappspring26.Services;

public class TokenStore : ITokenStore
{
    private const string AccessTokenKey = "horse_api_access_token";

    public string? AccessToken
    {
        get => Preferences.Get(AccessTokenKey, null);
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                Preferences.Remove(AccessTokenKey);
                return;
            }

            Preferences.Set(AccessTokenKey, value);
        }
    }

    public void Clear()
    {
        Preferences.Remove(AccessTokenKey);
    }
}
