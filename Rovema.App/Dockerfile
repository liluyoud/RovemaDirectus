FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY Rovema.App.csproj .
RUN dotnet restore Rovema.App.csproj

COPY . .
RUN dotnet build Rovema.App.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish Rovema.App.csproj -c Release -o /app/publish

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
COPY --from=publish /app/publish/wwwroot .
WORKDIR /src
COPY nginx.conf /etc/nginx/nginx.conf
