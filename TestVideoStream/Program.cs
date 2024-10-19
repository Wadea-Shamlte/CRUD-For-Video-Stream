using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

class Program
{
    private static readonly HttpClient client = new HttpClient();
    private static void SetHeaders(string apiToken)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiToken);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }


    static public async Task GetAll()
    {
        string apiToken = "aV9Z1-HSI2_90EWjh5Tp4GGUzLOolCZrbXPCmgbm";
        string accountId = "5a9402955ea225a36794e268ff0c4d3d";

        try
        {
            
            SetHeaders(apiToken);

            // Send API request to fetch all videos
            var response = await client.GetAsync($"https://api.cloudflare.com/client/v4/accounts/{accountId}/stream");
            response.EnsureSuccessStatusCode();

            // Read the response and parse the JSON
            var responseBody = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(responseBody);

            
            List<string> ListPreviewUrl = new List<string>();

            foreach (var video in json["result"])
            {
                string previewUrl = video["preview"].ToString();
                ListPreviewUrl.Add(previewUrl);
            }

            // Print all links
            Console.WriteLine("Preview URLs:");
            foreach (var url in ListPreviewUrl)
            {
                Console.WriteLine(url);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    static public async Task GetByID(string videoUid)
    {
        string apiToken = "aV9Z1-HSI2_90EWjh5Tp4GGUzLOolCZrbXPCmgbm";
        string accountId = "5a9402955ea225a36794e268ff0c4d3d";
        

        try
        {
            SetHeaders(apiToken);

            // Send API request to fetch all videos
            var response = await client.GetAsync($"https://api.cloudflare.com/client/v4/accounts/{accountId}/stream/{videoUid}");
            response.EnsureSuccessStatusCode();

            // Read the response and parse the JSON
            var responseBody = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(responseBody);

            string previewUrl = json["result"]["preview"].ToString();

            Console.WriteLine($"Preview URL: {previewUrl}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    static public async Task AddVideo(string filePath)
    {
        string apiToken = "aV9Z1-HSI2_90EWjh5Tp4GGUzLOolCZrbXPCmgbm"; 
        string accountId = "5a9402955ea225a36794e268ff0c4d3d"; 

        try
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiToken);

            using (var content = new MultipartFormDataContent())
            {
                // Add metadata
                var metadata = new
                {
                    defaultCreator = "",
                    meta = new { name = Path.GetFileName(filePath) }
                };
                content.Add(new StringContent(JsonConvert.SerializeObject(metadata), Encoding.UTF8, "application/json"), "payload");

                // Add video file
                var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                content.Add(new StreamContent(fileStream), "file", Path.GetFileName(filePath));

                // Send request
                var response = await client.PostAsync($"https://api.cloudflare.com/client/v4/accounts/{accountId}/stream", content);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(responseBody);

                
                string UID = json["result"]["uid"].ToString();

                
                Console.WriteLine("Video uploaded successfully: ");
                Console.WriteLine($"Video ID: {UID}");
                
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    static public async Task UpdateVideo(string videoId)
    {
        string apiToken = "aV9Z1-HSI2_90EWjh5Tp4GGUzLOolCZrbXPCmgbm";
        string accountId = "5a9402955ea225a36794e268ff0c4d3d";

        //Add metadata
        var data = new
        {
            defaultCreator = "",
            meta = new
            {
                name = "test 1"
            }

        };

        //Convert data to JSON
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        SetHeaders(apiToken);

        
        string url = $"https://api.cloudflare.com/client/v4/accounts/{accountId}/stream/live_inputs/{videoId}";
        try
        {
            HttpResponseMessage response = await client.PutAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("The video has been successfully edited.");
            }
            else
            {
                string error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Modification failed. Details: {error}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }

    }

    static public async Task DeleteVideo(string videoId)
    {
        string apiToken = "aV9Z1-HSI2_90EWjh5Tp4GGUzLOolCZrbXPCmgbm";
        string accountId = "5a9402955ea225a36794e268ff0c4d3d";

        
        string url = $"https://api.cloudflare.com/client/v4/accounts/{accountId}/stream/{videoId}";

        try
        {
            SetHeaders(apiToken);

            //Execute the DELETE request
            HttpResponseMessage response = await client.DeleteAsync(url);

            
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("The video has been successfully deleted.");
            }
            else
            {
                string error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Deletion failed. Details: {error}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            Console.ReadKey();
        }
    }

    [STAThread]
    static async Task Main(string[] args)
    {
        //Replace it with your video Path
        string Path = "C:\\Users\\wadea\\OneDrive\\Desktop\\Test Video/Test1.mp4";

        //await GetAll();
        //await GetByID("Replace it with Video ID");
        //await AddVideo(Path);
        //await DeleteVideo("Replace it with Video ID");
        //await UpdateVideo("Replace it with Video ID");

        Console.ReadKey();
    }
}


