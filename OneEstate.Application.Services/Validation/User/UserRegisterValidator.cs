using FluentValidation;
using OneEstate.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace OneEstate.Application.Services.Validation.User
{
    public class UserRegisterValidator : AbstractValidator<UserRegisterInput>
    {
        private readonly IUserAppService _userAppService;

        public UserRegisterValidator(IUserAppService userAppService)
        {
            _userAppService = userAppService;

            ValidateFirstLastNames();
            ValidateEmail();
            ValidatePassword();
        }

        private void ValidateFirstLastNames()
        {
            RuleFor(a => a.FirstName).MaximumLength(50).WithMessage("Prenumele trebuie sa aiba maxim 50 de caractere.");
            RuleFor(a => a.FirstName).NotEmpty().WithMessage("Prenumele trebuie completat.");

            RuleFor(a => a.LastName).MaximumLength(50).WithMessage("Numele trebuie sa aiba maxim 50 de caractere.");
            RuleFor(a => a.LastName).NotEmpty().WithMessage("Numele trebuie completat.");
        }

        private void ValidateEmail()
        {
            RuleFor(a => a.Email).MaximumLength(256).WithMessage("Adresa de email trebuie sa aiba maxim 256 de caractere.");
            RuleFor(a => a.Email).NotEmpty().WithMessage("Adresa de email este un camp obligatoriu.");
            RuleFor(a => a.Email).EmailAddress().WithMessage("Adresa de email nu este valida.");

            RuleFor(a => a).CustomAsync(async (input, context, ct) =>
            {
                var isEmailUnique = await _userAppService.IsEmailUnique(input.Email, null);
                if (!isEmailUnique)
                {
                    context.AddFailure("Email", "Adresa de email este deja folosita.");
                }
            });
        }

        private void ValidatePassword()
        {
            RuleFor(a => a.Password).MaximumLength(32).WithMessage("Parola trebuie sa aiba maxim 32 de caractere.");
            RuleFor(a => a.Password).MinimumLength(8).WithMessage("Parola trebuie sa aiba minim 8 de caractere.");
        }
    }
}
