using System.Text.Json.Serialization;

namespace Pokedex.Dtos
{
	public class PokemonDto
	{
		public string Name { get; set; }

		public string Description { get; set; }

		public string Habitat { get; set; }

		[JsonPropertyName("is_legendary")]
		public bool IsLegendary { get; set; }
	}
}
