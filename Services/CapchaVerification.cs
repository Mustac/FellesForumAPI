using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FellesForumAPI.Services;
public class CaptchaService
{
    private readonly HttpClient _httpClient;
    private readonly string _secretKey;
    private const string GoogleVerificationUrl = "https://www.google.com/recaptcha/api/siteverify";

    public CaptchaService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _secretKey = Environment.GetEnvironmentVariable("RECAPTCHA_SECRET_KEY")
                     ?? throw new InvalidOperationException("reCAPTCHA secret key not found in environment variables.");
    }

    public async Task<bool> VerifyCaptchaAsync(string token, float minScore = 0.5f)
    {
        var response = await _httpClient.PostAsJsonAsync(GoogleVerificationUrl, new
        {
            secret = _secretKey,
            response = token
        });

        if (!response.IsSuccessStatusCode)
            return false;

        var captchaResponse = await response.Content.ReadFromJsonAsync<CaptchaResponse>();
        return captchaResponse != null && captchaResponse.Success && captchaResponse.Score >= minScore;
    }
}


public class CaptchaResponse
{
    public bool Success { get; set; }
    public float Score { get; set; } // Used for reCAPTCHA v3
    public string[] ErrorCodes { get; set; }
}


