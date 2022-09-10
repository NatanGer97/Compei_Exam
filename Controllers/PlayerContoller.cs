using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Text.Json;
using System.Threading;
using WebApplication1.Cache;
using WebApplication1.Models;
using WebApplication1.Services;
using static System.Net.Mime.MediaTypeNames;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers
{
    [Route("")]
    [ApiController]
    public class PlayerContoller : ControllerBase
    {
        // GET: api/<Player>
        private readonly ILogger<PlayerContoller> _logger;
        private readonly HttpClient httpClient = new HttpClient();
        private string API_UR = "https://www.balldontlie.io/api/v1/players/{0}";
        private readonly CsvHandlerService csvHandlerService;
        private readonly PlayersService playersService;



        public PlayerContoller(ILogger<PlayerContoller> logger)
        {
            _logger = logger;
            csvHandlerService = CsvHandlerService.GetInstance();
            playersService = new PlayersService();

        }

        [HttpGet("player")]
        public async System.Threading.Tasks.Task<FileResult> Get()
        {
            List<FullPlayer> fullPlayers = new List<FullPlayer>();

            IEnumerable<Player> players = (IEnumerable<Player>)csvHandlerService.ReadCSVFile("players.csv");
            /*fullPlayers = playersService.GetFullDataAboutPlayers(players).Result;*/

            foreach (Player player in players)
            {
                Console.WriteLine(player.ToString());
                FullPlayer cachedItem = CacheManager.Instance.GetItemFromCache(player.Id);
                if (cachedItem != null)
                {
                    fullPlayers.Add(cachedItem);
                    Console.WriteLine(cachedItem.id);
                }

                else
                {
                    
                    HttpResponseMessage response = await httpClient.GetAsync(String.Format(API_UR, player.Id));

                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsStringAsync();
                        
                        FullPlayer fullPlayer = System.Text.Json.JsonSerializer.Deserialize<FullPlayer>(data);

                        fullPlayers.Add(fullPlayer);
                        CacheManager.Instance.AddToCache(player.Id, fullPlayer);

                    }
                }
            }


            /*Timer timer = new Timer(printHi, null, TimeSpan.Zero, TimeSpan.FromSeconds(5.0));*/

            /*CacheManager.Instance.SimpleWrite();*/

            csvHandlerService.WriteToResultsCSVFile(fullPlayers);

            return File(csvHandlerService.returnFile().Result, "text/csv", Path.GetFileName(@"results.csv"));


        }
        public void printHi(object o)
        {
            Console.WriteLine("Hi!");
        }

        /*  // GET api/<Player>/5
          [HttpGet("{id}")]
          public string Get(int id)
          {
              return "value";
          }

          // POST api/<Player>
          [HttpPost]
          public void Post([FromBody] string value)
          {
          }

          // PUT api/<Player>/5
          [HttpPut("{id}")]
          public void Put(int id, [FromBody] string value)
          {
          }

          // DELETE api/<Player>/5
          [HttpDelete("{id}")]
          public void Delete(int id)
          {
          }*/
    }
}
