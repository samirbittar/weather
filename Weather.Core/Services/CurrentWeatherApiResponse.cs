using System.Collections.Generic;

namespace Weather.Core.Services
{
    public class CurrentWeatherApiResponse
    {
        public List<WeatherItem> Weather { get; set; }
        
        public class WeatherItem
        {
            public string Description { get; set; }
        }
    }
}