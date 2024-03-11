using System.IO.Compression;
using System.Text;

namespace GeminiAIServices.Helpers
{
    public class General
    {
    }

    public static class GeneralHelper
    {
        public static byte[] CompressData(byte[] data)
        {
            using (var output = new MemoryStream())
            {
                using (var gzip = new GZipStream(output, CompressionMode.Compress))
                {
                    gzip.Write(data, 0, data.Length);
                }

                return output.ToArray();
            }
        }

        public static string GetMimeType(string fileName)
        {
            var provider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(fileName, out var contentType))
            {
                contentType = "application/octet-stream"; // Default MIME type
            }
            return contentType;
        }

    }

    public static class Constan
    {
        public const int CONST_RES_CD_SUCCESS = 200;
        public const int CONST_RES_CD_ERROR = 400;
        public const int CONST_RES_CD_CATCH = 500;

        public const string CONST_RES_MESSAGE_SUCCESS = "Success !";
        public const string CONST_RES_MESSAGE_ERROR = "Error !";
        public const string CONST_RES_MESSAGE_ERROR_FILE_SIZE = "Ukuran file melebihi batas maksimum 4 MB.";

        #region Google API

        public const string CONST_URL_GOOGLE_API_GEMINI_TEXT = "https://generativelanguage.googleapis.com/v1beta/models/gemini-pro:generateContent?key=";
        public const string CONST_URL_GOOGLE_API_GEMINI_TEXT_AND_IMAGE = "https://generativelanguage.googleapis.com/v1beta/models/gemini-pro-vision:generateContent?key=";
        public const string CONST_GOOGLE_API_KEY = "YOUR-API-KEY";

        #endregion




    }
}
