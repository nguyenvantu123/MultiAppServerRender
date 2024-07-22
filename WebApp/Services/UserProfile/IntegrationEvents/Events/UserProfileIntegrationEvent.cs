

using BlazorWebApi.Users.Models;
using MultiAppServer.EventBus.Events;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace eShop.WebApp.Services.OrderStatus.IntegrationEvents;

public record UserProfileIntegrationEvent : IntegrationEvent
{
    public long Id { get; set; }

    public Guid UserId { get; set; }

    public string LastPageVisited { get; set; } 
    public bool IsNavOpen { get; set; }
    public bool IsNavMinified { get; set; }
    public int Count { get; set; } 
    public string Culture { get; set; }

    public bool IsDarkMode { get; set; } 

    public UserProfileIntegrationEvent(
        long id, Guid userId, string lastPageVisited, bool isNavOpen, bool isNavMinified, int count , string culture, bool isDarkMode)
    {
        Id = id;
        UserId = userId;
        LastPageVisited = lastPageVisited;
        IsNavOpen = isNavOpen;
        IsNavMinified = isNavMinified;
        Count = count;
        Culture = culture;
        IsDarkMode = isDarkMode;
    }
}

