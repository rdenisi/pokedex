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

		[HttpGet]
		[Route("{pokemonName}")]
		public async Task<IActionResult> GetPokemonInfo(string pokemonName)
		{
			logger.LogInformation("Called with pokemon name {pokemonName}", pokemonName);
			PokemonDto dto = await pokemonService.GetPokemonAsync(pokemonName);
			return Ok(dto);
		}

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
