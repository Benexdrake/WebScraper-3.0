using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webscraper_API.Interfaces;
using Webscraper_API.Scraper.TCG_Pokemon.Models;

namespace Webscraper_API.Scraper.TCG_Pokemon.Controller
{
    public class TCG_API : ITCG_API
    {
        public async Task<int> GetMaxPokemonCards(HtmlDocument doc)
        {
            List<string> pokemonCardsUrls = new List<string>();

            // Finden der Anzahl von Max Seiten.
            var main = FindNodesByDocument(doc, "div", "id", "cards-load-more").Result.FirstOrDefault();
            var span = FindNodesByNode(main, "span", "", "").Result[1].InnerText.Split(" ");

            int max = int.Parse(span[2]);
            return max;
        }

        public async Task<List<string>> GetUrlsFromSite(HtmlDocument doc)
        {
            List<string> pokemonCardsUrls = new List<string>();

            var main = FindNodesByDocument(doc, "ul", "class", "cards-grid clear").Result.FirstOrDefault();
            // Durchlaufen jeder Seite und sammeln der URLs auf jeder Seite.
            var urls = FindNodesByNode(main, "a", "href", "").Result;

            foreach (var u in urls)
            {
                var split = u.OuterHtml.Split('"'); ;
                string newU = "https://www.pokemon.com" + split[1];
                pokemonCardsUrls.Add(newU);
            }
            return pokemonCardsUrls;
        }

        public async Task<PokemonCard> GetPokemonCardAsync(string url, HtmlDocument doc)
        {
            PokemonCard pc = new();

            var main = FindNodesByDocument(doc, "section", "class", "mosaic section card-detail").Result.FirstOrDefault();

            var information = FindNodesByNode(main, "div", "class", "full-card-information").Result.FirstOrDefault();

            var descriptionBlock = FindNodesByNode(information, "div", "class", "card-description").Result.FirstOrDefault();

            var statsBlock = FindNodesByNode(main, "div", "class", "pokemon-stats").Result.FirstOrDefault();

            var urlSplit = url.Split('/');
            // ID
            pc.Id = $"{urlSplit[7]}-{urlSplit[8]}";
            // Url
            pc.Url = url;
            // Image Url
            pc.ImageUrl = FindNodesByNode(main, "img", "src", "assets").Result.FirstOrDefault().OuterHtml.Split('"')[1]; ;
            // Name
            var nameBlock = FindNodesByNode(information, "div", "class", "color-block color-block-gray").Result.FirstOrDefault();
            var name = nameBlock.InnerText.Trim();
            pc.Name = name;
            // Card Type
            var cardType = FindNodesByNode(descriptionBlock, "div", "class", "card-type").Result.FirstOrDefault();
            var ct = FindNodesByNode(cardType, "h2").Result.FirstOrDefault();
            pc.CardType = ct.InnerText;

            //KP
            var kpBlock = FindNodesByNode(descriptionBlock, "span", "class", "card-hp").Result.FirstOrDefault();
            if (kpBlock != null)
            {
                //tryparse kp none
                var kp = kpBlock.InnerText.Remove(0, 2);
                pc.KP = kp;
            }
            // Element
            var energyIcon = FindNodesByNode(descriptionBlock, "i", "class", "energy").Result.FirstOrDefault();
            var es = energyIcon.OuterHtml.Split('"')[1].Split("-")[1];
            pc.Element = UpperLetter(es);


            //Weakness
            var energyBlock = FindNodesByNode(statsBlock, "div", "class", "stat").Result;
            if (energyBlock.Count > 1)
            {
                var eSplit = energyBlock[0].OuterHtml.Split('"');
                if (eSplit.Length > 12)
                {
                    var eIcon = UpperLetter(eSplit[13].Split("-")[1]);
                    var eCounter = eSplit[14].Split(" ")[56].Trim();
                    pc.Weakness = $"{eIcon} {eCounter}";
                }

                //Resistance
                var resistanceBlock = energyBlock[1].OuterHtml.Split('"');
                if (resistanceBlock.Length > 3)
                {
                    var rSplit = UpperLetter(resistanceBlock[13].Split("-")[1]);
                    var rCounter = resistanceBlock[14].Split(" ")[56].Trim();
                    pc.Resistance = $"{rSplit} {rCounter}";
                }
                // Retreat
                var retreatBlock = energyBlock[2].OuterHtml.Split('"');
                if (retreatBlock.Length > 3)
                {
                    // Knallt weil findet Liste von Colorless Icons, muss nach Anzahl suchen und jedes angeben
                    var retreatEnergy = FindNodesByNode(energyBlock[2], "i", "class", "energy").Result;
                    string retreat = string.Empty;
                    for (int i = 0; i < retreatEnergy.Count; i++)
                    {
                        if (i < retreatEnergy.Count - 1)
                            retreat += UpperLetter(retreatEnergy[i].OuterHtml.Split('"')[1].Split("-")[1]) + ", ";
                        else
                            retreat += UpperLetter(retreatEnergy[i].OuterHtml.Split('"')[1].Split("-")[1]);
                    }
                    pc.Retreat = retreat;
                }
            }
            //Expansion
            var footer = FindNodesByNode(statsBlock, "div", "class", "stats-footer").Result.FirstOrDefault();
            if (footer != null)
            {
                var expansion = FindNodesByNode(footer, "a", "href", "").Result.FirstOrDefault().InnerText;
                pc.Expansion = expansion;
            }
            // Illustrator
            var highlight = FindNodesByNode(main, "h4", "class", "highlight").Result.FirstOrDefault();
            if (highlight != null)
                pc.Illustrator = highlight.InnerText.Split(":")[1].Trim();

            // Evolves from Name
            var evolve = FindNodesByNode(descriptionBlock, "div", "class", "card-type").Result.FirstOrDefault();
            var evolvesBlock = FindNodesByNode(evolve, "h4").Result.FirstOrDefault();
            if (evolvesBlock != null)
            {
                pc.EvolvesName = evolvesBlock.InnerText.Split(":")[1].Trim();
            }

            // Skills
            var abilities = FindNodesByNode(information, "div", "class", "pokemon-abilities").Result.FirstOrDefault();
            var abilityList = FindNodesByNode(abilities, "div", "class", "ability").Result;

            List<Skills> skills = new();
            for (int i = 0; i < abilityList.Count; i++)
            {
                Skills skill = new();

                var skillElements = FindNodesByNode(abilityList[i], "li", "rel", "tooltip").Result;
                for (int j = 0; j < skillElements.Count; j++)
                {
                    var elementSplit = skillElements[j].InnerHtml.Split('"');

                    if (j < skillElements.Count - 1)
                        skill.Element += elementSplit[3] + "; ";
                    else
                        skill.Element += elementSplit[3];
                }

                var skillName = FindNodesByNode(abilityList[i], "h4", "class", "left label").Result.FirstOrDefault();
                if (skillName != null)
                    skill.Name = skillName.InnerText;

                var skilldmg = FindNodesByNode(abilityList[i], "span", "class", "right plus").Result.FirstOrDefault();
                if (skilldmg != null)
                    skill.Damage = skilldmg.InnerText;

                var skilldescPre = FindNodesByNode(abilityList[i], "pre").Result.FirstOrDefault();
                if (skilldescPre != null)
                    skill.Description = skilldescPre.InnerText;

                var skilldescP = FindNodesByNode(abilityList[i], "p").Result.FirstOrDefault();
                if (skilldescP != null)
                    skill.Description = skilldescP.InnerText;
                skills.Add(skill);
            }
            pc.Skills = skills.ToArray();
            return pc;
        }



        private string UpperLetter(string str)
        {
            string energy = string.Empty;
            for (int i = 0; i < str.Length; i++)
            {
                if (i == 0)
                    energy += str[i].ToString().ToUpper();
                else
                    energy += str[i].ToString();
            }
            return energy;
        }


        #region Private Nodes

        private async Task<List<HtmlNode>> FindNodesByNode(HtmlNode node, string a = "", string b = "", string c = "")
        {
            return node.Descendants(a).Where(node => node.GetAttributeValue(b, "").Contains(c)).ToList();
        }

        private async Task<List<HtmlNode>> FindNodesByDocument(HtmlDocument document, string a = "", string b = "", string c = "")
        {
            return document.DocumentNode.Descendants(a).Where(node => node.GetAttributeValue(b, "").Contains(c)).ToList();
        }

        #endregion
    }
}
