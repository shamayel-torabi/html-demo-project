// Ignore Spelling: Pdf Gc Api Gpt

using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GcPdfViewerSupportApiDemo.Controllers

{
    /// <summary>
    /// A controller that acts as a middleware for interacting with OpenAI's ChatGPT API.
    /// Provides endpoints for client applications to send requests without exposing the API key.
    /// </summary>
    [ApiController] // Specifies that this class is an API controller, enabling automatic request validation and routing.
    [Route("api/openai")] // Defines the base route for all endpoints in this controller (e.g., /api/chatgpt/summarize).
    public class ChatGptController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public ChatGptController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Endpoint to interact with OpenAI's ChatGPT.
        /// Receives a model name and user input, sends a request to OpenAI, and returns the response.
        /// </summary>
        [HttpPost("summarize")]
        public async Task<IActionResult> Summarize([FromBody] ChatGptRequest request)
        {
            // Get the API key from the environment variable and decode it from Base64
            var encodedApiKey = Environment.GetEnvironmentVariable("OPEN_AI_KEY");
            if (string.IsNullOrEmpty(encodedApiKey))
            {
                return StatusCode(500, "API key is not configured in the environment variables.");
            }

            var apiKey = Encoding.UTF8.GetString(Convert.FromBase64String(encodedApiKey));

            // Construct the payload for OpenAI's API
            var openAiPayload = new
            {
                model = request.Model,
                messages = new[]
                {
                    new { role = "system", content = $"You are a helpful assistant. Your responses must be in {(string.IsNullOrEmpty(request.UserLanguage) ? "English": request.UserLanguage)} and never in another language." },
                    
                    new { role = "user", content = request.Content }
                }
            };

            var openAiRequestContent = new StringContent(
                JsonSerializer.Serialize(openAiPayload),
                Encoding.UTF8,
                "application/json"
            );

            // Add the API key to the Authorization header
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

            try
            {
                // Send the POST request to OpenAI's ChatGPT endpoint
                var openAiResponse = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", openAiRequestContent);

                // Handle unsuccessful responses from OpenAI
                if (!openAiResponse.IsSuccessStatusCode)
                {
                    var errorContent = await openAiResponse.Content.ReadAsStringAsync();
                    return StatusCode((int)openAiResponse.StatusCode, errorContent);
                }

                // Deserialize and return the successful response
                var responseContent = await openAiResponse.Content.ReadAsStringAsync();
                return Ok(JsonSerializer.Deserialize<object>(responseContent));
            }
            catch (Exception ex)
            {
                // Handle unexpected errors
                return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Request DTO for interacting with ChatGPT.
    /// </summary>
    public class ChatGptRequest
    {
        public string Model { get; set; } // The model to use, e.g., "gpt-4".
        public string Content { get; set; } // The input text to summarize or analyze.
        public string UserLanguage { get; set; }
    }
}

