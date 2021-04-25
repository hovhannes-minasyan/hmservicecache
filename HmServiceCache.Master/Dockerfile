#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["HmServiceCache.Master/HmServiceCache.Master.csproj", "HmServiceCache.Master/"]
COPY ["HmServiceCache.Storage/HmServiceCache.Storage.csproj", "HmServiceCache.Storage/"]
COPY ["HmServiceCache.Common/HmServiceCache.Common.csproj", "HmServiceCache.Common/"]
RUN dotnet restore "HmServiceCache.Master/HmServiceCache.Master.csproj"
COPY . .
WORKDIR "/src/HmServiceCache.Master"
RUN dotnet build "HmServiceCache.Master.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HmServiceCache.Master.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HmServiceCache.Master.dll"]