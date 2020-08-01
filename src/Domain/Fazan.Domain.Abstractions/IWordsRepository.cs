namespace Fazan.Domain.Abstractions
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using CSharpFunctionalExtensions;
    using Models;

    public interface IWordsRepository
    {
        Task<Result> CreateBulk(IEnumerable<Word> data);

        Task<Result> Calculate();

        Task<Result<int>> Commit();

        Task<Result<Word>> GetMostEasyWord(string firstTwoCharacters);

        Task<Result<Word>> GetHardestWord(string firstTwoCharacters);
    }
}
