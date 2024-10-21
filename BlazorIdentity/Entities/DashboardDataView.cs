using System.Collections.Generic;

namespace BlazorIdentityApi.Entities;

public class DashboardDataView
{
    public int ClientsTotal { get; set; }

    public int ApiResourcesTotal { get; set; }

    public int ApiScopesTotal { get; set; }

    public int IdentityResourcesTotal { get; set; }

    public int IdentityProvidersTotal { get; set; }
}