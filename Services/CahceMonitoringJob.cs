using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;

using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebApplication1.Cache;
using WebApplication1.HashingUtility;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class CahceMonitoringJob : IHostedService, IDisposable
    {
        private readonly ILogger<CahceMonitoringJob> _logger;
        private string API_UR = "https://www.balldontlie.io/api/v1/players/{0}";
        private Timer _timer;
        private int _counter;
        public CahceMonitoringJob(ILogger<CahceMonitoringJob> logger)
        {
            _logger = logger;
            _counter = 0;
        }



        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Task startef");
            _timer = new Timer(DoWork,null,TimeSpan.Zero, TimeSpan.FromSeconds(20) );

            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            if (_counter++ == 0) return;
            HttpClient httpClient = new HttpClient();

            IEnumerable<Player> playersEnumerable =
                (IEnumerable<Player>)CsvHandlerService.GetInstance().ReadCSVFile("players.csv");

            foreach (var player in playersEnumerable)
            {
                HttpResponseMessage response = await httpClient.GetAsync(String.Format(API_UR, player.Id));
                if (response.IsSuccessStatusCode)
                {
                    string responseContentAsJsonString = await response.Content.ReadAsStringAsync();
                   string hash =  HSA1Utility.Hash(responseContentAsJsonString);

                    if (!CacheManager.Instance.CheckIfContentChanged(player.Id, hash))
                    {
                        _logger.LogInformation("player details was changed");

                        // update cache
                        // notify data was change via web socket
                    }
                    _logger.LogInformation(CacheManager.Instance.CheckIfContentChanged(player.Id, hash).ToString());

                    /*using (SHA1 sha1Hash = SHA1.Create())
                    {

                        byte[] jsonAsBytes = Encoding.UTF8.GetBytes(responseContentAsJsonString);
                        byte[] hashBytes = sha1Hash.ComputeHash(jsonAsBytes);
                        string hash = BitConverter.ToString(hashBytes).Replace("-", String.Empty);
                        if (!CacheManager.Instance.CheckIfContentChanged(player.Id, hash))
                        {
                            _logger.LogInformation("player details was changed");
                            
                            // update cache
                            // notify data was change via web socket
                        }


                    }*/

                }

            }

            /*HttpClient httpClient = new HttpClient();*/
            /*Dictionary<string, FullPlayer> dictionary = CacheManager.Instance.readCacheFile();*/
            /*foreach (var item in dictionary)
            {
                HttpResponseMessage response = await httpClient.GetAsync(String.Format(API_UR, item.Key));
                if (response.IsSuccessStatusCode)
                {
                    string responseContentAsJsonString = await response.Content.ReadAsStringAsync();
                    FullPlayer v = CacheManager.Instance.GetCacheItem(item.Key) as FullPlayer;
                    var jsonString = JsonConvert.SerializeObject(item.Value);
                    Console.WriteLine(jsonString.Equals(responseContentAsJsonString));

                }

            }*/
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("host stopped");
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer.Dispose();
            
        }

    }
}
