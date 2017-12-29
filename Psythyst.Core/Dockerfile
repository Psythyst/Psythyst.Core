FROM microsoft/dotnet:sdk AS Build
WORKDIR /Psythyst.Core
COPY ./ ./
RUN dotnet restore
RUN dotnet publish -c Release -o ./Publish

WORKDIR /Psythyst
COPY ./ ../Psythyst/Psythyst
RUN dotnet restore
RUN dotnet publish -c Release -o ./Publish