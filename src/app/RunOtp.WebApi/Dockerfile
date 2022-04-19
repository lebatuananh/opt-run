FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/app/RunOtp.WebApi/RunOtp.WebApi.csproj", "src/app/RunOtp.WebApi/"]
RUN dotnet restore "src/app/RunOtp.WebApi/RunOtp.WebApi.csproj"
COPY . .
WORKDIR "/src/src/app/RunOtp.WebApi"
RUN dotnet build "RunOtp.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RunOtp.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RunOtp.WebApi.dll"]
