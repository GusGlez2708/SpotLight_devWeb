# Etapa de compilación
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiamos el proyecto y restauramos
COPY [".Net/SpotLight.API/SpotLight.API.csproj", ".Net/SpotLight.API/"]
RUN dotnet restore ".Net/SpotLight.API/SpotLight.API.csproj"

# Copiamos todo el repositorio
COPY . .

# Compilamos la API
WORKDIR "/src/.Net/SpotLight.API"
RUN dotnet publish "SpotLight.API.csproj" -c Release -o /app/publish

# Etapa final
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .

# --- ESTA ES LA CLAVE ---
# Copiamos la carpeta Web del repositorio a la carpeta wwwroot de la API
COPY ["SpotLight.Web/", "wwwroot/"]

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "SpotLight.API.dll"]