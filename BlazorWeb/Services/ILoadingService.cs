namespace BlazorWeb.Services
{
    public interface ILoadingService
    {
        event Action<bool> OnLoadingChanged;

        void ShowLoading();
        void HideLoading();
    }

    public class LoadingService : ILoadingService
    {
        public event Action<bool> OnLoadingChanged;

        public void ShowLoading()
        {
            OnLoadingChanged?.Invoke(true);
        }

        public void HideLoading()
        {
            OnLoadingChanged?.Invoke(false);
        }
    }
}
