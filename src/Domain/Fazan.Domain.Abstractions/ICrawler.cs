namespace Fazan.Domain.Abstractions
{
    using System.Threading.Tasks;
    using CSharpFunctionalExtensions;
    using HtmlAgilityPack;

    public interface ICrawler
    {
        Task<Result<HtmlDocument>> GetPageContent(string url);
        Task<Result> ReadAllPages();
    }
}