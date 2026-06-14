FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Shared/Shared.Kernel/Shared.Kernel.csproj", "Shared/Shared.Kernel/"]
COPY ["src/Services/Files/Files.Domain/Files.Domain.csproj", "Services/Files/Files.Domain/"]
COPY ["src/Services/Files/Files.Application/Files.Application.csproj", "Services/Files/Files.Application/"]
COPY ["src/Services/Files/Files.Infrastructure/Files.Infrastructure.csproj", "Services/Files/Files.Infrastructure/"]
COPY ["src/Services/Files/Files.API/Files.API.csproj", "Services/Files/Files.API/"]
RUN dotnet restore "Services/Files/Files.API/Files.API.csproj"
COPY . .
WORKDIR "/src/Services/Files/Files.API"
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN mkdir -p /app/uploads
ENTRYPOINT ["dotnet", "Files.API.dll"]
