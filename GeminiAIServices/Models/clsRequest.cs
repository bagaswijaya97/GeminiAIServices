namespace GeminiAIServices.Models
{
    public class clsRequest
    {
    }

    #region TextOnly Request
    public class Root
    {
        public List<Content> contents { get; set; } = new List<Content>();
    }

    public class Content
    {
        public List<Part> parts { get; set; } = new List<Part>();
    }

    public class Part
    {
        public string? text { get; set; }
        public InlineData? inlineData { get; set; }

    }

    public class InlineData
    {
        public string? mimeType { get; set; }
        public byte[]? data { get; set; }
    }


    #endregion
}
