namespace BlazorWebApi.Users
{
    [Flags]
    public enum Actions
    {
        Create = 1,
        Update = 2,
        Read = 4,
        Delete = 8,
        // ReSharper disable once InconsistentNaming
        CRUD = Create | Update | Read | Delete,
        // ReSharper disable once InconsistentNaming
        CUD = Create | Update | Delete
    }
}
