FROM mcr.microsoft.com/dotnet/sdk:8.0 as build

WORKDIR /src

COPY . /src
COPY ../Heinekamp.CodingChallenge.FileApi.Common/ /Heinekamp.CodingChallenge.FileApi.Common
COPY ../Heinekamp.CodingChallenge.FileApi.Handlers /Heinekamp.CodingChallenge.FileApi.Handlers
COPY ../Heinekamp.CodingChallenge.FileApi.Persistence /Heinekamp.CodingChallenge.FileApi.Persistence
COPY ../Heinekamp.CodingChallenge.FileApi.Services /Heinekamp.CodingChallenge.FileApi.Services

RUN dotnet publish Heinekamp.CodingChallenge.FileApi -o /app


FROM mcr.microsoft.com/dotnet/aspnet:8.0 as final

WORKDIR /app

COPY --from=build /app .

USER $APP_UID

ENTRYPOINT [ "dotnet", "Heinekamp.CodingChallenge.FileApi.dll" ]