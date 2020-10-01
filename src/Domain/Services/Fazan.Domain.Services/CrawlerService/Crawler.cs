using Fazan.Domain.Models;
using MassTransit.Mediator;

namespace Fazan.Domain.Services.CrawlerService
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Abstractions;
    using CSharpFunctionalExtensions;
    using Extensions;
    using HtmlAgilityPack;
    using Properties;

    public class Crawler : ICrawler
    {
        private readonly ICrawlerContext _context;

        private readonly IMediator _mediator;

        public Crawler(ICrawlerContext context, IMediator mediator)
        {
            this._context = context;
            this._mediator = mediator;
        }

        public Task<Result<HtmlDocument>> GetPageContent(string url) =>
            GetBodyUsingWebRequest(url).Map(body =>
            {
                var htmlDoc = body.ToHtmlDocument();
                return htmlDoc;
            });

        public async Task<Result> ReadAllPages()
        {
            var oldUrl = string.Empty;
            var url = _context.BaseUrl;
            var uriBuilder = new UriBuilder(url);
            var scheme = uriBuilder.Scheme;
            var host = uriBuilder.Host;
            var port = uriBuilder.Port;

            Result<HtmlDocument> result;
            do
            {
                result = await GetPageContent(url).Tap(
                             async htmlDocument =>
                                 {
                                     var words = _context.DomProcessor.GetWordsFromDoc(htmlDocument);
                                     await _mediator.Send(Log.Create(string.Format(
                                         Resources.Crawler_ReadAllPages_Adding_words_from__0__to__1_,
                                         words.First(),
                                         words.Last()))).ConfigureAwait(false);
                                     await _context.WordsService.CreateBulk(words).ConfigureAwait(false);
                                     var nextUrl = _context.DomProcessor.GetNextPageUrl(htmlDocument);
                                     oldUrl = url;
                                     url = $"{scheme}://{host}:{port}{nextUrl}";
                                 }).ConfigureAwait(false);
            }
            while (result.IsSuccess && !url.Equals(oldUrl));

            if (!result.IsSuccess)
            {
                return result;
            }

            await _mediator.Send(Log.Create("Started calculating words dependencies")).ConfigureAwait(false);
            await _context.WordsService.Calculate().ConfigureAwait(false);
            return result;
        }

        private Task<Result<string>> GetBodyUsingWebRequest(string url) =>
            Task.Run(() =>
                {
                    var request = (HttpWebRequest) WebRequest.Create(url);
                    request.AllowAutoRedirect = true;
                    Result<string> bodyResult;
                    using (var response = (HttpWebResponse) request.GetResponse())
                    {
                        var result = string.Empty;
                        using (var dataStream = response.GetResponseStream())
                        {
                            if (dataStream != null)
                            {
                                using (var reader = new StreamReader(dataStream))
                                {
                                    result = reader.ReadToEnd();
                                }
                            }
                        }

                        bodyResult = response.StatusCode == HttpStatusCode.OK
                                         ? Result.Success(result)
                                         : Result.Failure<string>(result);

                        response.Close();
                    }

                    return bodyResult;
                });

        private async Task<Result<string>> GetBodyUsingHttpClient(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            var response = await _context.Client.SendAsync(request).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                return Result.Failure<string>($"Status code: {response.StatusCode}");
            }

            var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return Result.Success(body);
        }
    }
}
