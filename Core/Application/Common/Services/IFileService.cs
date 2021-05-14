using System.Threading.Tasks;

namespace Core.Application.Common.Services
{
    public interface IFileService
    {
        /// <summary>
        /// Сохранить файл
        /// </summary>
        /// <param name="fileName">Наименование файла</param>
        /// <param name="content">Файл</param>
        /// <returns>Путь к файлу</returns>
        Task<string> Save(string fileName, byte[] content);

        /// <summary>
        /// Загрузить файл
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        /// <returns>Файл</returns>
        Task<byte[]> Load(string path);
    }
}