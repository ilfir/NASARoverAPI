#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["NASARoverAPI/NASARoverAPI.csproj", "NASARoverAPI/"]
RUN dotnet restore "NASARoverAPI/NASARoverAPI.csproj"
COPY . .
WORKDIR "/src/NASARoverAPI"
RUN dotnet build "NASARoverAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NASARoverAPI.csproj" -c Release -o /app/publish

FROM base AS final
VOLUME /datavolume
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NASARoverAPI.dll"]
