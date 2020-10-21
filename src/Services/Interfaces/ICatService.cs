using System.Collections.Generic;
using System.Threading.Tasks;
using CatApi.Models;

namespace CatApi.Services.Interfaces
{
    public interface ICatService
    {
        Task<Cat> GetCatAsync(string id);
        Task<IEnumerable<Cat>> GetCatsAsync();
        Task<IEnumerable<Cat>> GetCatsAsync(bool randomOrder);
        Task<Cat> AddCatAsync(Cat cat);
        Task<bool> AddLoveAsync(string id);
        Task<bool> AddHateAsync(string id);
        Task<bool> DownloadCatImageAsync(string id);
    }
}