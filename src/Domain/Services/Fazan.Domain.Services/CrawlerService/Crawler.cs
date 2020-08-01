﻿namespace Fazan.Domain.Services.CrawlerService
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
        private readonly ICrawlerContext context;

        public Crawler(ICrawlerContext context)
        {
            this.context = context;
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
            var url = context.BaseUrl;
            var uriBuildr = new UriBuilder(url);
            var scheme = uriBuildr.Scheme;
            var host = uriBuildr.Host;
            var port = uriBuildr.Port;

            Result<HtmlDocument> result;
            do
            {
                result = await GetPageContent(url)
                    .Tap(htmlDocument =>
                    {
                        var words = context.DomProcessor.GetWordsFromDoc(htmlDocument);
                        Console.WriteLine(Resources.Crawler_ReadAllPages_Adding_words_from__0__to__1_, words.First(), words.Last());
                        context.WordsService.CreateBulk(words);
                        var nextUrl = context.DomProcessor.GetNextPageUrl(htmlDocument);
                        oldUrl = url;
                        url = $"{scheme}://{host}:{port}{nextUrl}";
                    });
            } while (result.IsSuccess && !url.Equals(oldUrl));

            return result;
        }

        private async Task<Result<string>> GetBodyUsingWebRequest(string url) =>
            await Task.Run(() =>
            {
                var request = (HttpWebRequest) WebRequest.Create(url);
                request.AllowAutoRedirect = true;
                Result<string> bodyResult;
                using (var response = (HttpWebResponse) (request.GetResponse()))
                {
                    string result;
                    using (var dataStream = response.GetResponseStream())
                    {
                        using (var reader = new StreamReader(dataStream))
                        {
                            result = reader.ReadToEnd();
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
            var response = await context.Client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                return Result.Failure<string>($"Status code: {response.StatusCode}");
            }

            var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return Result.Success(body);
        }
    }
}
