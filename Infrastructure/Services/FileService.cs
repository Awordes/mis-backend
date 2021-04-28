using System;
using System.IO;
using System.Threading.Tasks;
using Core.Application.Common.Services;
using Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services
{
    public class FileService: IFileService
    {
        private readonly MercuryFileOptions _mercuryFileOptions;

        public FileService(IOptionsMonitor<MercuryFileOptions> fileOptions)
        {
            _mercuryFileOptions = fileOptions.CurrentValue;
        }

        public async Task<string> Save(string fileName, byte[] content)
        {
            Console.WriteLine("Env: " +_mercuryFileOptions.Folder);
            var directoryFolder = Path.Combine(
                _mercuryFileOptions.Folder,
                DateTime.Now.Year.ToString(),
                DateTime.Now.Month.ToString());
            
            if (!Directory.Exists(directoryFolder))
                Directory.CreateDirectory(directoryFolder);

            var identifier = Guid.NewGuid().ToString()[..5];
            var path = Path.Combine(directoryFolder, $"{identifier}_{fileName}");
            
            await File.WriteAllBytesAsync(path, content);

            return path;
        }

        public Task<byte[]> Load(string path)
        {
            throw new NotImplementedException();
        }
    }
}