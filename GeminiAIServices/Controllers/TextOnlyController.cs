using GeminiAIServices.Helpers;
using GeminiAIServices.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace GeminiAIServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TextOnlyController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        // Constructor: Injects IHttpClientFactory for creating HttpClient instances.
        public TextOnlyController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [HttpPost]
        public async Task<IActionResult> Post(string prompt)
        {
            #region Initial Object and Variable

            Root objRoot = new Root();
            Content objContent = new Content();
            Part objPart = new Part
            {
                text = prompt // Set nilai Text di sini
            };

            objContent.parts.Add(objPart);
            objRoot.contents.Add(objContent);

            #endregion

            try
            {
                #region This is Core..

                var httpClient = _clientFactory.CreateClient();
                var content = new StringContent(JsonConvert.SerializeObject(objRoot), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(Constan.CONST_URL_GOOGLE_API_GEMINI_TEXT + Constan.CONST_GOOGLE_API_KEY, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();

                    // Get value from properti "text"
                    var jObject = JObject.Parse(responseString);
                    var textValue = jObject["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();

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
