using Microsoft.Extensions.Configuration;
using SifenApi.Application.Common.Interfaces;

namespace SifenApi.Infrastructure.Services;

public class FileStorageService : IFileStorageService
{
    private readonly string _basePath;

    public FileStorageService(IConfiguration configuration)
    {
        _basePath = configuration["Storage:BasePath"] ?? Path.Combine(Directory.GetCurrentDirectory(), "storage");
        
        if (!Directory.Exists(_basePath))
            Directory.CreateDirectory(_basePath);
    }

    public async Task<string> SaveFileAsync(
        byte[] content,
        string fileName,
        string contentType,
        string folder,
        CancellationToken cancellationToken = default)
    {
        var folderPath = Path.Combine(_basePath, folder);
        
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        var filePath = Path.Combine(folderPath, fileName);
        await File.WriteAllBytesAsync(filePath, content, cancellationToken);

        // Retornar URL relativa
        return $"/files/{folder}/{fileName}";
    }

    public async Task<byte[]> GetFileAsync(
        string fileName,
        string folder,
        CancellationToken cancellationToken = default)
    {
        var filePath = Path.Combine(_basePath, folder, fileName);
        
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Archivo no encontrado: {fileName}");

        return await File.ReadAllBytesAsync(filePath, cancellationToken);
    }

    public async Task<bool> DeleteFileAsync(
        string fileName,
        string folder,
        CancellationToken cancellationToken = default)
    {
        var filePath = Path.Combine(_basePath, folder, fileName);
        
        if (!File.Exists(filePath))
            return false;

        await Task.Run(() => File.Delete(filePath), cancellationToken);
        return true;
    }

    public Task<string> GetFileUrlAsync(
        string fileName,
        string folder,
        TimeSpan? expiration = null)
    {
        // En producción, esto podría generar URLs firmadas con expiración
        return Task.FromResult($"/files/{folder}/{fileName}");
    }
}