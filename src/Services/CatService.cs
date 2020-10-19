using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CatApi.Models;
using CatApi.Services.Interfaces;

namespace CatApi.Services
{
    public class CatService : ICatService
    {
        private readonly CatDbContext _catDbContext;

        public CatService(CatDbContext catDbContext)
        {
            _catDbContext = catDbContext;
            SeedDatabase();
        }

        public List<Cat> Cats()
        {
            return _catDbContext.Cats.ToList();
        }
        private void SeedDatabase()
        {
            if (_catDbContext.Cats.Any())
            {
                return;
            }

            var cats = GetCatsFromJson();

            if (!cats.Any())
            {
                return;
            }
            _catDbContext.Cats.AddRange(cats);
            _catDbContext.SaveChanges();
        }

        private List<Cat> GetCatsFromJson()
        {
            using (FileStream fs = new FileStream("cats.json", FileMode.Open))
            using (StreamReader sr = new StreamReader(fs))
            {
                var file = sr.ReadToEnd();
                return JsonSerializer.Deserialize<List<Cat>>(file);
            }
        }
    }
}