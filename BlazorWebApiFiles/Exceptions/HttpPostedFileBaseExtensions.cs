using System.Text.RegularExpressions;

namespace BlazorWebApi.Files.Exceptions
{
    public static class HttpPostedFileBaseExtensions
    {
        //public const int ImageMinimumBytes = 512;

        public static bool IsImage(IFormFile FormFile)
        {
            //-------------------------------------------
            //  Check the image mime types
            //-------------------------------------------
            if (!string.Equals(FormFile.ContentType, "image/jpg", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(FormFile.ContentType, "image/jpeg", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(FormFile.ContentType, "image/pjpeg", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(FormFile.ContentType, "image/gif", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(FormFile.ContentType, "image/x-png", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(FormFile.ContentType, "image/png", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            //-------------------------------------------
            //  Check the image extension
            //-------------------------------------------
            var postedFileExtension = Path.GetExtension(FormFile.FileName);
            if (!string.Equals(postedFileExtension, ".jpg", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(postedFileExtension, ".png", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(postedFileExtension, ".gif", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(postedFileExtension, ".jpeg", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            //-------------------------------------------
            //  Try to instantiate new Bitmap, if .NET will throw exception
            //  we can assume that it's not a valid image
            //-------------------------------------------
            return true;
        }
    }
}
