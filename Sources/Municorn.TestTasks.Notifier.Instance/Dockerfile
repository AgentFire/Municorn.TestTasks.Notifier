#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 8075

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Sources/Municorn.TestTasks.Notifier.Instance/Instance.csproj", "Sources/Municorn.TestTasks.Notifier.Instance/"]
COPY ["Sources/Municorn.TestTasks.Notifier.BusinessLogic/BusinessLogic.csproj", "Sources/Municorn.TestTasks.Notifier.BusinessLogic/"]
COPY ["Sources/Municorn.TestTasks.Notifier.Data/Data.csproj", "Sources/Municorn.TestTasks.Notifier.Data/"]
RUN dotnet restore "Sources/Municorn.TestTasks.Notifier.Instance/Instance.csproj"
COPY . .
WORKDIR "/src/Sources/Municorn.TestTasks.Notifier.Instance"
RUN dotnet build "Instance.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Instance.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
EXPOSE 8075
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Municorn.TestTasks.Notifier.Instance.dll"]