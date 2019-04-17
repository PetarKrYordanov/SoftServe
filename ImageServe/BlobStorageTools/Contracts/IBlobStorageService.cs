namespace BlobStorageTools.Contracts
{
using System.Threading.Tasks;
    public interface IBlobStorageService
    {
        Task UploadFromFileAsync(byte[] file, string name, string extension);


        Task DeleteImageAsync(string imageName);
    }
}
