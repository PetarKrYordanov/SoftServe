namespace TextAnalyticsTools.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using ImageServe.Models;
    public interface ITextAnalyticsService
    {
        Task<ICollection<ImageTag>> GenerateTagsAsync(string text);
    }
}
