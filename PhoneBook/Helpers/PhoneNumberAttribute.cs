using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace PhoneBook.Validators
{
    public class PhoneNumberAttribute : ValidationAttribute
    {
        private static readonly Regex _bgPhoneRegex = new Regex(
            @"^(?:\+3598[7-9]\d{7}|08[7-9]\d{7})$",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public PhoneNumberAttribute()
        {
            ErrorMessage = "Invalid Bulgarian phone number format.";
        }

        public override bool IsValid(object value)
        {
            if (value == null)
                return false;

            var str = value as string;
            if (string.IsNullOrWhiteSpace(str))
                return false;

            return _bgPhoneRegex.IsMatch(str);
        }
    }
}
