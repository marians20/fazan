namespace Fazan.Domain.Abstractions
{
    using CSharpFunctionalExtensions;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models;

    public interface IWordsService
    {
        Task<Result> CreateBulk(IList<string> words);

        Task<Result<string>> GetMostEasyWord(string firstTwoCharacters);

        Task<Result<string>> GetHardestWord(string firstTwoCharacters);
    }
}
