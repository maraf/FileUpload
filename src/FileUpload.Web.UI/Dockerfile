FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /app

# Copy csproj and do restore.
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build.
COPY . ./
RUN dotnet publish -c Release -r linux-arm -o /out

# Build final image.
FROM microsoft/dotnet:2.2.1-aspnetcore-runtime-stretch-slim-arm32v7
WORKDIR /out
COPY --from=build /out .
ENTRYPOINT ["dotnet", "FileUpload.Web.UI.dll"]