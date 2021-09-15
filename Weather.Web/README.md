# Weather
#### Author: Samir Bittar Rosas

## Plan

### Back-end
1. Implement a `HTTP GET` API endpoint that accepts a city and country name, calls the `api.openweathermap.org/data/2.5/weather` API and retrieves the weather data.
   1. Return `200 OK` with weather data on successful OpenWeatherMap API response.
   2. Return appropriate status codes for different errors.
   3. Add retry and circuit breaker pattern with Polly
   4. Use both provided API keys to communicate with OpenWeatherMap API.

2. Implement API Key middleware
   1. This middleware will check if the client API key is provided, and valid. It will block requests without a valid API key.

3. Implement rate limiting middleware
   1. This middleware will apply rate limits to all the individual API keys. Using `AspNetCoreRateLimit` package.

4. Implement front-end.

## Build / Run / Test

To build / run / test the application, you need to install:
- [.NET Core SDK 5.0](https://docs.microsoft.com/en-us/dotnet/core/install/windows?tabs=net50).
- [Node.js v14.x](https://nodejs.org/en/)
- [Angular CLI](https://angular.io/guide/setup-local#install-the-angular-cli)

### Build

1. Create a file called `appsettings.Development.json` inside the `Weather.Web` directory. Include the content below into the file:

```json
{
    "OpenWeatherMapApi": {
        "ApiKeys": [
            "REPLACE_WITH_YOUR_API_KEY_1",
            "REPLACE_WITH_YOUR_API_KEY_2"
        ]
    }
}
```

2. Open a command line window
3. Navigate to the root directory of the repository
4. Run `dotnet build`.
5. If you run into `npm` errors, just run the `dotnet build` command again. It should work.

### Run

1. Open a command line window
2. Navigate to the root directory of the repository
3. Run `dotnet run --project Weather.Web`
4. Open your browser and navigate to `https://localhost:5001`
5. You can view API documentation on `https://localhost:5001/swagger`. You can also use this page to test rate limiting with the different API keys.

### Test

1. Open a command line window
2. Navigate to the root directory of the repository
3. Run `dotnet test -v n`

## Test cases:

```
GIVEN Input Data Is Valid
   AND Api Key Is Valid
   AND OpenWeatherApi Returns Successful Response
WHEN Calling Current Weather Endpoint
THEN Returns Success Response With Current Weather Description
```
```
GIVEN Input Data Is Valid
   AND Api Key Is Missing Or Incorrect
WHEN Calling Current Weather Endpoint
THEN Returns Unauthorised Response
```
```
GIVEN Input Data Is Valid
   AND Api Key Is Valid
WHEN Calling Current Weather Endpoint More Than Five Times In Less Than An Hour
THEN Returns TooManyRequests Response For Rate Limited Api Key
   AND Returns Successful Response For Different Api Key
```
