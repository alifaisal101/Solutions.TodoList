# root Dockerfile for Solutions.TodoList.WebApi
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
ARG APP_UID=1000
ARG APP_GID=1000

# create non-root user to run app
RUN addgroup --gid $APP_GID appgroup || true \
 && adduser --disabled-password --gecos "" --uid $APP_UID --gid $APP_GID appuser || true

USER appuser
WORKDIR /app

EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# copy project file(s) for restore. adjust path to the WebApi csproj in your repo
COPY ["src/Presentation/Solutions.TodoList.WebApi/Solutions.TodoList.WebApi.csproj", "src/Presentation/Solutions.TodoList.WebApi/"]
RUN dotnet restore "src/Presentation/Solutions.TodoList.WebApi/Solutions.TodoList.WebApi.csproj"

# copy everything and build
COPY . .
WORKDIR "./src/Presentation/Solutions.TodoList.WebApi"
RUN dotnet build "Solutions.TodoList.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Solutions.TodoList.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Solutions.TodoList.WebApi.dll"]
