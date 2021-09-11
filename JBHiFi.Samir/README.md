# JB Hi-Fi Programming Challenge
#### Author: Samir Bittar Rosas

## Plan of attack: Keep it simple.
### Back-end
1. Implement a `HTTP GET` API endpoint that accepts a city and country name, calls the `api.openweathermap.org/data/2.5/weather` API and retrieves the weather data.
   1. If input validation fails, return `400 Bad Request` with validation message.
   2. If `description` field is present in Open Weather Map's API response, return `200 OK` with the value in the `description` field.
   3. If `cod` field is present with value `404`, return `404 Not Found` with message from Open Weather Map's API.

2. Implement logic to switch `openweathermap` API keys

3. Implement API Key middleware
   1. This middleware will check if the API key is provided, and valid. It will block requests without a valid API key.

4. Implement rate limiting middleware
   1. This middleware will apply rate limits to all the individual API keys.



## Assumptions

## Test cases

### Back-end
1. *Given* a get request to /api/weather has a properly formatted 
