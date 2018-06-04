using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LolSynergy
{
    class StatScraper
    {
        readonly static string username = "TSM Zven";
        static Dictionary<string, Champion> statsDict = new Dictionary<string, Champion>();

        private static void InitStats(string champion)
        {
            statsDict[champion] = new Champion() { Name = champion };
        }

        private static bool IsVictory(IWebElement div)
        {
            return div.GetAttribute("class").Contains("Win");
        }

        /// <summary>
        /// Return a tuple of teams. 
        /// The first team is the allied team and the second one is the enemy team.
        /// </summary>
        private static (IWebElement, IWebElement) DetermineTeams(IReadOnlyCollection<IWebElement> teams)
        {
            var teamsArray = teams.ToArray();
            if (teamsArray[0].Text.Contains(@"\n" + username))
            {
                return (teamsArray[0], teamsArray[1]);
            }
            return (teamsArray[1], teamsArray[0]);
        }

        private static void ProcessTeam(IWebElement team, bool isVictory)
        {
            var summoners = team.FindElements(By.ClassName("Summoner"));
            foreach (var summoner in summoners)
            {
                // Skip user's champion
                var summonerName = summoner.FindElement(By.ClassName("Link")).Text;
                if (summonerName == username) continue;

                var champion = summoner.FindElement(By.ClassName("__sprite")).Text;

                if (!statsDict.ContainsKey(champion))
                    InitStats(champion);

                if (isVictory)
                    statsDict[champion].Victories++;
                else
                    statsDict[champion].Defeats++;
                
            }
        }

        private static void ProcessVictoriesAndDefeats(ChromeDriver driver)
        {
            var divs = driver.FindElements(By.ClassName("GameItem"));
            foreach (var div in divs)
            {
                bool isVictory = IsVictory(div);
                var teams = div.FindElements(By.ClassName("Team"));
                (var allies, var enemies) = DetermineTeams(teams);
                ProcessTeam(allies, isVictory);
            }
        }

        private static void PrintStats()
        {
            foreach (var stat in statsDict.Values.OrderBy(x => x.Name))
            {
                Console.WriteLine("Winrates (Allied Team):");
                Console.WriteLine($"{stat.Name}: {stat.Victories} / {stat.TotalMatches} ({stat.WinRate} Winrate)");
            }
        }
        public static void GetStats()
        {
            var options = new ChromeOptions();
            options.AddArguments("headless");

            using (var driver = new ChromeDriver(options))
            {
                driver.Navigate().GoToUrl(@"http://na.op.gg/summoner/userName=" + username);
                ProcessVictoriesAndDefeats(driver);

                PrintStats();
            }
        }
    }
}