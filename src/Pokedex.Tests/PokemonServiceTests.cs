using Moq;
using Pokedex.Dtos;
using Pokedex.Services;
using RichardSzalay.MockHttp;
using System.Net;
using System.Net.Http.Json;

namespace Pokedex.Tests
{
	public class PokemonServiceTests
	{
		private readonly PokemonService pokemonService;
		private readonly MockHttpMessageHandler handlerMock = new();
		private readonly Mock<IHttpClientFactory> httpClientFactoryMock = new();
		private readonly string pokeapiBaseUrl = "https://pokeapi.co/api/v2/pokemon-species";
		private readonly string pokeApiClientName = "PokeApi";
		private readonly string funTranslationsClientName = "FunTranslations";


		public PokemonServiceTests()
		{
			pokemonService = new PokemonService(httpClientFactoryMock.Object);
		}

		[Fact]
		public async Task GetPokemonAsync_WhenArgumentIsNullOrWhiteSpace_ShouldThrowArgumentNullException()
		{
			//Arrange
			string? nullName = null;

			//Act
			//Assert
			await Assert.ThrowsAsync<ArgumentNullException>(() => pokemonService.GetPokemonAsync(nullName));
		}

		[Fact]
		public async Task GetPokemonAsync_WhenNameIsNotFound_ShouldThrowKeyNotFoundException()
		{
			//Arrange
			var notExistentName = Guid.NewGuid().ToString();
			var url = $"{pokeapiBaseUrl}/{notExistentName}";

			handlerMock
				.When(url)
				.Respond(HttpStatusCode.NotFound, JsonContent.Create(new { }));

			httpClientFactoryMock.Setup(x => x.CreateClient(pokeApiClientName))
				.Returns(new HttpClient(handlerMock));

			//Act
			//Assert
			await Assert.ThrowsAsync<KeyNotFoundException>(() => pokemonService.GetPokemonAsync(notExistentName));
		}

		[Fact]
		public async Task GetPokemonAsync_WhenNameExists_ShouldReturnAValidPokemon()
		{
			//Arrange
			var existentName = "existentPokemonName";
			var url = $"{pokeapiBaseUrl}/{existentName}";

			handlerMock
				.When(url)
				.Respond(HttpStatusCode.OK, JsonContent.Create(GetFakePokemonApiResponse(existentName, string.Empty)));

			httpClientFactoryMock.Setup(x => x.CreateClient(pokeApiClientName))
				.Returns(new HttpClient(handlerMock));

			//Act
			var result = await pokemonService.GetPokemonAsync(existentName);

			//Assert
			Assert.True(result is not null);
			Assert.False(string.IsNullOrEmpty(result.Name));
		}

		[Fact]
		public async Task TranslateAsync_WhenArgumentIsNullOrWhiteSpace_ShouldThrowArgumentNullException()
		{
			//Arrange
			string? nullName = null;

			//Act
			//Assert
			await Assert.ThrowsAsync<ArgumentNullException>(() => pokemonService.TranslateAsync(nullName));
		}

		[Fact]
		public async Task TranslateAsync_WhenApiResponseIsNoSuccessCode_ShouldThrowAnExceptionWithTheErrorMessage()
		{
			//Arrange
			var existentName = "existentPokemonName";
			var description = "Pokemon description";
			var pokeApiUrl = $"{pokeapiBaseUrl}/{existentName}";

			var translationType = "yoda";
			var text = description;
			var funTranslationsUrl = $"https://api.funtranslations.com/translate/{translationType}.json?text={text}";
			var jsonContent = JsonContent.Create(new { error = "An error occured!" });
			var error = await jsonContent.ReadAsStringAsync();
			var expectedExceptionMessage = $"An error occured calling the pokeapi service: [{error}]";

			handlerMock
				.When(pokeApiUrl)
				.Respond(HttpStatusCode.OK, JsonContent.Create(GetFakePokemonApiResponse(existentName, description))); ;

			handlerMock
				.When(funTranslationsUrl)
				.Respond(HttpStatusCode.InternalServerError, jsonContent);

			httpClientFactoryMock.Setup(x => x.CreateClient(pokeApiClientName))
				.Returns(new HttpClient(handlerMock));

			httpClientFactoryMock.Setup(x => x.CreateClient(funTranslationsClientName))
				.Returns(new HttpClient(handlerMock));

			//Act
			//Assert
			var exception = await Assert.ThrowsAsync<Exception>(() => pokemonService.TranslateAsync(existentName));
			Assert.Equal(expectedExceptionMessage, exception.Message);

		}

		[Fact]
		public async Task TranslateAsync_WhenApiResponseIsSuccessCode_ShouldReturnTranslatedPokemonDescription()
		{
			//Arrange
			var existentName = "existentPokemonName";
			var description = "Pokemon description";
			var descriptionTranlsated = "Pokemon description translated";
			var pokeApiUrl = $"{pokeapiBaseUrl}/{existentName}";

			var translationType = "yoda";
			var funTranslationsUrl = $"https://api.funtranslations.com/translate/{translationType}.json?text={description}";
			var jsonContent = JsonContent.Create(GetFakeFunTranslationsApiResponse(description, descriptionTranlsated));

			handlerMock
				.When(pokeApiUrl)
				.Respond(HttpStatusCode.OK, JsonContent.Create(GetFakePokemonApiResponse(existentName, description))); ;

			handlerMock
				.When(funTranslationsUrl)
				.Respond(HttpStatusCode.OK, jsonContent);

			httpClientFactoryMock.Setup(x => x.CreateClient(pokeApiClientName))
				.Returns(new HttpClient(handlerMock));

			httpClientFactoryMock.Setup(x => x.CreateClient(funTranslationsClientName))
				.Returns(new HttpClient(handlerMock));

			//Act
			var result = await pokemonService.TranslateAsync(existentName);

			//Assert
			Assert.Equal(result.Name, existentName);
			Assert.Equal(result.Description, descriptionTranlsated);
			Assert.NotEqual(result.Description, description);
		}

		private static PokemonApiResponse GetFakePokemonApiResponse(string name, string description)
		{
			return new PokemonApiResponse
			{
				Name = name,
				FlavorTextEntries = [new FlavorTextEntry { FlavorText = description, Language = new Language { Name = Enums.Language.En.ToString() } }],
				Habitat = new Habitat { Name = Enums.Habitat.Cave.ToString() },
				IsLegendary = true,
			};
		}

		private static FunTranslationsResponse GetFakeFunTranslationsApiResponse(string description, string descriptionTranslated)
		{
			return new FunTranslationsResponse
			{
				Success = new Success { Total = 1 },
				Contents = new Contents { Text = description, Translated = descriptionTranslated }
			};
		}
	}
}