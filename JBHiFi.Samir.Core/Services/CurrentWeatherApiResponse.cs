using System.Collections.Generic;

namespace JBHiFi.Samir.Core.Services
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