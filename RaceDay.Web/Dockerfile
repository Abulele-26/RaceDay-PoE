# ==========================================
# Stage 1: Build
# ==========================================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy the csproj file and restore dependencies
COPY ["RaceDay.Web.csproj", "."]
RUN dotnet restore "RaceDay.Web.csproj"

# Copy everything else and build the app
COPY . .
RUN dotnet publish "RaceDay.Web.csproj" -c Release -o /app/publish

# ==========================================
# Stage 2: Runtime
# ==========================================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080

# Copy the published files from the build stage
COPY --from=build /app/publish .

# Start the app
ENTRYPOINT ["dotnet", "RaceDay.Web.dll"]