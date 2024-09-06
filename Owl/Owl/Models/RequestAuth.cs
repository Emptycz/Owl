namespace Owl.Models;

public class RequestAuth
{
    public string Scheme { get; set; } = "Bearer";
    public string Token { get; set; } = string.Empty;

    public RequestAuth Clone()
    {
        return new RequestAuth
        {
            Scheme = Scheme,
            Token = Token
        };
    }
}
