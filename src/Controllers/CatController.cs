using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using CatApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using CatApi.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace CatApi.Controllers
{
    public class CatController : Controller
    {
        private readonly ICatService _catService;

        public CatController(ICatService catService)
        {
            _catService = catService;
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
        public async Task<IActionResult> Get(string id)
        {
            var result = await _catService.GetCatAsync(id);
            return result != null ? (IActionResult) Ok(result) : NotFound();
        }
        
        /// <summary>
        /// Gets cats list. Not randomized by default.
        /// </summary>
        /// <param name="rand"></param>
        /// <returns>The cat list</returns>
        /// <response code="200">Returns the cat list</response>
        [HttpGet("api/cats")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IEnumerable<Cat>> Get([FromQuery] bool rand = false)
        {
            if (!rand)
                return _catService.GetCatsAsync()
                    .Result
                    .OrderByDescending(x => x.Loves)
                    .ThenByDescending(x => x.Hates);

            return _catService.GetCatsAsync()
                .Result
                .OrderBy(x => Guid.NewGuid());
        }
        
        /// <summary>
        /// Adds a new cat.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sourceUrl"></param>
        /// <param name="name"></param>
        /// <response code="200">Returns Ok when the cat is added to database</response>
        [Authorize]
        [HttpPost("api/cat/")]
        public async Task<IActionResult> AddCat(string url, string sourceUrl, string name)
        {
            var result = await _catService.AddCatAsync(new Cat(url, sourceUrl, name));
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
        public async Task<IActionResult> Love(string id)
        {
            var result = await _catService.AddLoveAsync(id);
            return result ? (IActionResult) Ok() : NotFound();
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
        public async Task<IActionResult> Hate(string id)
        {
            var result = await _catService.AddHateAsync(id);
            return result ? (IActionResult) Ok() : NotFound();
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
        public async Task<IActionResult> Image(string id)
        {
            var result = await _catService.DownloadCatImageAsync(id);
            return result ? (IActionResult) Ok() : NotFound();
        }
    }
}