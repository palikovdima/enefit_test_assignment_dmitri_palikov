FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY *.props .
COPY *.sln . 

COPY BakeSale.API/*.csproj ./BakeSale.API/
COPY BakeSale.Domain/*.csproj ./BakeSale.Domain/
COPY BakeSale.Infrastructure/*.csproj ./BakeSale.Infrastructure/
COPY BakeSale.Tests/*.csproj ./BakeSale.Tests/
COPY ClientApp/clientapp.client/*.esproj ./ClientApp/clientapp.client/

RUN dotnet restore

COPY BakeSale.API/ ./BakeSale.API/
COPY BakeSale.API/Config/Images /app/Config/Images
COPY BakeSale.API/Resources /app/Resources
COPY BakeSale.Domain/ ./BakeSale.Domain/
COPY BakeSale.Infrastructure/ ./BakeSale.Infrastructure/
COPY BakeSale.Tests/ ./BakeSale.Tests/
COPY ClientApp/clientapp.client/ ./ClientApp/clientapp.client/

WORKDIR /app/BakeSale.API
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/BakeSale.API/out ./

ENV ASPNETCORE_URLS=http://+:7190
ENV ConnectionStrings__DefaultConnection="Host=db;Database=bakesale;Username=postgres;Password=postgres"

EXPOSE 7190

ENTRYPOINT ["dotnet", "API.dll"]