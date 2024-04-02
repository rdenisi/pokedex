using Microsoft.AspNetCore.Mvc;
using Pokedex.Dtos;
using Pokedex.Services;

namespace Pokedex.Controllers
{
	[Route("api/pokemon")]
	[ApiController]
	public class PokemonController(ILogger<PokemonController> logger, IPokemonService pokemonService) : ControllerBase
	{
		private readonly ILogger<PokemonController> logger = logger;
		private readonly IPokemonService pokemonService = pokemonService;

        /// <summary>
        /// Retrieves the requested pokemon name informations.
        /// </summary>
        /// <param name="pokemonName">The name of the pokemon.</param>
        /// <returns>Returns the informations of the pokemon.</returns>
        [HttpGet]
		[Route("{pokemonName}")]
		public async Task<IActionResult> GetPokemonInfo(string pokemonName)
		{
			logger.LogInformation("Called with pokemon name {pokemonName}", pokemonName);
			PokemonDto dto = await pokemonService.GetPokemonAsync(pokemonName);
			return Ok(dto);
		}

        /// <summary>
        /// Retrieves the requested pokemon name information, but with a "fun translation" of its description.
        /// </summary>
        /// <param name="pokemonName">The name of the pokemon.</param>
        /// <returns>Returns the informations of the pokemon.</returns>
        [HttpGet]
		[Route("translated/{pokemonName}")]
		public async Task<IActionResult> GetTranslatedPokemonInfo(string pokemonName)
		{
			logger.LogInformation("Called with pokemon name {pokemonName}", pokemonName);
			PokemonDto dto = await pokemonService.TranslateAsync(pokemonName);
			return Ok(dto);
		}
	}
}
