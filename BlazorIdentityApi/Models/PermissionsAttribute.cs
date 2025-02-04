using BlazorIdentityApi.Models;

namespace BlazorIdentityApi.Users
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PermissionsAttribute : Attribute
    {
        public Actions Actions;

        public PermissionsAttribute(Actions actions)
        {
            Actions = actions;
        }
    }
}
