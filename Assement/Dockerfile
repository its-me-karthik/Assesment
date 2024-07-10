# Use the official .NET Core runtime as a parent image
FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
# Use the official .NET Core SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Assement/Assement.csproj", "Assement/"]
RUN dotnet restore "Assement/Assement.csproj"
COPY . .
WORKDIR "/src/Assement"
RUN dotnet build "Assement.csproj" -c Release -o /app/build
FROM build AS publish
RUN dotnet publish "Assement.csproj" -c Release -o /app/publish
# Use the runtime image to run the application
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Assement.dll"]