using System;

namespace Fazan.Infrastructure.Repositories.PlainTextFile
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using CSharpFunctionalExtensions;
    using Domain.Abstractions;
    using Domain.Models;

    public class WordsRepository : IWordsRepository
    {
        private readonly string destinationPath;

        public WordsRepository(string destinationPath)
        {
            this.destinationPath = destinationPath;
        }

        public Task<Result> CreateBulk(IEnumerable<Word> data) =>
            Task.Run(() => Result.Try(() => File.AppendAllLines(destinationPath, data.Select(x => x.Value))));

        public Task<Result> Calculate() => throw new NotImplementedException();

        public Task<Result<int>> Commit() => Task.Run(() => Result.Success(0));

        public Task<Result<Word>> GetMostEasyWord(string firstTwoCharacters)
        {
            throw new NotImplementedException();
        }

        public Task<Result<Word>> GetHardestWord(string firstTwoCharacters)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<Result<Word>> GetAWord(string firstTwoCharacters, IList<string> excludedWords) => throw new NotImplementedException();

        /// <inheritdoc />
        public Task<bool> Exists(string word) => throw new NotImplementedException();
    }
}
