using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using WebApplication1.Cache;
using WebApplication1.Models;
using System.Threading.Tasks;

namespace WebApplication1.Services
{
    public class PlayersService
    {
        private readonly string FILE_NAME = "players.csv";
        private readonly HttpClient _httpClient = new HttpClient();
        private string API_UR = "https://www.balldontlie.io/api/v1/players/{0}";



        public async Task<List<FullPlayer>> GetFullDataAboutPlayers(IEnumerable objects)
        {
            List<FullPlayer> fullPlayers = new List<FullPlayer>();

            foreach (var item in objects)
            {
                Player player = item as Player;
                Console.WriteLine(player.ToString());
                FullPlayer cachedItem = CacheManager.Instance.GetItemFromCache(player.Id);
                if (cachedItem != null)
                {
                    fullPlayers.Add(cachedItem);
                    Console.WriteLine(cachedItem.id);
                }
                else
                {

                    HttpResponseMessage response = await _httpClient.GetAsync(String.Format(API_UR, player.Id));

                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsStringAsync();

                        FullPlayer fullPlayer = System.Text.Json.JsonSerializer.Deserialize<FullPlayer>(data);

                        fullPlayers.Add(fullPlayer);
                        CacheManager.Instance.AddToCache(player.Id, fullPlayer);

                    }
                }

            }

            return fullPlayers;
        }

    }
}
