# ------------------------------------------------------------------ build ---

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

WORKDIR /app

ARG APP_NAME=gateway

COPY . ./${APP_NAME}

RUN dotnet restore ${APP_NAME}

# ----------------------------------------------------------------- publish ---

FROM build as publish

ARG APP_NAME=gateway

RUN dotnet publish ./${APP_NAME} -c Release -o /${APP_NAME}/publish/

# --------------------------------------------------------------------- app ---

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS app
EXPOSE 80

ARG APP_NAME=gateway
ENV APP_NAME ${APP_NAME}

WORKDIR /app

COPY --from=publish /${APP_NAME}/publish/ .

ENTRYPOINT ["dotnet", "gateway.dll"]
