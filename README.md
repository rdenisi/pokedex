# Pokedex API
This is a sample API that provides some informations about pokemon.  
The informations are retrieved from 2 external services, one that provide all the information about a pokemon and one that apply a funny translation to its description.

- **PokeAPI** - https://pokeapi.co/
- **FunTranslations API** - https://funtranslations.com/

## Prerequisites

This project use Net 8, you can download the runtime and the sdk from the official page:
https://dotnet.microsoft.com/en-us/download/dotnet/8.0

## Solution structure

The solution contains a WebAPI project and a Unit Test project:

- **Pokedex.API**
- **Poxedex.Tests**

## API 

- **GET /api/pokemon/{pokemonName}**   
  Retrieves the requested pokemon name informations.

- **GET /api/pokemon/translated/{pokemonName}**  
  Retrieves the requested pokemon name informations, but with a "fun translation" of its description.   
  The rules are:
  - If the Pokemon’s habitat is cave or it’s a legendary Pokemon then apply the Yoda translation.
  - For all other Pokemon, apply the Shakespeare translation.
  - If you can’t translate the Pokemon’s description (for whatever reason 😉) then use the standard description

## Tests

Tests are written with the **XUnit** library, see https://www.nuget.org/packages/xunit.

## How to run

You can run the application from an IDE like Visual Studio, run the app with play and navigate to the swagger url, the app is available from the http and https url:  
- http://localhost:5272/swagger/index.html  
- https://localhost:7087/swagger/index.html

To run the application from command line, go to the Pokedex.API project folder and execute "dotnet run", then you can open a browser and navigate to the swagger url.

![image](https://github.com/rdenisi/pokedex/assets/5156034/c46da38c-d50b-4884-939c-c9b0afee3a9e)

![image](https://github.com/rdenisi/pokedex/assets/5156034/83d514e8-d89b-4cb4-a7d3-7eaa5d583324)

You can run the unit tests from an IDE like Visual Studio, open the test explorer and press "Run All Tests" to execute all the tests.

![image](https://github.com/rdenisi/pokedex/assets/5156034/dd32b12c-3792-49dc-9392-3e4940e24f66)

To run the unit tests from command line, go to the Pokedex.Tests project folder and execute "dotnet test".

![image](https://github.com/rdenisi/pokedex/assets/5156034/9b0d5109-3135-4733-a55c-6974987efd3c)
