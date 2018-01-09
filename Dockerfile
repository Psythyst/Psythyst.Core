FROM microsoft/dotnet:sdk AS Build

# Build Psythyst
WORKDIR /
COPY ./ ./
RUN dotnet restore
RUN dotnet publish -c Release -o ./Publish