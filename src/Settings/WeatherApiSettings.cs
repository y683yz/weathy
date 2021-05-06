namespace Weathy.Settings
{
    public class WeatherApiSettings
    {
        public string ApiKey { get; set; }
        public string BaseUrl { get; set; } = "https://api.weatherapi.com/";
        public int NumDays { get; set; } = 5;
    }
}
