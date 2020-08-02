namespace Fazan.Domain.Abstractions
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using CSharpFunctionalExtensions;

    public interface IWordsService
    {
        Task<Result> CreateBulk(IList<string> words);

        Task<Result<string>> GetMostEasyWord(string firstTwoCharacters);

        Task<Result<string>> GetHardestWord(string firstTwoCharacters);

        Task<Result<string>> GetAWord(string firstTwoCharacters, IList<string> excludedWords);

        Task<Result> Calculate();

        Task<bool> Exists(string word);
    }
}
