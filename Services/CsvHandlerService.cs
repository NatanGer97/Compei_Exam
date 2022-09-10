using CsvHelper.Configuration;
using CsvHelper;
using System.Globalization;
using System.IO;
using System.Security.Policy;
using System.ServiceProcess;
using WebApplication1.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Dynamic;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WebApplication1.Services
{
    
    public sealed class CsvHandlerService
    {
        private static CsvHandlerService _instance;
        private static readonly object _lock = new object();

        private readonly string FILE_NAME = @"players.csv";
        private  readonly string RESULTS_FILE_NAME = @"results.csv";

        private CsvHandlerService()
        {

        }

        public  static CsvHandlerService GetInstance()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new CsvHandlerService();
                    }
                }
            }
            return _instance;
        }
           

        public  bool isFileExite(string fileName)
        {
            return File.Exists(fileName);
        }

        public  IEnumerable<object> ReadCSVFile(string fileName)
        {

            IEnumerable<object> players = null;

            if (isFileExite(fileName))
            {
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    PrepareHeaderForMatch = args => args.Header.ToLower(),
                };

                using (var reader = new StreamReader(fileName)) 

                using (var csv = new CsvReader(reader, config))
                {
                    players = csv.GetRecords<Player>().ToList();                   
                }
               
            }

            return players;
        }

      
        public void WriteToResultsCSVFile(List<FullPlayer> players)
        {
            if (players != null)
            {
                using (StreamWriter writer = new StreamWriter(RESULTS_FILE_NAME))
                {
                    using (CsvWriter csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
                    {
                       csvWriter.WriteRecords(players);
                    }
                }                              

            }
        }

        /*    Dictionary<int, FullPlayer> dict = new Dictionary<int, FullPlayer>();
       foreach (var item in players)
       {
           dict.Add(item.id, item);
       }

       using (StreamWriter file = File.CreateText(@"dict.json"))
       using (JsonTextWriter writer = new JsonTextWriter(file))
       {
           JObject jObject = JObject.FromObject(dict);
           jObject.WriteTo(writer);
       }*/

        public async Task<byte[]> returnFile()
        {
            
            var bytes = await System.IO.File.ReadAllBytesAsync(RESULTS_FILE_NAME);

            return bytes;

        }
    }


}
