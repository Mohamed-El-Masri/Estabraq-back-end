namespace EstabraqTourismAPI.Configuration;

public class SecuritySettings
{
    public int BcryptRounds { get; set; }
    public int RateLimitMax { get; set; }
    public int RateLimitWindowInMinutes { get; set; }
    public bool EnableCSP { get; set; } = true;
    public bool EnableHSTS { get; set; } = true;
    public int HSTSMaxAge { get; set; } = 31536000; // 1 year
}
