namespace Fazan.Domain.Services.Extensions
{
    using HtmlAgilityPack;

    public static class StringExtensions
    {
        public static HtmlDocument ToHtmlDocument(this string value)
        {
            var result = new HtmlDocument();
            result.LoadHtml(value);
            return result;
        }
    }
}
