using FluentValidation;
using OneEstate.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace OneEstate.Application.Services.Validation.User
{
    public class UserLoginValidator : AbstractValidator<UserLoginInput>
    {
        public UserLoginValidator()
        {
            ValidateEmail();
            ValidatePassword();
        }

        private void ValidateEmail()
        {
            RuleFor(a => a.Email).MaximumLength(256).WithMessage("Adresa de email trebuie sa aiba maxim 256 de caractere.");
            RuleFor(a => a.Email).NotEmpty().WithMessage("Adresa de email este un camp obligatoriu.");
            RuleFor(a => a.Email).EmailAddress().WithMessage("Adresa de email nu este valida.");
        }

        private void ValidatePassword()
        {
            RuleFor(a => a.Password).MaximumLength(32).WithMessage("Parola trebuie sa aiba maxim 32 de caractere.");
            RuleFor(a => a.Password).MinimumLength(8).WithMessage("Parola trebuie sa aiba minim 8 de caractere.");
        }
    }
}
