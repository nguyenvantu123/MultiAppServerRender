using MultiAppServer.EventBus.Events;
using Shared.Models;

namespace Shared;

public record UpdateProfileEvent(UserProfileViewModel userProfile) : IntegrationEvent;
