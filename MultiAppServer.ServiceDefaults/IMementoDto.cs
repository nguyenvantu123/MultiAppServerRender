namespace MultiAppServer.ServiceDefaults
{
    public interface IMementoDto
    {
        void SaveState();
        void RestoreState();
        void ClearState();
    }
}
