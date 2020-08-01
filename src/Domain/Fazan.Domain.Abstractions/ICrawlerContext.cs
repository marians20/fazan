namespace Fazan.Domain.Abstractions
{
    using System.Net.Http;

    public interface ICrawlerContext
    {
        string BaseUrl { get; }

        HttpClient Client { get; }

        IDomProcessor DomProcessor { get; }

        IWordsService WordsService { get; }
    }
}