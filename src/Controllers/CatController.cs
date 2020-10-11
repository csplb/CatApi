using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using CatApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;

namespace CatApi.Controllers
{
    public class CatController : Controller
    {
        private static readonly List<Cat> cats;

        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string _downloadDirectoryPath;
 
        static CatController()
        {
            using (FileStream fs = new FileStream("cats.json", FileMode.Open))
            using (StreamReader sr = new StreamReader(fs))
            {
                var file = sr.ReadToEnd();
                cats = JsonSerializer.Deserialize<List<Cat>>(file);
            }
        }

        public CatController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _downloadDirectoryPath =  $"{_webHostEnvironment.ContentRootPath}\\Downloads";
            if (!Directory.Exists(_downloadDirectoryPath))
            {
                Directory.CreateDirectory(_downloadDirectoryPath);
            }
        }

        // GET api/values
        [HttpGet("api/cats")]
        public IEnumerable<Cat> Get([FromQuery] bool rand = false)
        {
            if (!rand)
                return cats.OrderByDescending(x => x.Loves).ThenByDescending(x => x.Hates);
            
            return cats.OrderBy(x => Guid.NewGuid());
        }

        // GET api/values/5
        [HttpGet("api/cat/{id}")]
        public IActionResult Get(string id)
        {
            var result = cats.FirstOrDefault(x => x.Id == id);
            if (result == null)
                return NotFound();
            
            return Ok(result);
        }

        // PUT api/values/5
        [Authorize]
        [HttpPut("api/love/{id}")]
        public IActionResult Love(string id)
        {
            var result = cats.FirstOrDefault(x => x.Id == id);
            if (result == null)
            {
                return NotFound();
            }
            
            result.Loves++;
            return Ok();
        }

        [Authorize]
        [HttpPut("api/hate/{id}")]
        public IActionResult Hate(string id)
        {
            var result = cats.FirstOrDefault(x => x.Id == id);
            if (result == null)
            {
                return NotFound();
            }
            
            result.Hates++; 
            return Ok();
        }

        [Authorize]
        [HttpGet("api/image/{id}")]
        public IActionResult Image(string id)
        {
            var cat = cats.FirstOrDefault(x => x.Id == id);

            if (cat == null)
            {
                return NotFound();
            }

            try
            {
                using (var client = new WebClient())
                {
                    var filepath = Path.Combine($"{_downloadDirectoryPath}\\{cat.Name}.{cat.Url.Substring(cat.Url.Length - 3)}");
                    client.DownloadFileAsync(new Uri(cat.Url), filepath);
                    return Ok();
                }
            }
            catch (Exception)
            {
                return NotFound();
            }
        }
    }
}