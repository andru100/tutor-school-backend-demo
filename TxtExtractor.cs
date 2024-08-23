using Microsoft.AspNetCore.Http;

namespace Ai;

public class TxtExtractor
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public TxtExtractor(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<string> AnalyseImage(string fileUrl)
    { 
        try
        {
            var analyseImageBackend = _configuration["ANALYSE_IMAGE_BACKEND"];
            var url = analyseImageBackend;
            var formData = new MultipartFormDataContent();
            formData.Add(new StringContent(fileUrl), "image_url");
            var response = await _httpClient.PostAsync(url, formData);
            
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("error contacting txt extractor");
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            throw;
        }
    }

    public async Task<string> AnalyseDocument(string documentUrl)
    {
        try
        {
            var analyseDocumentBackend = _configuration["ANALYSE_DOCUMENT_BACKEND"];
            var url = analyseDocumentBackend;
            var formData = new MultipartFormDataContent();
            formData.Add(new StringContent(documentUrl), "document_url");
            var response = await _httpClient.PostAsync(url, formData);
            
            if (!response.IsSuccessStatusCode)
            {
                return "error contacting txt extractor";
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return "error contacting txt extractor";
        }
    }
}

