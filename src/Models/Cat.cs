using System;

namespace CatApi.Models
{
    public class Cat
    {
        public string Id { get; set; }
        public string Url { get; set; }
        public string SourceUrl { get; set; }
        public string Name { get; set; }
        public int Loves { get; set; }
        public int Hates { get; set; }

        public Cat(string url, string sourceUrl, string name)
        {
            Id = Guid.NewGuid().ToString();
            Url = url;
            SourceUrl = sourceUrl;
            Name = name;
            Hates = 0;
            Loves = 0;
        }

        public Cat()
        {
        }
    }
}