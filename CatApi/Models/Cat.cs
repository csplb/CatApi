using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CatApi.Models
{
    public class Cat
    {
        public string Id { get; set; }
        public string Url { get; set; }
        public string SourceUrl { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Loves { get; set; }
        public int Hates { get; set; }
    }
}
