using Pokedex.Dtos;

namespace Pokedex.Services
{
	public interface IPokemonService
	{
		Task<PokemonDto> GetPokemonAsync(string name);
		Task<PokemonDto> TranslateAsync(string name);
	}
}
