namespace ComputerVisionTools.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using ImageServe.Models; 

    public interface IComputerVisionService
    {
        Task<ICollection<ImageTag>> GenerateTagsAsync(string imageUrl);
    }
}
