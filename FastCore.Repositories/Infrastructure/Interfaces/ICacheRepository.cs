using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastCore.Repositories.Infrastructure.Interfaces
{
    public interface ICacheRepository
    {
        Task AddAsync(string key, string value);
        Task AddAsync(string key, string value, double minutes);
        Task UpdateAsync(string key, string value);
        Task UpdateAsync(string key, string value, double minutes);
        Task<string> GetAsync(string key);
        Task RemoveAsync(string key);
        Task ClearAllDataBaseAsync();
        Task ClearDataBaseAsync(int database);
        Task<bool> ExistsAsync(string key);
    }
}
