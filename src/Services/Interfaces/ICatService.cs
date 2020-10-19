using System.Collections.Generic;
using CatApi.Models;

namespace CatApi.Services.Interfaces
{
    public interface ICatService
    {
        List<Cat> Cats();
    }
}