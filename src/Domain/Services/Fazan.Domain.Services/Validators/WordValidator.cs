namespace Fazan.Domain.Services.Validators
{
    using FluentValidation;
    using Models;
    using Properties;

    public class WordValidator : AbstractValidator<Word>
    {
        public WordValidator()
        {
            RuleFor(word => word.Value).NotNull()
                .WithMessage(string.Format(Resources.MsgNotNullValidator, nameof(Word.Value)));
            RuleFor(word => word.Value).NotEmpty()
                .WithMessage(string.Format(Resources.MsgNotEmptyValidator, nameof(Word.Value)));
            RuleFor(word => word.Value).MinimumLength(Constants.LettersCount)
                .WithMessage(string.Format(Resources.MsgMinLengthValidator, nameof(Word.Value), Constants.LettersCount));
        }
    }
}
