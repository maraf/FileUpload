# Build runtime image
FROM microsoft/dotnet:2.2.1-aspnetcore-runtime-stretch-slim-arm32v7
WORKDIR /app
COPY /bin/Release/docker .
ENTRYPOINT ["dotnet", "FileUpload.Web.UI.dll"]