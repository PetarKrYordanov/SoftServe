namespace TextAnalyticsTools
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
    using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;

    using ImageServe.Models;
    using TextAnalyticsTools.Contracts;
    public class TextAnalyticsService : ITextAnalyticsService
    {
        private readonly List<string> supportedLanguage = new List<string>() { "en", "da", "nl", "fi", "fr", "de", "it", "ja", "ko", "no", "pl", "pt-PT", "pt-BR", "ru", "es", "sv" };
        private const string REQUIRED_PARAMETER = "1";
        
        private readonly ITextAnalyticsClient client;
       
        public TextAnalyticsService(ITextAnalyticsClient client)
        {
            this.client = client;
        }

        public async Task<ICollection<ImageTag>> GenerateTagsAsync(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }

            var language = await this.DetectLanguage(text);

            if (!supportedLanguage.Contains(language))
            {
                return null;
            }

            var analysis = await TextAnalysis(text, language);

            //extract the key phrases from the result as a collection of strings
            var tags = ExtractTags(analysis);

            return tags;
        }

        private async Task<KeyPhraseBatchResult> TextAnalysis(string text, string language)
        {
            //analyze the text
            var analysis = await this.client.KeyPhrasesAsync(new MultiLanguageBatchInput(
                        new List<MultiLanguageInput>()
                        {
                          new MultiLanguageInput(language, REQUIRED_PARAMETER, text)
                          
                        }));

            return analysis;
        }

        private ICollection<ImageTag> ExtractTags(KeyPhraseBatchResult result)
        {
            return result.Documents[0].KeyPhrases.Select(k => new ImageTag() { Name = k, FromDescription = true }).ToList();
        }

        private async Task<string> DetectLanguage(string text)
        {
            //analyze the text
            var result = await this.client.DetectLanguageAsync(new BatchInput(
                    new List<Input>()
                        {
                          new Input(REQUIRED_PARAMETER, text)
                    }));

            //return text language as string in Iso6391format ex. "en" for english
            var language = result.Documents[0].DetectedLanguages[0].Iso6391Name;

            return language;
        }
    }
}
