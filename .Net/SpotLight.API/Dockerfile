# Etapa de compilación
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiamos el archivo de proyecto usando la ruta desde la raíz del repo
COPY [".Net/SpotLight.API/SpotLight.API.csproj", ".Net/SpotLight.API/"]
RUN dotnet restore ".Net/SpotLight.API/SpotLight.API.csproj"

# Copiamos todo el contenido del repositorio
COPY . .

# Nos movemos a la carpeta del proyecto para compilar
WORKDIR "/src/.Net/SpotLight.API"
RUN dotnet publish "SpotLight.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa final (ejecución)
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .

# Render usa el puerto 8080 por defecto
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "SpotLight.API.dll"]