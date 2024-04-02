# Pokedex API
This is a sample API that provide some information about pokemon.  
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
  Retrieves the requested pokemon name information, but with a "fun translation" of its description.

## Tests

Tests are written with the **XUnit** library, see https://www.nuget.org/packages/xunit.

## How to run

Open a browser and navigate to the swagger url:

![image](https://github.com/rdenisi/pokedex/assets/5156034/ebdd9198-1b83-4e1e-bb29-e742263af01d)
