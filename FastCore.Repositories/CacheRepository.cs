using FastCore.Repositories.Infrastructure.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastCore.Repositories
{
    public class CacheRepository : ICacheRepository
    {
        private string _connectionString;
        private ConnectionMultiplexer _connectionMultiplexer;
        private IDatabase _db;

        public CacheRepository(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString)) { throw new ArgumentNullException("connectionString", "A string de conexão não pode ser nula."); }
            _connectionString = connectionString;

            _connectionMultiplexer = ConnectionMultiplexer.Connect(_connectionString);
            _db = _connectionMultiplexer.GetDatabase();
        }
        
        public async Task AddAsync(string key, string value)
        {
            try
            {
                await Task.Run(() =>
                {
                    _db.StringSet(key, value);
                });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        
        public async Task AddAsync(string key, string value, double minutes)
        {
            try
            {
                await Task.Run(() =>
                {
                    var timeSpan = TimeSpan.FromMinutes(minutes);
                    _db.StringSet(key, value, timeSpan);
                });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        
        public async Task UpdateAsync(string key, string value)
        {
            try
            {
                await Task.Run(async () =>
                {
                    var isExistsKeyInCache = await ExistsAsync(key);
                    if (isExistsKeyInCache)
                    {
                        await RemoveAsync(key);
                        await AddAsync(key, value);
                    }
                    else
                    {
                        await AddAsync(key, value);
                    }
                });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        
        public async Task UpdateAsync(string key, string value, double minutes)
        {
            try
            {
                await Task.Run(async () =>
                {
                    var isExistsKeyInCache = await ExistsAsync(key);
                    if (isExistsKeyInCache)
                    {
                        await RemoveAsync(key);
                        await AddAsync(key, value, minutes);
                    }
                });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        
        public async Task<string> GetAsync(string key)
        {
            try
            {
                return await Task.Run(() =>
                {
                    var result = _db.StringGet(key);
                    return result;
                });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        
        public async Task RemoveAsync(string key)
        {
            try
            {
                await Task.Run(() =>
                {
                    _db.KeyDelete(key);
                });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        
        public async Task ClearAllDataBaseAsync()
        {
            try
            {
                await Task.Run(() =>
                {
                    var endpoints = _connectionMultiplexer.GetEndPoints(true);
                    foreach (var endpoint in endpoints)
                    {
                        var server = _connectionMultiplexer.GetServer(endpoint);
                        server.FlushAllDatabases(); // Limpa todos os banco de dados.
                    }
                });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        
        public async Task ClearDataBaseAsync(int database)
        {
            try
            {
                await Task.Run(() =>
                {
                    var endpoints = _connectionMultiplexer.GetEndPoints(true);
                    foreach (var endpoint in endpoints)
                    {
                        var server = _connectionMultiplexer.GetServer(endpoint);
                        server.FlushDatabase(database); // limpa apenas um banco de dados, 0 by default.
                    }
                });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        
        public async Task<bool> ExistsAsync(string key)
        {
            try
            {
                return await Task.Run(() =>
                {
                    return _db.KeyExists(key);
                });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
