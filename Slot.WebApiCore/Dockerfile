FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /app

COPY . ./

WORKDIR /app/Slot.WebApiCore
RUN dotnet restore
RUN dotnet publish -c Release -o /app/out

FROM microsoft/dotnet:2.2-aspnetcore-runtime AS runtime
WORKDIR /app

COPY --from=build /app/out ./

ENTRYPOINT ["dotnet", "Slot.WebApiCore.dll"]
