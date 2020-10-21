using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using CatApi.Models;
using CatApi.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace CatApi.Services
{
    public class CatService : ICatService
    {
        private readonly CatDbContext _catDbContext;
        private readonly string _downloadDirectoryPath;
        
        public CatService(CatDbContext catDbContext, IWebHostEnvironment webHostEnvironment)
        {
            _catDbContext = catDbContext;
            SeedDatabase();
            
            _downloadDirectoryPath =  $"{webHostEnvironment.ContentRootPath}\\Downloads";
            if (!Directory.Exists(_downloadDirectoryPath))
            {
                Directory.CreateDirectory(_downloadDirectoryPath);
            }
        }

        public Task<Cat> GetCatAsync(string id)
        {
            return _catDbContext.Cats.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<Cat>> GetCatsAsync()
        {
            return await _catDbContext.Cats.ToListAsync();
        }

        public async Task<IEnumerable<Cat>> GetCatsAsync(bool randomOrder)
        {
            if (!randomOrder)
                return await _catDbContext.Cats
                    .OrderByDescending(x => x.Loves)
                    .ThenByDescending(x => x.Hates)
                    .ToListAsync();

            return await _catDbContext.Cats
                .OrderBy(x => Guid.NewGuid())
                .ToListAsync();
        }
        
        public async Task<Cat> AddCatAsync(Cat cat)
        {
            await _catDbContext.AddAsync(cat);
            await _catDbContext.SaveChangesAsync();
            return await _catDbContext.Cats.LastAsync();
        }

        public async Task<bool> AddLoveAsync(string id)
        {
            var cat = await _catDbContext.Cats.FirstOrDefaultAsync(x => x.Id == id);
            if (cat == null)
            {
                return false;
            }
            cat.Loves++;
            await _catDbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddHateAsync(string id)
        {
            var cat = await _catDbContext.Cats.FirstOrDefaultAsync(x => x.Id == id);
            if (cat == null)
            {
                return false;
            }
            cat.Hates++;
            await _catDbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DownloadCatImageAsync(string id)
        {
            var cat = await _catDbContext.Cats.FirstOrDefaultAsync(x => x.Id == id);

            if (cat == null)
            {
                return false;
            }

            try
            {
                using var client = new WebClient();
                var filepath = Path.Combine($"{_downloadDirectoryPath}\\{cat.Name}.{cat.Url.Substring(cat.Url.Length - 3)}");
                client.DownloadFileAsync(new Uri(cat.Url), filepath);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
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
            using FileStream fs = new FileStream("cats.json", FileMode.Open);
            using StreamReader sr = new StreamReader(fs);
            var file = sr.ReadToEnd();
            return JsonSerializer.Deserialize<List<Cat>>(file);
        }
    }
}