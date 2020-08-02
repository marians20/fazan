using System;

namespace Fazan.Infrastructure.Repositories.SqliteRepository
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using CSharpFunctionalExtensions;
    using Domain.Models;
    using Fazan.Domain.Abstractions;
    using Microsoft.EntityFrameworkCore;

    public class WordsRepository : IWordsRepository
    {
        private readonly FazanSqliteDbContext context;

        public WordsRepository(FazanSqliteDbContext context)
        {
            this.context = context;
        }

        public Task<Result> CreateBulk(IEnumerable<Word> data) =>
            Result.Try(async () =>
            {
                await context.AddRangeAsync(data).ConfigureAwait(false);
                await context.SaveChangesAsync().ConfigureAwait(false);
            }).OnFailure(async () =>
            {
                foreach (var word in data)
                {
                    await Result.Try(async () =>
                        {
                            await context.AddAsync(word).ConfigureAwait(false);
                            await context.SaveChangesAsync().ConfigureAwait(false);
                        }).OnFailure(error => {/*log the error*/}).ConfigureAwait(false);
                }
            });

        public async Task<Result> Create(Word entity)
        {
            try
            {
                entity.DerivedWordsCount = await context.Words
                                               .CountAsync(x => x.FirstLetters == entity.LastLetters).ConfigureAwait(false);
                await context.AddAsync(entity).ConfigureAwait(false);
                return Result.Success();
            }
            catch (Exception e)
            {
                return Result.Failure(e.Message);
            }
        }

        public async Task<Result> Calculate()
        {
            var entities = context.Words;
            foreach (var entity in entities)
            {
                entity.DerivedWordsCount = await context.Words.CountAsync(e => e.FirstLetters == entity.LastLetters)
                                               .ConfigureAwait(false);
            }

            return await Commit().ConfigureAwait(false);
        }

        public Task<Result<int>> Commit() => Result.Try(() => context.SaveChangesAsync());

        public Task<Result<Word>> GetMostEasyWord(string firstTwoCharacters)
        {
            var word = GetWordsQuery(firstTwoCharacters).OrderByDescending(x => x.DerivedWordsCount).FirstOrDefault();
            return Task.Run(() => word != null ? Result.Success(word) : Result.Failure<Word>("Not found."));
        }

        public Task<Result<Word>> GetHardestWord(string firstTwoCharacters)
        {
            var word = GetWordsQuery(firstTwoCharacters).OrderBy(x => x.DerivedWordsCount).FirstOrDefault();
            return Task.Run(() => word != null ? Result.Success(word) : Result.Failure<Word>("Not found."));
        }

        /// <inheritdoc />
        public async Task<Result<Word>> GetAWord(string firstTwoCharacters, IList<string> excludedWords)
        {
            var words = GetWordsQuery(firstTwoCharacters).OrderBy(x => x.DerivedWordsCount);
            var filteredWords = words.Where(word => !excludedWords.Contains(word.Value));
            if (!filteredWords.Any())
            {
                return Result.Failure<Word>("Not found.");
            }

            var randomPosition = new Random().Next(await filteredWords.CountAsync().ConfigureAwait(false));
            return (await words.Skip(randomPosition).Take(1).ToListAsync().ConfigureAwait(false)).FirstOrDefault();
        }

        /// <inheritdoc />
        public Task<bool> Exists(string word) =>
            context.Words.AnyAsync(x => x.Value == word);

        private IQueryable<Word> GetWordsQuery(string firstTwoCharacters)
        {
            firstTwoCharacters = firstTwoCharacters.ToLower();
            return context.Words.Where(w => w.FirstLetters == firstTwoCharacters);
        }
    }
}
