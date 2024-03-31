using Pokedex.Dtos;
using Pokedex.Enums;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Pokedex.Services
{
	public class PokemonService(IHttpClientFactory httpClientFactory) : IPokemonService
	{
		private readonly IHttpClientFactory httpClientFactory = httpClientFactory;

		private readonly JsonSerializerOptions jsonSerializerOptions = new()
		{
			PropertyNameCaseInsensitive = true
		};

		public async Task<PokemonDto> GetPokemonAsync(string name)
		{
			PokemonApiResponse pokemonApiResponse = await GetPokemonFromApi(name);
			PokemonDto pokemonDto = ApiResponseToDto(pokemonApiResponse);
			return pokemonDto;
		}

		public async Task<PokemonDto> TranslateAsync(string name)
		{
			PokemonApiResponse pokemonApiResponse = await GetPokemonFromApi(name);

			PokemonDto pokemonDto = ApiResponseToDto(pokemonApiResponse);

			string translationType = TranslationType.Shakespeare.ToString().ToLower();

			if (pokemonApiResponse.Habitat.Name.Equals(Enums.Habitat.Cave.ToString(), StringComparison.CurrentCultureIgnoreCase) || pokemonApiResponse.IsLegendary)
			{
				translationType = TranslationType.Yoda.ToString().ToLower();
			}

			string cleanedDescription = CleanText(pokemonDto.Description);
			FunTranslationsResponse funTranslationsResponse = await GetTranslationFromApi(translationType, cleanedDescription);
			if (funTranslationsResponse.Success.Total == 1)
			{
				pokemonDto.Description = funTranslationsResponse.Contents.Translated;
			}

			return pokemonDto;
		}

		private async Task<PokemonApiResponse> GetPokemonFromApi(string name)
		{
			var apiUri = $"https://pokeapi.co/api/v2/pokemon-species/{name}";

			var httpClient = httpClientFactory.CreateClient();
			var httpResponse = await httpClient.GetAsync(apiUri);

			if (!httpResponse.IsSuccessStatusCode)
			{
				if (httpResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
				{
					throw new KeyNotFoundException($"The given key [{name}] was not found calling the pokeapi service.");
				}

				var error = await httpResponse.Content.ReadAsStringAsync();
				throw new Exception($"An error occured calling the pokeapi service: [{error}]");
			}

			var apiResponseRaw = await httpResponse.Content.ReadAsStringAsync();

			var apiResponseDto = JsonSerializer.Deserialize<PokemonApiResponse>(apiResponseRaw, jsonSerializerOptions);
			return apiResponseDto;
		}

		private async Task<FunTranslationsResponse> GetTranslationFromApi(string translationType, string text)
		{
			var apiUri = $"https://api.funtranslations.com/translate/{translationType}.json?text={text}";

			var httpClient = httpClientFactory.CreateClient();
			var httpResponse = await httpClient.GetAsync(apiUri);

			if (!httpResponse.IsSuccessStatusCode)
			{
				var error = await httpResponse.Content.ReadAsStringAsync();
				throw new Exception($"An error occured calling the pokeapi service: [{error}]");
			}

			var apiResponseRaw = await httpResponse.Content.ReadAsStringAsync();

			var apiResponseDto = JsonSerializer.Deserialize<FunTranslationsResponse>(apiResponseRaw, jsonSerializerOptions);
			return apiResponseDto;
		}

		private static PokemonDto ApiResponseToDto(PokemonApiResponse pokemonApiResponse)
		{
			return new PokemonDto
			{
				Name = pokemonApiResponse.Name,
				Description = pokemonApiResponse.FlavorTextEntries?.FirstOrDefault(e => (e.Language?.Name).Equals(Enums.Language.En.ToString(), StringComparison.CurrentCultureIgnoreCase)).FlavorText,
				Habitat = pokemonApiResponse.Habitat?.Name,
				IsLegendary = pokemonApiResponse.IsLegendary
			};
		}

		private static string CleanText(string text)
		{
			return Regex.Replace(text, @"\n|\f", " ");
		}
	}
}
