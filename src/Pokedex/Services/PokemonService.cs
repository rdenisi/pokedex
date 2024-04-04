using Pokedex.Dtos;
using Pokedex.Enums;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Pokedex.Services
{
    public class PokemonService(ILogger<PokemonService> logger, IHttpClientFactory httpClientFactory) : IPokemonService
    {
        private readonly ILogger<PokemonService> logger = logger;
        private readonly IHttpClientFactory httpClientFactory = httpClientFactory;

        private readonly JsonSerializerOptions jsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public async Task<PokemonDto> GetPokemonAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name), $"{nameof(name)} cannot be empty.");
            }

            PokemonApiResponse pokemonApiResponse = await GetPokemonFromApi(name);
            PokemonDto pokemonDto = ApiResponseToDto(pokemonApiResponse);
            return pokemonDto;
        }

        public async Task<PokemonDto> TranslateAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name), $"{nameof(name)} cannot be empty.");
            }

            PokemonApiResponse pokemonApiResponse = await GetPokemonFromApi(name);

            PokemonDto pokemonDto = ApiResponseToDto(pokemonApiResponse);

            string translationType = TranslationType.Shakespeare.ToString().ToLower();

            if (string.Equals(pokemonDto.Habitat, Enums.Habitat.Cave.ToString(), StringComparison.CurrentCultureIgnoreCase) || pokemonDto.IsLegendary)
            {
                translationType = TranslationType.Yoda.ToString().ToLower();
            }

            string cleanedDescription = CleanText(pokemonDto.Description ?? string.Empty);
            FunTranslationsResponse funTranslationsResponse = await GetTranslationFromApi(translationType, cleanedDescription);
            if (funTranslationsResponse is not null && funTranslationsResponse.Success.Total == 1)
            {
                pokemonDto.Description = funTranslationsResponse.Contents.Translated;
            }

            return pokemonDto;
        }

        private async Task<PokemonApiResponse> GetPokemonFromApi(string name)
        {
            var apiUri = $"https://pokeapi.co/api/v2/pokemon-species/{name}";

            var httpClient = httpClientFactory.CreateClient("PokeApi");
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
            try
            {
                var apiUri = $"https://api.funtranslations.com/translate/{translationType}.json?text={text}";

                var httpClient = httpClientFactory.CreateClient("FunTranslations");
                var httpResponse = await httpClient.GetAsync(apiUri);

                if (!httpResponse.IsSuccessStatusCode)
                {
                    var error = await httpResponse.Content.ReadAsStringAsync();
                    throw new Exception($"The response from funtranslations service is in error: [{error}]");
                }

                var apiResponseRaw = await httpResponse.Content.ReadAsStringAsync();

                var apiResponseDto = JsonSerializer.Deserialize<FunTranslationsResponse>(apiResponseRaw, jsonSerializerOptions);
                return apiResponseDto;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception message:{exceptionMessage}", ex.Message);
                return null;
            }
        }

        public static PokemonDto ApiResponseToDto(PokemonApiResponse pokemonApiResponse)
        {
            return new PokemonDto
            {
                Name = pokemonApiResponse.Name,
                Description = pokemonApiResponse.FlavorTextEntries?.FirstOrDefault(e => string.Equals(e.Language?.Name, Enums.Language.En.ToString(), StringComparison.CurrentCultureIgnoreCase))?.FlavorText,
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
