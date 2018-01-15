using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CatApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CatApi.Controllers
{
    public class CatController : Controller
    {
        private static List<Cat> cats;

        static CatController()
        {
            using (FileStream fs = new FileStream("cats.json", FileMode.Open))
            using (StreamReader sr = new StreamReader(fs))
            {
                var file = sr.ReadToEnd();
                cats = JsonConvert.DeserializeObject<List<Cat>>(file);
            }
        }

        // GET api/values
        [HttpGet("api/cats")]
        public IEnumerable<Cat> Get()
        {
            return cats;
        }

        // GET api/values/5
        [HttpGet("api/cat/{id}")]
        public IActionResult Get(string id)
        {
            var result = cats.FirstOrDefault(x => x.Id == id);
            if (result == null)
                return NotFound();
            else
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
            else
            {
                result.Loves++;
                return Ok();
            }
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
            else
            {
                result.Hates++;
                return Ok();
            }
        }

        [Authorize]
        [HttpPost("api/admin/save")]
        public IActionResult Save()
        {
            using (FileStream fs = new FileStream("cats.json", FileMode.Create))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                var data = JsonConvert.SerializeObject(cats);
                sw.Write(data);
            }

            return Ok();
        }
    }
}
