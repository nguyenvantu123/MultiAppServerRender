using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public static class SharedExtensions
    {
        public static string GetFileNameFromMultipartContent(MultipartFormDataContent content)
        {
            foreach (var httpContent in content)
            {
                if (httpContent.Headers.ContentDisposition != null)
                {
                    var fileName = httpContent.Headers.ContentDisposition.FileName?.Trim('"');
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        return fileName;
                    }
                }
            }
            return string.Empty;
        }

        public static string GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ".txt" => "text/plain",
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".png" => "image/png",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".gif" => "image/gif",
                ".csv" => "text/csv",
                _ => "application/octet-stream",
            };
        }
    }

}
