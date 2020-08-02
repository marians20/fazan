using System.Collections.Generic;

namespace Fazan.Domain.Services.WordsServices
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Abstractions;
    using CSharpFunctionalExtensions;
    using FluentValidation;
    using Models;

    public sealed class WordsService : IWordsService
    {
        private readonly IWordsRepository repository;

        private readonly AbstractValidator<Word> wordValidator;

        public WordsService(AbstractValidator<Word> wordValidator, IWordsRepository repository)
        {
            this.wordValidator = wordValidator;
            this.repository = repository;
        }

        public async Task<Result> CreateBulk(IList<string> words)
        {
            var errors = new List<string>();
            var parsedWords = words
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrEmpty(x) && x.Length >= Constants.LettersCount)
                .Select(word => new Word
            {
                Id = Guid.NewGuid(),
                Value = word,
                FirstLetters = word.Substring(0, Constants.LettersCount),
                LastLetters = word.Substring(word.Length - Constants.LettersCount)
            });

            var wordsToBeAdded = new List<Word>();

            foreach (var word in parsedWords)
            {
                var validationResult = wordValidator.Validate(word);
                if (validationResult.IsValid)
                {
                    wordsToBeAdded.Add(word);
                }
                else
                {
                    errors.AddRange(validationResult.Errors.Select(err => $"{err.ErrorCode}|{err.ErrorMessage}"));
                }

            }

            return await repository.CreateBulk(wordsToBeAdded);
        }

        public Task<Result<string>> GetMostEasyWord(string firstTwoCharacters) =>
            repository.GetMostEasyWord(firstTwoCharacters).Map(word => word?.Value ?? string.Empty);

        public Task<Result<string>> GetHardestWord(string firstTwoCharacters) =>
            repository.GetHardestWord(firstTwoCharacters).Map(word => word?.Value ?? string.Empty);

        /// <inheritdoc />
        public Task<Result<string>> GetAWord(string firstTwoCharacters, IList<string> excludedWords) =>
            repository.GetAWord(firstTwoCharacters, excludedWords).Map(word => word?.Value ?? string.Empty);

        /// <inheritdoc />
        public Task<Result> Calculate() => repository.Calculate();

        /// <inheritdoc />
        public Task<bool> Exists(string word) => repository.Exists(word);
    }
}
