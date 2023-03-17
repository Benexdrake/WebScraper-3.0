using Webscraper.API.Interfaces;

namespace Webscraper.API.Scraper.Pokemons.Controller
{
    public class Pokemon_API : IPokemon_API
    {
        private readonly Browser _browser;
        public Pokemon_API(IServiceProvider service)
        {
            _browser = service.GetRequiredService<Browser>();
        }
        public async Task<List<Pokemon>> GetPokemonByIDAsync(int nr)
        {
            var doc = _browser.GetPageDocument($"https://www.pokemon.com/de/pokedex/{nr}", 2000).Result;
            var main = FindNodesByDocument(doc, "div", "class", "pokedex").Result.FirstOrDefault();
            if (main is not null)
            {
                var details = FindNodesByNode(main, "section", "class", "pokemon-details").Result.FirstOrDefault();
                if (details is not null)
                    return GetPokemons(main, nr);
            }
            return null;
        }

        private List<Pokemon> GetPokemons(HtmlNode node, int nr, int index = 0)
        {
            MainBuilder b = new MainBuilder();

            List<Pokemon> pokemons = new List<Pokemon>();
            var status = FindNodesByNode(node, "div", "class", "pokemon-stats").Result;
            var version = FindNodesByNode(node, "div", "class", "version-descriptions").Result;
            var abilityInfos = FindNodesByNode(node, "div", "class", "pokemon-ability-info color-bg color-lightblue").Result;
            var attributeList = FindNodesByNode(abilityInfos[index], "span", "class", "attribute-value").Result;
            var types = FindNodesByNode(node, "div", "class", "pokedex-pokemon-attributes").Result;

            var profileImages = FindNodesByNode(node, "div", "class", "profile-images").Result.FirstOrDefault();
            var images = FindNodesByNode(profileImages, "img", "class", "").Result;


            bool isVersion = false;
            int versionCounts = 1;
            var pokemonVersions = FindNodesByNode(node, "div", "class", "custom-select-menu").Result.FirstOrDefault();
            if (pokemonVersions != null)
            {
                var versions = FindNodesByNode(pokemonVersions, "li", "class", "").Result;
                versionCounts = versions.Count;

            }

            for (int i = 0; i < versionCounts; i++)
            {
                if (i > 0)
                    isVersion = true;
                string _id = $"{nr}-{i + 1}";

                var skills = GetPokemonSkills(abilityInfos[i]);
                var statusList = FindNodesByNode(status[i], "li", "class", "meter").Result;
                Pokemon p = b
                .p
                .NewPokemon()
                .ID(_id)
                .Nr(nr)
                .Name(GetPokemonName(images[i], node, i))
                .Url($"https://www.pokemon.com/de/pokedex/{nr}")
                .Image(GetPokemonImageUrl(images[i], i))
                .SkillName(GetSkillName(skills))
                .SkillDescription(eGetSkillDescription(skills))
                .Description(GetPokemonDescription(version[i]))
                .Size(GetPokemonSize(attributeList[0]))
                .Weight(GetPokemonWeight(attributeList[1]))
                .Sex(GetPokemonSex(attributeList[2]))
                .Category(GetPokemonCategory(attributeList[3]))
                .Type(GetPokemonType(types[i]))
                .Weakness(GetPokemonWeakness(types[i]))
                .KP(GetStatusKP(statusList[0]))
                .Attack(GetStatusAttack(statusList[1]))
                .Defensiv(GetStatusDefensiv(statusList[2]))
                .SPAttack(GetStatusSPAttack(statusList[3]))
                .SPDefensiv(GetStatusSPDefensive(statusList[4]))
                .Initiative(GetStatusInitiative(statusList[5]))
                .IsVersion(isVersion)
                .GetPokemon();
                pokemons.Add(p);
            }
            return pokemons;
        }



        #region Private Get Methods for Pokemon

        private string GetPokemonImageUrl(HtmlNode node, int i)
        {
            var split = node.OuterHtml.Split('"');
            if (i == 0)
                return split[3];
            return split[1];
        }

        private string GetPokemonName(HtmlNode node, HtmlNode node2, int index)
        {
            var name = FindNodesByNode(node2, "div", "class", "pokedex-pokemon-pagination-title").Result.FirstOrDefault().InnerText.Split(' ')[10].Replace("\n", "").Replace("\r", "").Trim();
            if (index > 0 || !node.OuterHtml.Contains(name))
            {
                var split = node.OuterHtml.Split('"');
                for (int i = 0; i < split.Length; i++)
                {
                    if (split[i].Contains("alt"))
                        return $"{name} - {split[i + 1]}";
                }
            }
            return name;
        }

        private string GetPokemonDescription(HtmlNode node)
        {
            var descriptionList = FindNodesByNode(node, "p", "class", "version").Result;

            string description = string.Empty;
            foreach (var desc in descriptionList)
            {
                description += desc.InnerText.Replace("\n", "").Replace("\r", "").Replace("                  ", "").Replace("                ", "") + Environment.NewLine;
            }

            description = description.Substring(0, description.Length - 2);

            return description;
        }

        private string GetPokemonSize(HtmlNode node)
        {
            return node.InnerText;
        }

        private string GetPokemonWeight(HtmlNode node)
        {
            return node.InnerText;
        }

        private string GetPokemonCategory(HtmlNode node)
        {
            return node.InnerText;
        }

        private Skill[] GetPokemonSkills(HtmlNode node)
        {
            List<Skill> list = new();

            var skillsList = FindNodesByNode(node, "div", "class", "pokemon-ability-info-detail").Result;

            string skills = string.Empty;
            for (int i = 0; i < skillsList.Count; i++)
            {
                var name = FindNodesByNode(skillsList[i], "h3", "", "").Result.FirstOrDefault().InnerText;
                var desc = FindNodesByNode(skillsList[i], "p", "", "").Result.FirstOrDefault().InnerText;

                list.Add(new Skill()
                {
                    Name = name,
                    Description = desc
                });
            }

            return list.ToArray();
        }

        private string GetSkillName(Skill[] skills)
        {
            string name = string.Empty;
            for (int i = 0; i < skills.Length; i++)
            {
                if (i < skills.Length - 1)
                    name += skills[i].Name + ";";
                else
                    name += skills[i].Name;
            }
            return name;
        }

        private string eGetSkillDescription(Skill[] skills)
        {
            string desc = string.Empty;
            for (int i = 0; i < skills.Length; i++)
            {
                if (i < skills.Length - 1)
                    desc += skills[i].Description + ";";
                else
                    desc += skills[i].Description;
            }
            return desc;
        }

        private string GetPokemonSex(HtmlNode node)
        {
            string sex = string.Empty;
            var male = FindNodesByNode(node, "i", "class", "icon_male_symbol").Result.FirstOrDefault();
            var female = FindNodesByNode(node, "i", "class", "icon_female_symbol").Result.FirstOrDefault();
            if (male != null)
                sex += "♂";
            if (female != null)
                sex += "♀";
            if (male != null && female != null)
                sex = "⚥";
            if (male == null && female == null)
                sex = "???";

            return sex;
        }

        private string GetPokemonType(HtmlNode node)
        {
            var t = FindNodesByNode(node, "div", "class", "dtm-type").Result.FirstOrDefault();
            var typs = FindNodesByNode(t, "li", "class", "background-color").Result;

            string typ = string.Empty;

            for (int i = 0; i < typs.Count; i++)
            {
                if (i < typs.Count - 1)
                    typ += typs[i].InnerText.Trim() + ", ";
                else
                    typ += typs[i].InnerText.Trim();
            }
            return typ;
        }

        private string GetPokemonWeakness(HtmlNode node)
        {
            var w = FindNodesByNode(node, "div", "class", "dtm-weaknesses").Result.FirstOrDefault();
            var weakness = FindNodesByNode(w, "li", "class", "background-color").Result;

            string weak = string.Empty;

            for (int i = 0; i < weakness.Count; i++)
            {
                if (i < weakness.Count - 1)
                    weak += weakness[i].InnerText.Trim() + ", ";
                else
                    weak += weakness[i].InnerText.Trim();
            }

            return weak;
        }

        private int GetStatusKP(HtmlNode node)
        {
            return int.Parse(node.OuterHtml.Split('"')[1]);
        }
        private int GetStatusAttack(HtmlNode node)
        {
            return int.Parse(node.OuterHtml.Split('"')[1]);
        }
        private int GetStatusDefensiv(HtmlNode node)
        {
            return int.Parse(node.OuterHtml.Split('"')[1]);
        }
        private int GetStatusSPAttack(HtmlNode node)
        {
            return int.Parse(node.OuterHtml.Split('"')[1]);
        }
        private int GetStatusSPDefensive(HtmlNode node)
        {
            return int.Parse(node.OuterHtml.Split('"')[1]);
        }
        private int GetStatusInitiative(HtmlNode node)
        {
            return int.Parse(node.OuterHtml.Split('"')[1]);
        }
        #endregion

        #region Private Nodes

        private async Task<List<HtmlNode>> FindNodesByNode(HtmlNode node, string a, string b, string c)
        {
            return node.Descendants(a).Where(node => node.GetAttributeValue(b, "").Contains(c)).ToList();
        }

        private async Task<List<HtmlNode>> FindNodesByDocument(HtmlDocument document, string a, string b, string c)
        {
            return document.DocumentNode.Descendants(a).Where(node => node.GetAttributeValue(b, "").Contains(c)).ToList();
        }

        #endregion
    }
}
