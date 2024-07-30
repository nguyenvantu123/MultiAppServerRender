namespace BlazorWebApi.Files.Constant
{
    public enum FileType
    {
        UserProfile,
        Media,
        Sign,
        Mark
    }

    public enum DocumentType
    {
        Certificate,
        Contract,
        KyDuLieu,
        DongDauDuLieu
    }

    public enum DocumentStatus
    {
        New,
        Sign,
        Mark,
        Receive,
        UploadAgain
    }
}
