using System;

namespace Fazan.Domain.Models
{
    public class Word
    {
        public Guid Id { get; set; }

        public string Value { get; set; }

        public string LastLetters { get; set; }

        public string FirstLetters { get; set; }

        public int DerivedWordsCount { get; set; }
    }
}
