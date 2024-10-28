using ETicaretAPI.Application.Abstractions.Storage;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Infrastructure.Services.Storage
{
    public class StorageService : IStorageService
    {
        readonly IStorage _storage;//Mimaride kullanılacak Storage temsil ediyor Azure/Local vs 

        public StorageService(IStorage storage)
        {
            _storage = storage;
        }

        public string StorageName => _storage.GetType().Name;

        public async Task DeleteAsync(string pathOrContainerName, string fileName)
        
           => await _storage.DeleteAsync(pathOrContainerName, fileName);
        

        public  List<string> GetFileNames(string pathOrContainerName)
        
           => _storage.GetFileNames(pathOrContainerName);
        

        public bool HasFile(string pathOrContainerName, string fileName)

           => _storage.HasFile(pathOrContainerName, fileName);  

        public Task<List<(string fileName, string pathOrContainerName)>> UploadAsync(string path, IFormFileCollection files)
           
            => _storage.UploadAsync(path, files);
    }
}
