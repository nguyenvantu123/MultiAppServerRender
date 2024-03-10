

using BlazorWeb.Constants.Localization;

namespace BlazorWeb.Localization
{
    public static class LocalizationConstants
    {
        public static readonly LanguageCode[] SupportedLanguages = {
            new LanguageCode
            {
                Code = "en-US",
                DisplayName= "English"
            },
            new LanguageCode
            {
                 Code = "vn-VN",
                DisplayName= "Vietnamese"
            }
        };
    }
}
