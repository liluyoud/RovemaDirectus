FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
EXPOSE 8080
WORKDIR /src
COPY ["Rovema.App/Rovema.App.csproj", "Rovema.App/"]
RUN dotnet restore "./Rovema.App/Rovema.App.csproj"

COPY . .
WORKDIR "/src/Rovema.App"
RUN dotnet build "./Rovema.App.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./Rovema.App.csproj" -c Release -o /app/publish

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
COPY --from=publish /app/publish/wwwroot .
COPY nginx.conf /etc/nginx/nginx.conf
