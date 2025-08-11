using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WeatherApp.Services;
using WeatherApp.Data;

namespace WeatherApp.UI
{
    /// <summary>
    /// UI Controller for the Weather Application
    /// Students will connect this to the API client and handle user interactions
    /// </summary>
    public class WeatherUIController : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TMP_InputField cityInputField;
        [SerializeField] private Button getWeatherButton;
        [SerializeField] private TextMeshProUGUI weatherDisplayText;
        [SerializeField] private TextMeshProUGUI statusText;
        
        [Header("API Client")]
        [SerializeField] private WeatherApiClient apiClient;
        
        private void Start()
        {
            // Set up button click listener
            getWeatherButton.onClick.AddListener(OnGetWeatherClicked);

            // Initialize UI state
            SetStatusText("Enter a city name and click Get Weather");
        }
        
        /// TODO: Students will implement this method
        private async void OnGetWeatherClicked()
        {
            // Get city name from input field
            string cityName = cityInputField.text;
            
            // Validate input
            if (string.IsNullOrWhiteSpace(cityName))
            {
                SetStatusText("Please enter a city name");
                return;
            }
            
            // Disable button and show loading state
            getWeatherButton.interactable = false;
            SetStatusText("Loading weather data...");
            weatherDisplayText.text = "";
            
            try
            {
                // TODO: Call API client to get weather data
              
                
                // TODO: Handle the response
            }
            catch (System.Exception ex)
            {
                // Handle exceptions
                Debug.LogError($"Error getting weather data: {ex.Message}");
                SetStatusText("An error occurred. Please try again.");
            }
            finally
            {
                // Re-enable button
                getWeatherButton.interactable = true;
            }
        }
        
        /// TODO: Students will implement this method
        private void DisplayWeatherData(WeatherData weatherData)
        {
            // TODO: Format and display weather information
            // Example format:
            // City: London
            // Temperature: 15.2°C (Feels like: 14.1°C)
            // Description: Clear sky
            // Humidity: 65%
            // Pressure: 1013 hPa

            string displayText = "";
            
            // TODO: Add more weather details
            if (weatherData.Main != null)
            {
                displayText += "";
                displayText += "";
            }
            
            weatherDisplayText.text = displayText;
        }
        
        private void SetStatusText(string message)
        {
            if (statusText != null)
            {
                statusText.text = message;
            }
        }
        
        public void ClearDisplay()
        {
            weatherDisplayText.text = "";
            cityInputField.text = "";
            SetStatusText("Enter a city name and click Get Weather");
        }
    }
}

using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;

public class ApiClient : MonoBehaviour
{
    [SerializeField] private string apiKey = "7796a05f3a9e798b75bdf18431e6b71d";
    [SerializeField] private string baseUrl = "http://api.openweathermap.org/data/2.5/weather";

    public async Task<string> GetWeatherDataAsync(string city)
    {
        string url = $"{baseUrl}?q={city}&appid={apiKey}&units=metric"; // Celsius

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            var operation = request.SendWebRequest();

            while (!operation.isDone)
                await Task.Yield();

            if (request.result == UnityWebRequest.Result.Success)
            {
                return request.downloadHandler.text;
            }

            Debug.LogError($"Request failed: {request.error}");
            return null;
        }
    }
}

Weather Data.CS
using Newtonsoft.Json;
using System;

namespace WeatherApp.Data
{
    [Serializable]
    public class WeatherData
    {
        [JsonProperty("main")]
        public MainWeatherInfo Main { get; set; }
        
        [JsonProperty("weather")]
        public WeatherDescription[] Weather { get; set; }
        
        [JsonProperty("name")]
        public string CityName { get; set; }
        
        [JsonProperty("cod")]
        public int StatusCode { get; set; }
        
        // Now already in Celsius (API handles units)
        public float TemperatureInCelsius => Main?.Temperature ?? 0f;
        public string PrimaryDescription => Weather?.Length > 0 ? Weather[0].Description : "Unknown";
        
        public bool IsValid => StatusCode == 200 && Main != null && !string.IsNullOrEmpty(CityName);
    }

    [Serializable]
    public class MainWeatherInfo
    {
        [JsonProperty("temp")]
        public float Temperature { get; set; }
        
        [JsonProperty("feels_like")]
        public float FeelsLike { get; set; }
        
        [JsonProperty("humidity")]
        public int Humidity { get; set; }
        
        [JsonProperty("pressure")]
        public int Pressure { get; set; }
    }

    [Serializable]
    public class WeatherDescription
    {
        [JsonProperty("main")]
        public string Main { get; set; }
        
        [JsonProperty("description")]
        public string Description { get; set; }
        
        [JsonProperty("icon")]
        public string Icon { get; set; }
    }
}
WeatherTest.cs
using UnityEngine;
using Newtonsoft.Json;
using WeatherApp.Data;

public class WeatherTest : MonoBehaviour
{
    private async void Start()
    {
        var apiClient = gameObject.AddComponent<ApiClient>();

        string city = "London"; // Change as needed
        string json = await apiClient.GetWeatherDataAsync(city);

        if (!string.IsNullOrEmpty(json))
        {
            var weather = JsonConvert.DeserializeObject<WeatherData>(json);

            if (weather.IsValid)
            {
                Debug.Log($"City: {weather.CityName}");
                Debug.Log($"Temperature: {weather.Main.Temperature}°C");
                Debug.Log($"Feels like: {weather.Main.FeelsLike}°C");
                Debug.Log($"Humidity: {weather.Main.Humidity}%");
                Debug.Log($"Description: {weather.PrimaryDescription}");
            }
            else
            {
                Debug.LogError("Invalid weather data received.");
            }
        }
    }
}
