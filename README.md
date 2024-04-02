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

Open a browser and navigate to the swagger url:

![image](https://github.com/rdenisi/pokedex/assets/5156034/83d514e8-d89b-4cb4-a7d3-7eaa5d583324)

