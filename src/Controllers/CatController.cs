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
using Microsoft.AspNetCore.Http;

namespace CatApi.Controllers
{
    public class CatController : Controller
    {
        private static List<Cat> cats;

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

        /// <summary>
        /// Gets cats list. Not randomized by default.
        /// </summary>
        /// <param name="rand"></param>
        /// <returns>The cat list</returns>
        /// <response code="200">Returns the cat list</response>
        [HttpGet("api/cats")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IEnumerable<Cat> Get([FromQuery] bool rand = false)
        {
            if (!rand)
                return cats.OrderByDescending(x => x.Loves).ThenByDescending(x => x.Hates);
            
            return cats.OrderBy(x => Guid.NewGuid());
        }
        
        /// <summary>
        /// Gets a cat with given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>A cat</returns>
        /// <response code="200">Returns the cat</response>
        /// <response code="404">Returns Not Found if the id is wrong</response>   
        [HttpGet("api/cat/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Get(string id)
        {
            var result = cats.FirstOrDefault(x => x.Id == id);
            if (result == null)
                return NotFound();
            
            return Ok(result);
        }

        /// <summary>
        /// Increases love of a cat.
        /// </summary>
        /// <param name="id"></param>
        /// <response code="200">Returns Ok if love's been increased</response>
        /// <response code="404">Returns Not Found if the id is wrong</response> 
        [Authorize]
        [HttpPut("api/love/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        /// <summary>
        /// Increases hate of a cat.
        /// </summary>
        /// <param name="id"></param>
        /// <response code="200">Returns Ok if hate's been increased</response>
        /// <response code="404">Returns Not Found if the id is wrong</response> 
        [Authorize]
        [HttpPut("api/hate/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        /// <summary>
        /// Downloads the image of a cat with given id.
        /// </summary>
        /// <param name="id"></param>
        /// <response code="200">Returns Ok if image's been downloaded</response>
        /// <response code="404">Returns Not Found if the id is wrong</response>  
        [Authorize]
        [HttpGet("api/image/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        [Authorize]
        [HttpPost("api/cat/")]
        public IActionResult AddCat(string url, string sourceUrl, string name)
        {
            Cat cat = new Cat(url, sourceUrl, name);
            cats.Add(cat);
            
            using (FileStream fs = new FileStream("cats.json", FileMode.OpenOrCreate))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.Write(JsonSerializer.Serialize(cats));
            }

            return Ok(cat);
        }
    }
}