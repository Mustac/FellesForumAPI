using Blazored.LocalStorage;
using FellesForumAPI.Data;
using FellesForumAPI.Helpers;
using FellesForumAPI.Models.DTO;
using FellesForumAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Net;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

public class SmsService : BaseService
{
    
    private readonly string localStoreageSaveName = "gid";
    public int ResendSmsTime { get; } = 60;

    private readonly IMemoryCache _cache;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILocalStorageService _localStorageService;
    private readonly TimeSpan _rateLimitDuration;

    public SmsService(IMemoryCache cache, IHttpContextAccessor httpContextAccessor, ILocalStorageService localStorageService, ApplicationDbContext db)
        : base(db)
    {
        _rateLimitDuration = TimeSpan.FromSeconds(ResendSmsTime);
        _cache = cache;
        _httpContextAccessor = httpContextAccessor;
        _localStorageService = localStorageService;
    }

    /// <summary>
    /// Send an SMS and store identifiers (phone, IP, GUID) in the cache to enforce rate limits.
    /// </summary>
    public async Task<Outcome<int>> SendSmsAsync(string phone) => 
        await InjectDatabaseFunc(async (context) =>
    {
        var user = await context.Users.FirstOrDefaultAsync(x => x.Phone == phone);

        if (user == null)
            return new Outcome<int>(false, 0, "User does not exist, SMS has not been sent.");

        var guid = Guid.NewGuid().ToString();

        var timeStamp = DateTime.UtcNow;

        if (!string.IsNullOrEmpty(guid))
            await SaveIndentifierToCache(guid, timeStamp, saveToLocalStorage: true);

        if (!string.IsNullOrEmpty(phone))
            await SaveIndentifierToCache(phone, timeStamp);

        var userIp = GetClientIpAddress();
        if (!string.IsNullOrEmpty(userIp))
            await SaveIndentifierToCache(userIp, timeStamp);

        var randomCode = new Random().Next(100000, 1000000).ToString();

        user.LastToken = randomCode;

        if(await context.SaveChangesAsync()==0)
            return new Outcome<int>(false, 0, "Could not establish connection to a database");

        var message = await MessageResource.CreateAsync(
            body: $"Your authentication code is : {randomCode}",
            from: new Twilio.Types.PhoneNumber("+12294718104"),
            to: new Twilio.Types.PhoneNumber($"+47{user.Phone}"));

        return new Outcome<int>(true, (int)_rateLimitDuration.TotalSeconds, "SMS sent successfully.");
    });

    /// <summary>
    /// Checks if the SMS can be resent, based on cached identifiers.
    /// Returns the remaining time (in seconds) until resending is allowed.
    /// </summary>
    public async Task<Outcome<int>> GetResendingTimer(string phone = "", bool useGuidFromLocalStorage = false) =>
        await InjectFunc(async () =>
        {
            var userIp = GetClientIpAddress();

            // Check for IP-based rate limiting
            if (!string.IsNullOrEmpty(userIp) && _cache.TryGetValue(userIp, out DateTime ipTimestamp))
            {
                int remainingTime = GetRemainingSeconds(ipTimestamp);
                if (remainingTime > 0)
                    return new Outcome<int>(false, remainingTime, "Please wait before resending.");
            }

            // Check for phone-based rate limiting
            if (!string.IsNullOrEmpty(phone) && _cache.TryGetValue(phone, out DateTime phoneTimestamp))
            {
                int remainingTime = GetRemainingSeconds(phoneTimestamp);
                if (remainingTime > 0)
                    return new Outcome<int>(false, remainingTime, "Please wait before resending.");
            }

            // Check for GUID-based rate limiting
            var clientId = await _localStorageService.GetItemAsStringAsync(localStoreageSaveName);
            if (!string.IsNullOrEmpty(clientId) && _cache.TryGetValue(clientId, out DateTime guidTimestamp))
            {
                int remainingTime = GetRemainingSeconds(guidTimestamp);
                if (remainingTime > 0)
                    return new Outcome<int>(false, remainingTime, "Please wait before resending.");
            }

            return new Outcome<int>(true, 0, "You can send SMS now.");
        });

    /// <summary>
    /// Saves the given identifier and timestamp to the cache.
    /// Optionally saves the identifier to local storage.
    /// </summary>
    private async Task SaveIndentifierToCache(string identifier, DateTime timeStamp, bool saveToLocalStorage = false)
    {
        if (!string.IsNullOrEmpty(identifier))
        {
            if (saveToLocalStorage)
                await _localStorageService.SetItemAsStringAsync(localStoreageSaveName, identifier);

            _cache.Set(identifier, timeStamp, new MemoryCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = _rateLimitDuration
            });
        }
    }

    /// <summary>
    /// Calculates the remaining time (in seconds) based on the stored timestamp and the rate limit duration.
    /// </summary>
    private int GetRemainingSeconds(DateTime savedTimestamp)
    {
        var elapsed = DateTime.UtcNow - savedTimestamp;
        var remainingTime = _rateLimitDuration.TotalSeconds - elapsed.TotalSeconds;
        return remainingTime > 0 ? (int)Math.Ceiling(remainingTime) : 0;
    }

    /// <summary>
    /// Retrieves the client's IP address.
    /// </summary>
    private string GetClientIpAddress()
    {
        var ipAddress = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress;

        // Handle IPv6 mapped IPv4
        if (ipAddress is not null && ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
        {
            ipAddress = Dns.GetHostEntry(ipAddress).AddressList
                .FirstOrDefault(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
        }

        return ipAddress?.ToString() ?? "Unknown";
    }
}
