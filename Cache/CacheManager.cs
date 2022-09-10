using Microsoft.AspNetCore.Authentication.Cookies;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.Caching;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Web;
using System.Xml.Linq;
using WebApplication1.HashingUtility;
using WebApplication1.Models;

namespace WebApplication1.Cache
{
    public sealed class CacheManager : ICacheable
    {
        private static CacheManager cacheManager; 
        private static object _lock = new System.Object();
        private Dictionary<string, string> playerToShaHash;

        private ObjectCache playersCache;
        private ObjectCache sha1Cache;
        private  CacheManager()
        {
            playersCache = MemoryCache.Default;
            sha1Cache = MemoryCache.Default;
            playerToShaHash = new Dictionary<string, string>();
        }

        public object getSh1OfPlayer(string key)
        {
            object hash = null;
            
            if (sha1Cache.Contains(key))
            {
                hash = sha1Cache.Get(key) as string;
            }

            return hash;
        }

        public static CacheManager Instance
        {
            get
            {
                if (cacheManager == null)
                {
                    lock (_lock)
                    {
                        if (cacheManager == null)
                        {
                            cacheManager = new CacheManager();
                        }
                    }
                }
                
                return cacheManager;
            }

        }

        public void AddToCache(string key, object value)
        {
            playersCache.Add(key, value, null);

            // calc sha hash for each record;
            using (SHA1 sha1Hash = SHA1.Create())
            {
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(value);
                byte[] jsonAsBytes = Encoding.UTF8.GetBytes(json);
                byte[] hashBytes = sha1Hash.ComputeHash(jsonAsBytes);
                string hash = BitConverter.ToString(hashBytes).Replace("-", String.Empty);
                
                sha1Cache.Add(key, hash,null);
                playerToShaHash.Add(key, hash);

                /*Console.WriteLine($"s -> {jsonAsBytes}\n hash -> {hashBytes}\n - {hash}\n");*/
                

            }

        }

        public void UpdateCacheItem(string key, object updatedValue)
        {
            if (playersCache.Contains(key) && updatedValue != null)
            {
                playersCache.Set(key, updatedValue,null);
            }
            
            // throw error (?)
        }

        public void UpdateShaPlyersHash(string key, object updatedValue)
        {
            if (updatedValue != null && sha1Cache.Contains(key))
            {
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(updatedValue);
                string hash = HSA1Utility.Hash(json);
                sha1Cache.Set(key, hash, null);

                /* using (SHA1 sha1Hash = SHA1.Create())
                {
                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(updatedValue);
                    byte[] jsonAsBytes = Encoding.UTF8.GetBytes(json);
                    byte[] hashBytes = sha1Hash.ComputeHash(jsonAsBytes);
                    string hash = BitConverter.ToString(hashBytes).Replace("-", String.Empty);

                    sha1Cache.Set(key, hash, null);

                }*/
            }
        }

        public FullPlayer GetItemFromCache(string key)
        {
            FullPlayer fullPlayer = null;

            if (playersCache.Contains(key))
            {
               fullPlayer =  playersCache.Get(key) as FullPlayer;
            }

            return fullPlayer;
            
        }

        public  object GetCacheItem(string key)
        {
            FullPlayer fullPlayer = null;

            if (playersCache.Contains(key))
            {
                fullPlayer = playersCache.Get(key) as FullPlayer;
            }

            return fullPlayer;
        }

        public bool CheckIfContentChanged(string key, string hashToCompareTo)
        {
            string oldHash = playerToShaHash[key];

           return string.Equals(oldHash, hashToCompareTo);
        }

        /*public  void SimpleWrite()
        {
            var jsonString = JsonConvert.SerializeObject(playeresDictCache);
            File.WriteAllText("dict.json", jsonString);
        }*/

/*        public Dictionary<string, FullPlayer> readCacheFile()
        {
            Dictionary<string, FullPlayer> playersCache = null;
            using (StreamReader r = new StreamReader("dict.json"))
            {
                string json = r.ReadToEnd();
                playersCache = JsonConvert.DeserializeObject<Dictionary<string,FullPlayer>>(json);
                foreach (var item in playersCache)
                {
                    Console.WriteLine(item);
                }
            }

            return playersCache;
        }
*/         
        
    }
}
