using Pokedex.Dtos;
using System.Text.Json;

namespace Pokedex.Services
{
	public class PokemonService(IHttpClientFactory httpClientFactory) : IPokemonService
	{
		private readonly IHttpClientFactory httpClientFactory = httpClientFactory;

		public async Task<PokemonDto> GetPokemonAsync(string name)
		{
			var httpClient = httpClientFactory.CreateClient();
			var httpResponse = await httpClient.GetAsync("https://pokeapi.co/api/v2/pokemon-species/mewtwo");

			if (httpResponse.StatusCode != System.Net.HttpStatusCode.OK)
			{
				throw new Exception("Ërror calling service");
			}

			var result = await httpResponse.Content.ReadAsStringAsync();
			var dto = JsonSerializer.Deserialize<PokemonDto>(result, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
			return dto;
		}

		public Task<PokemonDto> TranslateAsync(string name)
		{
			var dto = new PokemonDto
			{
				Name = Reverse(name)
			};
			return Task.FromResult(dto);
		}

		public static string Reverse(string s)
		{
			char[] charArray = s.ToCharArray();
			Array.Reverse(charArray);
			return new string(charArray);
		}
	}
}
