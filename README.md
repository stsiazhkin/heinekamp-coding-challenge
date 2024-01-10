# Hello! 
# This is Heinekamp coding challenge solution

It comprises <a href="./applications/back-end/README.md">back-end</a> and 
<a href="./applications/front-end/README.md">front-end</a>. Both can be found in `applications` folder of 
`heinekamp-coding-challenge`

# Quick start

## Prerequisites
<a href=""></a></br>
Have to be installed before running applications:

<a href="https://dotnet.microsoft.com/en-us/download/dotnet/8.0">.NET8</a></br>
<a href="https://docs.docker.com/desktop/">Docker engine and docker compose</a></br>
<a href="https://nodejs.org/en/download">node.js</a></br>
<a href="https://docs.npmjs.com/downloading-and-installing-node-js-and-npm">npm</a></br>

## To run `back-end`

while in `./applications/back-end` folder please run in a terminal the following command below:

````
docker compose -f docker-compose.infra.yml up
````
It will spin up back end's infrastructure which is localstack with AWS S3 bucket in it and a Postgres DB

as soon as you can see  `Finished LocalStack services initialisation.` and `database system is ready to accept connections`

please build `Heinekamp.CodingChallenge.FileApi` and run it. 
Please use IDE of your choice to do so or while in `./applications/back-end` folder please run in a terminal the 
following commands below:

````
dotnet build
````
````
dotnet run --project src/Heinekamp.CodingChallenge.FileApi/Heinekamp.CodingChallenge.FileApi.csproj
````

API should be up and running ready to accept requests likely on port `5062` 

To open swagger: 
Open <a href="http://localhost:5062/swagger/index.html">this link</a> in browser please

**In order to send requests to API one has to be authenticated.**
There are two test API keys available to try it out straight away:</br>
`dcbb55abc0c447498a26e1a22f4ec81c` and `d37cc31f6dd2434abd826456ae4b7063` </br> 
they can be found/added/removed in 
`appsettings.Development.json` of `Heinekamp.CodingChallenge.FileApi`. </br> 
**Please bear in mind this only for demo purposes!** No sensitive info must be stored 
in codebase when it comes to production development

## To run `front-end`

while in `./applications/front-end` folder please run in a terminal the following command below:

````
npm install
````
````
npm run start
````
UI spins up on port `3000` should <a href="http://localhost:3000">be available in browser</a>

In order to proceed one needs to enter a valid test API key. They are the same as above and will be used to send requests 
to back end API.

# Overview

Insert image here
Insert image here

