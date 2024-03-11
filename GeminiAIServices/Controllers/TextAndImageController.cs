using GeminiAIServices.Helpers;
using GeminiAIServices.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;

namespace GeminiAIServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TextAndImageController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        // Constructor: Injects IHttpClientFactory for creating HttpClient instances.
        public TextAndImageController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [HttpPost]
        public async Task<IActionResult> Post(string prompt, IFormFile file, CancellationToken cancellationToken)
        {
            #region Get File

            byte[] data;
            await using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                data = memoryStream.ToArray();
            }

            #endregion

            #region Validation

            // Mengecek ukuran file
            if (data.Length > 4194304) // 4 MB dalam byte
            {
                // Mencoba mengompresi data jika melebihi 4 MB
                data = GeneralHelper.CompressData(data);

                // Mengecek lagi setelah kompresi
                if (data.Length > 4194304)
                {
                    // Membuat objek respons dengan kode dan pesan kesalahan
                    var errorResponse = new
                    {
                        code = Constan.CONST_RES_CD_ERROR,
                        message = Constan.CONST_RES_MESSAGE_ERROR_FILE_SIZE
                    };

                    // Mengembalikan respons BadRequest dengan body berformat JSON
                    return BadRequest(errorResponse);
                }
            }

            #endregion

            #region Initial Object and Variable

            char[] trimChars = { ' ', ',', '.', '?', '!', ':', '\t', '\n', '\r' };

            // Set Value Prompt Text
            Root objRoot = new Root();
            Content objContent = new Content();
            Part textPart = new Part
            {
                text = prompt,
            };

            // Set Value Image
            Part imagePart = new Part
            {
                inlineData = new InlineData
                {
                    mimeType = GeneralHelper.GetMimeType(file.FileName),
                    data = data
                }
            };

            objContent.parts.Add(textPart);
            objContent.parts.Add(imagePart);
            objRoot.contents.Add(objContent);

            #endregion

            try
            {
                #region This is Core..

                var httpClient = _clientFactory.CreateClient();
                var content = new StringContent(JsonConvert.SerializeObject(objRoot), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(Constan.CONST_URL_GOOGLE_API_GEMINI_TEXT_AND_IMAGE + Constan.CONST_GOOGLE_API_KEY, content, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();

                    // Get value from properti "text"
                    var jObject = JObject.Parse(responseString);
                    var textValue = jObject["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString().Trim(trimChars);

                    return Ok(textValue);
                }
                else
                {
                    // Handling different HTTP status codes from the external API response.
                    switch (response.StatusCode)
                    {
                        case System.Net.HttpStatusCode.BadRequest:
                            return BadRequest(await response.Content.ReadAsStringAsync());
                        case System.Net.HttpStatusCode.Unauthorized:
                        case System.Net.HttpStatusCode.Forbidden:
                            return Unauthorized(await response.Content.ReadAsStringAsync());
                        case System.Net.HttpStatusCode.NotFound:
                            return NotFound(await response.Content.ReadAsStringAsync());
                        default:
                            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
                    }
                }
            }
            catch (HttpRequestException e)
            {
                // Handling network-related errors with a 500 Internal Server Error status.
                return StatusCode(Constan.CONST_RES_CD_CATCH, $"Catch#1 External service communication error: {e.Message}");
            }
            catch (Exception e)
            {
                // Handling unexpected errors with a 500 Internal Server Error status.
                return StatusCode(Constan.CONST_RES_CD_CATCH, $"Catch#2 An unexpected error occurred: {e.Message}");

                #endregion
            }
        }
    }
}
