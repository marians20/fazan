namespace Fazan.Domain.Services.CrawlerService
{
    using System.Net.Http;
    using Fazan.Domain.Abstractions;
    using Properties;

    public sealed class CrawlerContext : ICrawlerContext
    {
        private readonly IHttpClientFactory clientFactory;

        private readonly IDomProcessor domProcessor;

        private readonly IWordsService wordsService;

        public CrawlerContext(IHttpClientFactory clientFactory, IWordsService wordsService, IDomProcessor domProcessor)
        {
            this.clientFactory = clientFactory;
            this.wordsService = wordsService;
            this.domProcessor = domProcessor;
        }

        public string BaseUrl => Resources.BaseUrl;

        public HttpClient Client => clientFactory.CreateClient();

        public IDomProcessor DomProcessor => domProcessor;

        public IWordsService WordsService => wordsService;
    }
}
