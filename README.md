# Hello! 👋
# This is Heinekamp coding challenge solution

It comprises <a href="./applications/back-end">back-end</a> and 
<a href="./applications/front-end">front-end</a>. Both can be found in `applications` folder of 
`heinekamp-coding-challenge`

# Quick start

## Prerequisites
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

<img width="1291" alt="image" src="https://github.com/stsiazhkin/heinekamp-coding-challenge/assets/22170119/9a6ef69d-fba0-40c7-b0d3-47e923286cc9">


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

**To run unit and integration tests of the API** please perform the following command from a terminal while in `./applications/back-end` folder:
````
dotnet test
````
**Docker engine must be running in the background**


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

![potential_prod_implementation](https://github.com/stsiazhkin/heinekamp-coding-challenge/assets/22170119/5c1e6709-5922-46c8-9127-85bd146398c9)

![Coding_challenge_implementation](https://github.com/stsiazhkin/heinekamp-coding-challenge/assets/22170119/837b87ad-5fe5-4272-be35-d0f01b287577)

![uplaod_flow](https://github.com/stsiazhkin/heinekamp-coding-challenge/assets/22170119/e9b3f30b-776b-4c81-998e-1e62811025b5)

![fetch_filesinfo](https://github.com/stsiazhkin/heinekamp-coding-challenge/assets/22170119/13020de8-3066-4658-8923-e8b81bb37a1b)

![download_file_flow](https://github.com/stsiazhkin/heinekamp-coding-challenge/assets/22170119/60f62b07-68b7-4815-8d28-059d5291984e)

![shareable_link_flow](https://github.com/stsiazhkin/heinekamp-coding-challenge/assets/22170119/71ea68a8-ad43-4522-becc-e954f31c9b1d)

# What else has to be added
## backend wise
* Extend unit/integration tests coverage
* Add caching layer to reduce network load and improve response times
* Implement health checks so that loadbalancer can properly track issues and spread the workload across instances of API
* Move files archivation logic bit outside of API considering this is a heavy load operation, make it asyncronous
* Have Image Thumbnails logic separate and being done on BE side only. Front end only does representation for this
* Have a back end for frontend (BFF) for authentication logic (and whatever else is needed). Then File API itself should be an internal service which is not exposed to the outside
* File API can be used by any service in the company which needs files download/upload support

## Frontend wise
* Make it look nicer for sure
* Add pagination on front end
* Add an option to set download link expiratioin time manually. Currently it is done on BE only and set to 60 mins in config file of the API
* The rest is better to discuss with a Front-End engineer 😊






