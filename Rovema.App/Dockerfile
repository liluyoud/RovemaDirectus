FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY Rovema.App/Rovema.App.csproj ./Rovema.App/
RUN dotnet restore Rovema.App/Rovema.App.csproj

COPY . .
RUN dotnet publish "./Rovema.App.csproj" -c Release -o /app/publish

WORKDIR /usr/share/nginx/html
RUN rm -rf ./*
COPY --from=publish /app/publish /usr/share/nginx/html

FROM nginx:alpine
WORKDIR /app
COPY Rovema.App/nginx.conf /etc/nginx/nginx.conf
EXPOSE 8080 
ENTRYPOINT ["nginx", "-g", "daemon off;"]
