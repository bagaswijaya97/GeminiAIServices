namespace GeminiAIServices.Helpers
{
    public class General
    {
    }

    public static class Constan
    {
        public const int CONST_RES_CD_SUCCESS = 200;
        public const int CONST_RES_CD_ERROR = 400;
        public const int CONST_RES_CD_CATCH = 500;

        public const string CONST_RES_MESSAGE_SUCCESS = "Success !";
        public const string CONST_RES_MESSAGE_ERROR = "Error !";

        #region Google API

        public const string CONST_URL_GOOGLE_API_GEMINI_TEXT = "https://generativelanguage.googleapis.com/v1beta/models/gemini-pro:generateContent?key=";
        public const string CONST_GOOGLE_API_KEY = "YOUR-API-KEY";

        #endregion




    }
}
