using Webscraper.Models.Pokemons.Models;

namespace Webscraper.Models.Pokemons.Builder
{
    public class PokemonBuilder : MainBuilder
    {
        public PokemonBuilder(Pokemon _pokemon)
        {
            pokemon = _pokemon;
        }

        public PokemonBuilder ID(string id)
        {
            pokemon.Id = id;
            return this;
        }

        public PokemonBuilder Nr(int nr)
        {
            pokemon.Nr = nr;
            return this;
        }

        public PokemonBuilder Url(string url)
        {
            pokemon.Url = url;
            return this;
        }

        public PokemonBuilder Image(string url)
        {
            pokemon.ImageUrl = url;
            return this;
        }

        public PokemonBuilder Name(string name)
        {
            pokemon.Name = name;
            return this;
        }

        public PokemonBuilder Description(string description)
        {
            pokemon.Description = description;
            return this;
        }

        public PokemonBuilder Size(string size)
        {
            pokemon.Size = size;
            return this;
        }

        public PokemonBuilder Weight(string weight)
        {
            pokemon.Weight = weight;
            return this;
        }

        public PokemonBuilder SkillName(string skillName)
        {
            pokemon.SkillName = skillName;
            return this;
        }

        public PokemonBuilder SkillDescription(string skillDescription)
        {
            pokemon.SkillDescription = skillDescription;
            return this;
        }

        public PokemonBuilder Category(string category)
        {
            pokemon.Category = category;
            return this;
        }

        public PokemonBuilder Sex(string sex)
        {
            pokemon.Sex = sex;
            return this;
        }

        public PokemonBuilder Weakness(string weakness)
        {
            pokemon.Weakness = weakness;
            return this;
        }

        public PokemonBuilder Type(string type)
        {
            pokemon.Type = type;
            return this;
        }

        public PokemonBuilder KP(int kp)
        {
            pokemon.KP = kp;
            return this;
        }

        public PokemonBuilder Attack(int attack)
        {
            pokemon.Attack = attack;
            return this;
        }

        public PokemonBuilder Defensiv(int defensiv)
        {
            pokemon.Defensiv = defensiv;
            return this;
        }

        public PokemonBuilder SPAttack(int spAttack)
        {
            pokemon.SPAttack = spAttack;
            return this;
        }

        public PokemonBuilder SPDefensiv(int spDefensiv)
        {
            pokemon.SPDefensiv = spDefensiv;
            return this;
        }

        public PokemonBuilder Initiative(int initiative)
        {
            pokemon.Initiative = initiative;
            return this;
        }

        public PokemonBuilder IsVersion(bool version)
        {
            pokemon.HasVersions = version;
            return this;
        }

        public PokemonBuilder NewPokemon()
        {
            pokemon = new Pokemon();
            return this;
        }

        public Pokemon GetPokemon()
        {
            return pokemon;
        }
    }
}
