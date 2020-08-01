using System;

namespace Fazan.Infrastructure.Repositories.SqliteRepository
{
    using System.Collections.Generic;
    using System.Data.Entity;
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
                        await context.AddAsync(word);
                        await context.SaveChangesAsync();
                    }).OnFailure(error => {/*log the error*/});
                }
            });

        public async Task<Result> Create(Word entity)
        {
            try
            {
                entity.DerivedWordsCount = await EntityFrameworkQueryableExtensions.CountAsync(context.Words, x => x.FirstLetters == entity.LastLetters);
                await context.AddAsync(entity);
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
                entity.DerivedWordsCount = await EntityFrameworkQueryableExtensions.CountAsync(context.Words, e => e.FirstLetters == entity.LastLetters);
            }

            return await Commit();
        }

        public Task<Result<int>> Commit() => Result.Try(() => context.SaveChangesAsync());

        public async Task<Result<Word>> GetMostEasyWord(string firstTwoCharacters)
        {
            firstTwoCharacters = firstTwoCharacters.ToLower();
            var word = context.Words.Where(w => w.FirstLetters == firstTwoCharacters)
                .OrderByDescending(x => x.DerivedWordsCount)
                .FirstOrDefault();
            return await Task.Run(() => word != null ? Result.Success(word) : Result.Failure<Word>("Not found."));
        }

        public async Task<Result<Word>> GetHardestWord(string firstTwoCharacters)
        {
            firstTwoCharacters = firstTwoCharacters.ToLower();
            var word = context.Words.Where(w => w.FirstLetters == firstTwoCharacters)
                .OrderBy(x => x.DerivedWordsCount)
                .FirstOrDefault();
            return await Task.Run(() => word != null ? Result.Success(word) : Result.Failure<Word>("Not found."));
        }
    }
}
