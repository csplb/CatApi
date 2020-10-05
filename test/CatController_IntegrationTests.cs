using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Xml;
using CatApi.Models;
using Xunit;

namespace CatApi.Test
{
    public class CatApi_Integration : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        public CatApi_Integration(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("/api/cats")]
        public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
        {
            var response = await DoGet(url);

            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        private async Task<HttpResponseMessage> DoGet(string url)
        {
            var client = _factory.CreateClient();
            return await client.GetAsync(url).ConfigureAwait(false);
        }

        [Fact]
        public async Task Get_AllCats_ShouldReturnNotEmptyList()
        {
            var response = await DoGet("/api/cats");

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299

            var resp = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<IList<Cat>>(resp, _jsonOptions);

            Assert.NotEmpty(data);
        }

        [Fact]
        public async Task Get_AllCatsInRandomOrder_ShouldReturnRandomList()
        {
            var response = await DoGet("/api/cats?rand=true");

            // Assert
            response.EnsureSuccessStatusCode();

            var resp = await response.Content.ReadAsStringAsync();
            var randomList = JsonSerializer.Deserialize<IList<Cat>>(resp, _jsonOptions);

            response = await DoGet("/api/cats");
            resp = await response.Content.ReadAsStringAsync();
            var list = JsonSerializer.Deserialize<IList<Cat>>(resp, _jsonOptions);

            Assert.NotEmpty(randomList);
            Assert.NotEqual(randomList, list);
            Assert.Equal(randomList.OrderBy(x => x.Id).Select(x => x.Id).ToList(), list.OrderBy(x => x.Id).Select(x => x.Id).ToList());
        }

        [Fact]
        public async Task Get_FirstCatById_ShouldReturnProperObject()
        {
            var response = await DoGet("/api/cats");

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299

            var resp = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<IList<Cat>>(resp, _jsonOptions);

            var id = data[0].Id;

            response = await DoGet($"/api/cat/{id}");
            resp = await response.Content.ReadAsStringAsync();
            var singleCat = JsonSerializer.Deserialize<Cat>(resp, _jsonOptions);

            Assert.Equal(data[0].Id, singleCat.Id);
            Assert.Equal(data[0].Name, singleCat.Name);
            Assert.Equal(data[0].Loves, singleCat.Loves);
            Assert.Equal(data[0].Hates, singleCat.Hates);
        }

        [Theory]
        [InlineData("invalidid")]
        public async Task Get_CatByNonexistingId_ShouldReturn404(string catId)
        {
            var response = await DoGet($"/api/cat/{catId}");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Put_LoveCatWithoutAuth_ShouldBe401()
        {
            var response = await DoGet("/api/cats?rand=true");

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299

            var resp = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<IList<Cat>>(resp, _jsonOptions);

            var id = data[0].Id;

            response = await DoPut($"/api/love/{id}", null, null);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Put_LoveCatWithWrongAuth_ShouldBe401()
        {
            var username = "wrong";
            var password = "wrong";

            var response = await DoGet("/api/cats?rand=true");

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299

            var resp = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<IList<Cat>>(resp, _jsonOptions);

            var id = data[0].Id;

            response = await DoPut($"/api/love/{id}", username, password);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Put_HateCatWithoutAuth_ShouldBe401()
        {
            var response = await DoGet("/api/cats?rand=true");

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299

            var resp = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<IList<Cat>>(resp, _jsonOptions);

            var id = data[0].Id;

            response = await DoPut($"/api/hate/{id}", null, null);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Put_HateCatWithWrongAuth_ShouldBe401()
        {
            var username = "wrong";
            var password = "wrong";

            var response = await DoGet("/api/cats?rand=true");

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299

            var resp = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<IList<Cat>>(resp, _jsonOptions);

            var id = data[0].Id;

            response = await DoPut($"/api/hate/{id}", username, password);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Put_LoveCatWithAuth_ShouldIncreaseLoves()
        {
            var username = "username";
            var password = "password";

            var response = await DoGet("/api/cats?rand=true");

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299

            var resp = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<IList<Cat>>(resp, _jsonOptions);

            var before = data[0];

            await DoPut($"/api/love/{before.Id}", username, password);

            response = await DoGet($"/api/cat/{before.Id}");
            resp = await response.Content.ReadAsStringAsync();
            var after = JsonSerializer.Deserialize<Cat>(resp, _jsonOptions);

            Assert.Equal(before.Id, after.Id);
            Assert.Equal(before.Name, after.Name);
            Assert.Equal(before.Loves + 1, after.Loves);
            Assert.Equal(before.Hates, after.Hates);
        }

        [Fact]
        public async Task Put_HateCatWithAuth_ShouldIncreaseHates()
        {
            var username = "username";
            var password = "password";

            var response = await DoGet("/api/cats?rand=true");

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299

            var resp = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<IList<Cat>>(resp, _jsonOptions);

            var before = data[0];

            await DoPut($"/api/hate/{before.Id}", username, password);

            response = await DoGet($"/api/cat/{before.Id}");

            resp = await response.Content.ReadAsStringAsync();
            var after = JsonSerializer.Deserialize<Cat>(resp, _jsonOptions);

            Assert.Equal(before.Id, after.Id);
            Assert.Equal(before.Name, after.Name);
            Assert.Equal(before.Loves, after.Loves);
            Assert.Equal(before.Hates + 1, after.Hates);
        }

        private async Task<HttpResponseMessage> DoPut(string url, string username, string password)
        {
            var client = _factory.CreateClient();

            if (username != null)
            {
                var byteArray = Encoding.UTF8.GetBytes($"{username}:{password}");
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            }

            return await client.PutAsync(url, null);
        }
    }
}