#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Municorn.TestTasks.Notifier.Instance/Municorn.TestTasks.Notifier.Instance.csproj", "Municorn.TestTasks.Notifier.Instance/"]
RUN dotnet restore "Municorn.TestTasks.Notifier.Instance/Municorn.TestTasks.Notifier.Instance.csproj"
COPY . .
WORKDIR "/src/Municorn.TestTasks.Notifier.Instance"
RUN dotnet build "Municorn.TestTasks.Notifier.Instance.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Municorn.TestTasks.Notifier.Instance.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Municorn.TestTasks.Notifier.Instance.dll"]