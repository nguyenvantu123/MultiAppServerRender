using BlazorIdentity.Users.Models;
using MultiAppServer.EventBus.Events;

namespace WebApp.Events;

public record UpdateProfileEvent(UserProfile userProfile) : IntegrationEvent;
