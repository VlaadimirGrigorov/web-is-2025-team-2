namespace PhoneBook.Helpers
{
    public static class GenericHelpers
    {
        public static bool IsValidPhoneNumber(string? phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber) || phoneNumber.Length > 10)
                return false;

            return phoneNumber.All(char.IsDigit);
        }
    }
}
