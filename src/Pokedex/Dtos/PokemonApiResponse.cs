using System.Text.Json.Serialization;

namespace Pokedex.Dtos
{
	public class PokemonApiResponse
	{
		public string Name { get; set; }

		[JsonPropertyName("flavor_text_entries")]
		public List<FlavorTextEntry> FlavorTextEntries { get; set; }

		public Habitat Habitat { get; set; }

		[JsonPropertyName("is_legendary")]
		public bool IsLegendary { get; set; }
	}

	public class FlavorTextEntry
	{
		[JsonPropertyName("flavor_text")]
		public string FlavorText { get; set; }

		public Language Language { get; set; }
	}

	public class Language
	{
		public string Name { get; set; }

		public string Url { get; set; }
	}

	public class Habitat
	{
		public string Name { get; set; }

		public string Url { get; set; }
	}
}
