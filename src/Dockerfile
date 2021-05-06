# stage 1
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /build
COPY . ./
ENV DOTNET_CLI_TELEMETRY_OPTOUT 1
RUN dotnet publish weathy.csproj -c Release -o /app
RUN rm /app/appsettings.Development.json

# stage 2
FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS final
WORKDIR /app
COPY --from=build /app .

# uncomment to see the app dir contents
# RUN ls -la /app

# create new application user and set to a known uid for monitoring purposes 
RUN adduser --system --group --disabled-login --uid 1001 --shell=/bin/false fs-app-user
RUN chown -R fs-app-user:fs-app-user /app/
USER fs-app-user

ENV DOTNET_CLI_TELEMETRY_OPTOUT 1
EXPOSE $PORT
CMD ASPNETCORE_URLS=http://*:$PORT dotnet Weathy.dll