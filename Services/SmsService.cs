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

public class SmsService : BaseService
{
    private readonly string localStoreageSaveName = "gid";
    private readonly string cachePrefix = "RateLimit:";

    public SmsService(IMemoryCache cache, IHttpContextAccessor httpContextAccessor, ILocalStorageService localStorageService, ApplicationDbContext db) : base(db)
    {
        _cache = cache;
        _httpContextAccessor = httpContextAccessor;
        _localStorageService = localStorageService;
    }

    private readonly IMemoryCache _cache;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILocalStorageService _localStorageService;
    private readonly TimeSpan _rateLimitDuration = TimeSpan.FromSeconds(30);

    public async Task<Outcome> SendSmsAsync(string phone)
    {
        //// Get the client's IP address

        //// Check rate limit for IP
        //if (!string.IsNullOrEmpty(clientIp) && _cache.TryGetValue(clientIp, out _))
        //{
        //    return new Outcome(false, "You can only send one SMS every 30 seconds.");
        //}

        //// Fallback: Check rate limit for the phone number
        //if (_cache.TryGetValue(phoneNumber, out _))
        //{
        //    return new Outcome(false, "You can only send one SMS every 30 seconds.");
        //}


        try
        {
            // Generating some indentifiers for limiting sms abuse
            var userIp = GetClientIpAddress();
            var guid = Guid.NewGuid().ToString();


            var timeStamp = DateTime.UtcNow;

            await SaveIndentifierToCache(userIp, timeStamp);
            await SaveIndentifierToCache(phone, timeStamp);
            await SaveIndentifierToCache(guid, timeStamp, saveToLocalStorage:true);

            // Simulate asynchronous operation
            await Task.Delay(500);

            return new Outcome(true, "SMS sent successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending SMS: {ex.Message}");
            return new Outcome(false, "Failed to send SMS.");
        }
    }

    public async Task<Outcome> GetUsersCachedDataAsync(string ipAddress="")
    {
        bool isSavedAsIpId;

        if (!string.IsNullOrEmpty(ipAddress))
        {
            isSavedAsIpId = false;
        }

        var clientId = await _localStorageService.GetItemAsStringAsync(localStoreageSaveName);

        if (string.IsNullOrEmpty(clientId))
        {
            await _localStorageService.RemoveItemAsync(localStoreageSaveName);
        }

        return null;

    }

    //public async Task<Outcome> CheckIfSmsWasSentRecently()

    private string GetClientIpAddress()
    {
        var ipAddress = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress;

        // Handle IPv6 mapped IPv4
        if (ipAddress is not null && ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
        {
            ipAddress = Dns.GetHostEntry(ipAddress).AddressList.FirstOrDefault(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
        }

        return ipAddress?.ToString() ?? "Unknown";
    }

    private async Task SaveIndentifierToCache(string identifier, DateTime timeStamp, bool saveToLocalStorage = false)
    {
        if (!string.IsNullOrEmpty(identifier))
        {
            if (saveToLocalStorage)
                await _localStorageService.SetItemAsStringAsync(localStoreageSaveName, identifier);

            _cache.Set(cachePrefix + identifier, timeStamp, new MemoryCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = _rateLimitDuration
            });
        }
    }

    //Checks if sms can be sent, and gives back time in seconds untill it can
    private async Task<Outcome<int>> CanSendSms(string guid = "", string phone = "")
    {
        return null;
    }

    private async Task<Outcome> RegisterUserAsync(RegisterUserDto newUser)
        => await NewFunctionAsync(async context =>
        {
            string phone = newUser.Phone.Value.ToString();

            // Check if the user exists in the database
            bool userExists = await context.Users.AnyAsync(x => x.Phone == phone);

            // Return the appropriate outcome
            return userExists
                ? new Outcome(false, "User already exists!") // Outcome is false if the user already exists
                : new Outcome(true, "User does not exist and can be registered!");
        });





}
