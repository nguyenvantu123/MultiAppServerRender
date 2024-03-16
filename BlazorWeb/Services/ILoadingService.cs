namespace BlazorWeb.Services
{
    public class LoadingService
    {
        public event Action<bool> OnLoadingChanged;

        public void ShowLoading() => OnLoadingChanged?.Invoke(true);

        public void HideLoading() => OnLoadingChanged?.Invoke(false);
    }
}
