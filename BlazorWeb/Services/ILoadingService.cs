namespace BlazorWeb.Services
{
    public interface ILoadingService
    {
        event Action<bool> OnLoadingChanged;
        void SetLoadingState(bool isLoading);
    }

    public class LoadingService : ILoadingService
    {
        public event Action<bool> OnLoadingChanged;
        private bool isLoading = false;

        public void SetLoadingState(bool isLoading)
        {
            this.isLoading = isLoading;
            OnLoadingChanged?.Invoke(isLoading);
        }
    }
}
