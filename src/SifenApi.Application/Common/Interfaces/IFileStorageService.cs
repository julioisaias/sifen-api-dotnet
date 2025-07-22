namespace SifenApi.Application.Common.Interfaces;

public interface IFileStorageService
{
    Task<string> SaveFileAsync(
        byte[] content,
        string fileName,
        string contentType,
        string folder,
        CancellationToken cancellationToken = default);
    
    Task<byte[]> GetFileAsync(
        string fileName,
        string folder,
        CancellationToken cancellationToken = default);
    
    Task<bool> DeleteFileAsync(
        string fileName,
        string folder,
        CancellationToken cancellationToken = default);
    
    Task<string> GetFileUrlAsync(
        string fileName,
        string folder,
        TimeSpan? expiration = null);
}