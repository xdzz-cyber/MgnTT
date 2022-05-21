FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["MagniseTestTask/MagniseTestTask.csproj", "MagniseTestTask/"]
RUN dotnet restore "MagniseTestTask/MagniseTestTask.csproj"
COPY . .
WORKDIR "/src/MagniseTestTask"
RUN dotnet build "MagniseTestTask.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MagniseTestTask.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MagniseTestTask.dll"]
