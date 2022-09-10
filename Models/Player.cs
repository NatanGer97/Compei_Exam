using CsvHelper.Configuration.Attributes;

namespace WebApplication1.Models
{
    public class Player
    {
        [Name("id")]
        public string Id { get; set; }

        [Name("nickname")]
        public string Nickname { get; set; }

        public override string ToString()
        {
            return $"Player:{Id} - {Nickname}";
        }


    }
}
