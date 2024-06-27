namespace BlazorWebApi.Users
{
    [Permissions(Actions.Create | Actions.Update | Actions.Delete)]
    public partial class LocalizationRecord 
    {
        [Key]
        public long Id { get; set; }
        public string MsgId { get; set; }
        public string MsgIdPlural { get; set; }
        public string Translation { get; set; }
        public string Culture { get; set; }
        public string ContextId { get; set; }
        //public ICollection<PluralTranslation> PluralTranslations { get; set; }
        //ICollection<IPluralTranslation> ILocalizationRecord.PluralTranslations { get => PluralTranslations?.Select(i => (IPluralTranslation)i).ToList(); }
    }
}
