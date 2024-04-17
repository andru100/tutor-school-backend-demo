using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Model;


namespace Ai
{
    public class Assistant
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _openAiApiAddress;
        private readonly string _openAiApiKey;


        public Assistant(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _openAiApiAddress = _configuration["OPENAI_API_ADDRESS"];
            _openAiApiKey = _configuration["OPENAI_API_KEY"];
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_openAiApiKey}");
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13;
        }

        // standard gpt message
        public async Task<string> GenerateMessageAsync(string prompt)
        {
            try
            {
                var request = new
                {
                    model = "gpt-3.5-turbo",
                    messages = new[]
                    {
                        new { role = "system", content = "You are a helpful assistant." },
                        new { role = "user", content = prompt }
                    }
                };

                var jsonContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(_openAiApiAddress, jsonContent);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var responseObject = JsonConvert.DeserializeObject<dynamic>(responseContent);

                return responseObject.choices[0].message.content;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine(ex.InnerException.Message);
                }
                throw;
            }
        }
    
        // Provides ai assitant feedback on submitted homework
        public async Task<string> HomeworkFeedbackAgent(HomeworkFeedbackInput input)
        {
            try
            {
                var requestFeedback = $"I have been set homework for {input.stream} with the instructions {input.instructions}. I have submitted the following: {input.submission}. Can you tell me anything I could improve and any mistakes I've made?";
                Console.WriteLine(requestFeedback);
                var request = new
                {
                    model = "gpt-3.5-turbo",
                    messages = new[]
                    {
                        new { role = "system", content = $"You are a helpful tutor helping a student with their homework. The curriculum and level they are working towards is: {input.stream} You will be given the homework instructions and what the student has submitted. Your responses should be short and to the point. You should explain what was done well, what mistakes were made and suggest ways to improve. Under no circumstances can you give exaples of what you would write. Because we dont you to do the work for them. you should suggest and allow them to edit it" },
                        new { role = "user", content = requestFeedback }
                    }
                };

                var jsonContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(_openAiApiAddress, jsonContent);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var responseObject = JsonConvert.DeserializeObject<dynamic>(responseContent);

                return responseObject.choices[0].message.content;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine(ex.InnerException.Message);
                }
                throw;
            }
        }
    }
    
}

