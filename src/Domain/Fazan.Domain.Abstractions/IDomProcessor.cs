namespace Fazan.Domain.Abstractions
{
    using HtmlAgilityPack;
    using System.Collections.Generic;

    public interface IDomProcessor
    {
        IList<string> GetWordsFromDoc(HtmlDocument htmlDoc);
        string GetNextPageUrl(HtmlDocument htmlDoc);
    }
}