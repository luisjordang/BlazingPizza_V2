# Etapa de compilación
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copiar archivos del proyecto y restaurar dependencias
COPY ["BlazingPizza.csproj", "./"]
RUN dotnet restore "BlazingPizza.csproj"
COPY . .

# Compilar y publicar la aplicación
RUN dotnet build "BlazingPizza.csproj" -c Release -o /app/build
RUN dotnet publish "BlazingPizza.csproj" -c Release -o /app/publish

# Etapa de ejecución
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "BlazingPizza.dll"]