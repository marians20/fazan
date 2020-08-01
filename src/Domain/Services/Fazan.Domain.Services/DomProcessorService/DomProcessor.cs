namespace Fazan.Domain.Services.DomProcessorService
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Abstractions;
    using HtmlAgilityPack;
    using Properties;

    public sealed class DomProcessor : IDomProcessor
    {
        public IList<string> GetWordsFromDoc(HtmlDocument htmlDoc)
        {
            var list = htmlDoc.DocumentNode.SelectNodes(Resources.UlXpath).Descendants().Select(x => x.FirstChild)
                .Where(x => x != null).Where(x => x.Name == "a").Select(x => x.InnerHtml).ToList();

            return list;
        }

        public string GetNextPageUrl(HtmlDocument htmlDoc)
        {
            var nextPageLinkNode = htmlDoc.DocumentNode.SelectSingleNode(Resources.NextPageXPath)
                                   ?? htmlDoc.DocumentNode.SelectSingleNode(Resources.NextPageXPathFromFirstPage);

            var nextPageLink = nextPageLinkNode.Attributes.FirstOrDefault(x => x.Name == "href").Value;

            var decodedUrl = HttpUtility.HtmlDecode(nextPageLink);

            return decodedUrl;
        }
    }
}
