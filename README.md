# Weather Forecast Service (WEATHY)

## General

### Prerequisites

You will need the following tools and SDKs to build, run and test the service:
* Microsoft Visual Studio 2019 (Windows)
* Microsoft .NET SDK 5.0 or later
* Docker (including docker-compose)
* WSL (Windows Subsystem for Linux) or a virtual machine with Linux

You may also need:
* Postman
* Heroku CLI

## Build, run, test

### How to build locally

You can build and run the service in Microsoft Visual Studio with [the solution file](weathy.sln).

Alternatively, you can build with Microsoft .NET SDK. To build the release version of the service, run:
``` console
dotnet build src/weathy.csproj -c release
```

In order to build the service with all dependencies, run:
``` console
dotnet publish src/weathy.csproj -c release
```

### How to run tests

To run unit and integration tests, run:
``` console
dotnet test test/weathy-test.csproj
``` 

### How to run locally

You can run the service directly from Visual Studio using the solution file.

Alternatively, you can run the service from the command line as follows:

``` console
dotnet run -p src/weathy.csproj -c release
```

### How to build a docker container

To build a docker container with the service, run:
``` console
docker build -t weathy:latest src
```

### How to run locally with docker-compose

To get the service running locally as a container, run:
``` console
docker-compose up -d
```

To stop the service, run:
```
docker-compose down
```

### How to publish and run in Heroku

The following instructions show how to deploy and test the service in Heroku.

> **NOTE:** You will need to create your own Heroku app and use the app name in the commands below.


Login to Heroku:
``` console
heroku login
```

Login to Heroku Container service:
``` console
heroku container:login
```

Build the Dockerfile in the current directory and tag it as follows:
``` console
docker tag weathy registry.heroku.com/<your-heroku-app-name>/web
```

Push the Docker image to Heroku.
``` console
docker push registry.heroku.com/<your-heroku-app-name>/web
```

Release the newly pushed images to deploy the service.
``` console
heroku container:release web --app <your-heroku-app-name>
```

## Configuration

The service can read configuration settings from:
* appsettings.json file
* environment variables
* command line arguments

The above order is also the order of preference, which means that settings could be overridden in that order. The later value will be used.

### appsettings.json

This is the basic settings file for general settings that could be applied in all environments.
These settings could be overridden by ```appsettings.{environment}.json``` if there are settings specific to the environment. ```{environment}``` defines the execution environment, for example: development, production, test.

### Environment variables

The environment variables shall be created with *WEATHY_* prefix. When added to the configuration the prefix is removed. 
For example:

on Windows:
```
set WEATHY_ENABLE_SWAGGER_UI=true
```

on Linux:
```
export WEATHY_ENABLE_SWAGGER_UI=true
```

### Command line arguments

Command line arguments could be defined like this when running the program:

```
dotnet Weathy.dll ENABLE_SWAGGER_UI=true
```

## Configuration settings

Below are the configuration settings supported by the service:

| Name | Description | Default |
|:-----|:------------|:--------|
| ASPNETCORE_URLS | Defines URL(s) that the service is listening on. The setting shall be either defined as the environment variable or ```--urls``` command-line argument. | http://localhost:5000 | 
| ASPNETCORE_ENVIRONMENT| Defines the hosting environment. The service loads configuration settings based on the environment name, i.e. appsettings.json and appsettings.{Environment}.json | Empty |
| ENABLE_SWAGGER_UI | Enables Swagger UI at /swagger endpoint | false |
| WEATHER_API_KEY | API key to use [Weather API](https://www.weatherapi.com/) | Empty |
| WEATHER_API_BASEURL | The base URL of Weather API endpoint | https://api.weatherapi.com/ |
| WEATHER_API_NUMDAYS | The number of days of forecast required | 5 |

## REST API

The service implements the REST API with the following endpoints. You can run the service with ENABLE_SWAGGER_UI enabled and find the API documentation at http://localhost:5000/docs.

### GET /api/v1/weather

Retrieve "random" weather forecast

Example:
```
curl -X 'GET' 'http://localhost:5000/api/v1/weather'
```

Response:
``` json
[
  {
    "date": "2021-05-07T19:40:04.7780682+03:00",
    "temperatureC": -10,
    "temperatureF": 15,
    "summary": "Hot"
  },
  {
    "date": "2021-05-08T19:40:04.7783563+03:00",
    "temperatureC": 20,
    "temperatureF": 67,
    "summary": "Mild"
  },
  {
    "date": "2021-05-09T19:40:04.77836+03:00",
    "temperatureC": 39,
    "temperatureF": 102,
    "summary": "Bracing"
  },
  {
    "date": "2021-05-10T19:40:04.7783603+03:00",
    "temperatureC": -4,
    "temperatureF": 25,
    "summary": "Cool"
  },
  {
    "date": "2021-05-11T19:40:04.7783606+03:00",
    "temperatureC": 3,
    "temperatureF": 37,
    "summary": "Cool"
  }
]
```

### GET ​/api​/v2​/weather?city=<city_name>

Retrieve actual weather forecast for the requested city.

Example:
```
curl -X 'GET' 'http://localhost:5000/api/v2/weather?city=london'
```

Response:
``` json
[
  {
    "date": "2021-05-06T00:00:00",
    "temperatureC": 5,
    "temperatureF": 40,
    "summary": "Patchy rain possible"
  },
  {
    "date": "2021-05-07T00:00:00",
    "temperatureC": 9,
    "temperatureF": 48,
    "summary": "Partly cloudy"
  },
  {
    "date": "2021-05-08T00:00:00",
    "temperatureC": 8,
    "temperatureF": 46,
    "summary": "Moderate rain"
  }
]
```

## Logging

The service employs [Serilog library](https://serilog.net/) instead of the standard logging middleware in Microsoft .NET. Log events are printed to the console and recorded to the log file. Default logging level and settings can be defined/changed in the Serilog section of appsettings.json file.

The following example shows how to enable different log levels for Microsoft and system components:

```
  "Serilog": {
    "MinimumLevel": {
      "Default": "Debug",
      "Override": {
        "Microsoft": "Debug",
        "System.Net.Http.HttpClient": "Error"
      }
    },
```
