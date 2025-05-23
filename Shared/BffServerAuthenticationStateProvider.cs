﻿using Duende.IdentityModel;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Security.Claims;

using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;
using Shared;

public sealed class BffServerAuthenticationStateProvider : RevalidatingServerAuthenticationStateProvider, IDisposable
{
    private readonly IUserSessionStore _sessionStore;
    private readonly PersistentComponentState _state;
    private readonly NavigationManager _navigation;
    private readonly ILogger<BffServerAuthenticationStateProvider> _logger;

    private readonly PersistingComponentStateSubscription _subscription;

    private Task<AuthenticationState>? _authenticationStateTask;

    protected override TimeSpan RevalidationInterval { get; }

    public BffServerAuthenticationStateProvider(
        IUserSessionStore sessionStore,
        PersistentComponentState persistentComponentState,
        NavigationManager navigation,
        IOptions<BffBlazorOptions> options,
        ILoggerFactory loggerFactory)
        : base(loggerFactory)
    {
        _sessionStore = sessionStore;
        _state = persistentComponentState;
        _navigation = navigation;
        _logger = loggerFactory.CreateLogger<BffServerAuthenticationStateProvider>();

        // TODO - Consider separate options for server and client
        RevalidationInterval = TimeSpan.FromMilliseconds(options.Value.StateProviderPollingInterval);

        AuthenticationStateChanged += OnAuthenticationStateChanged;
        _subscription = _state.RegisterOnPersisting(OnPersistingAsync, RenderMode.InteractiveWebAssembly);
    }

    private void OnAuthenticationStateChanged(Task<AuthenticationState> task)
    {
        _authenticationStateTask = task;
    }

    private async Task OnPersistingAsync()
    {
        if (_authenticationStateTask is null)
        {
            throw new UnreachableException($"Authentication state not set in {nameof(OnPersistingAsync)}().");
        }

        var authenticationState = await _authenticationStateTask;

        var claims = authenticationState.User.Claims
            .Select(c => new ClaimLite
            {
                Type = c.Type,
                Value = c.Value?.ToString() ?? string.Empty,
                ValueType = c.ValueType == ClaimValueTypes.String ? null : c.ValueType
            }).ToArray();

        var principal = new ClaimsPrincipalLite
        {
            AuthenticationType = authenticationState.User.Identity!.AuthenticationType,
            NameClaimType = authenticationState.User.Identities.First().NameClaimType,
            RoleClaimType = authenticationState.User.Identities.First().RoleClaimType,
            Claims = claims
        };

        _logger.LogDebug("Persisting Authentication State");

        _state.PersistAsJson(nameof(ClaimsPrincipalLite), principal);
    }


    public void Dispose()
    {
        _subscription.Dispose();
        AuthenticationStateChanged -= OnAuthenticationStateChanged;
    }

    protected override async Task<bool> ValidateAuthenticationStateAsync(AuthenticationState authenticationState, CancellationToken cancellationToken)
    {
        var sid = authenticationState.User.FindFirstValue(JwtClaimTypes.SessionId);
        var sub = authenticationState.User.FindFirstValue(JwtClaimTypes.Subject);

        var sessions = await _sessionStore.GetUserSessionsAsync(new UserSessionsFilter
        {
            SessionId = sid,
            SubjectId = sub
        });
        return sessions.Count != 0;
    }
}
