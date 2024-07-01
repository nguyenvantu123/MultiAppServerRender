namespace WebApp
{
    public interface IDynamicComponent
    {
        string IntoComponent { get; }
        int Order { get; }
    }
}
