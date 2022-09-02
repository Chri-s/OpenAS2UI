FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
RUN dotnet tool install -g Microsoft.Web.LibraryManager.Cli
ENV PATH="$PATH:/root/.dotnet/tools"
WORKDIR /app
#Copy and restore everything under it
COPY . ./
RUN dotnet restore
WORKDIR "src/Client/."
RUN libman restore
WORKDIR "/app"
#Publish to out directory
RUN dotnet publish -c Release -o out

#Prepare an image for execution
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/out .
EXPOSE 80
ENTRYPOINT ["dotnet", "OpenAS2UI.Server.dll"]