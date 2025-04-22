using FluentValidation;
using System.Text.RegularExpressions;

namespace ATONTestTask.Extentions
{
    public static partial class ValidateExtentions
    {
        public static IRuleBuilderOptions<T, string> Login<T>(this IRuleBuilderOptions<T, string> ruleBuilder)
        {
            var regex = LoginRegex();
            return ruleBuilder.Matches(regex);
        }

        public static IRuleBuilderOptions<T, string> Password<T>(this IRuleBuilderOptions<T, string> ruleBuilder)
        {
            var regex = PasswordRegex();
            return ruleBuilder.Matches(regex);
        }


        public static IRuleBuilderOptions<T, string> Name<T>(this IRuleBuilderOptions<T, string> ruleBuilder)
        {
            var regex = NameRegex();
            return ruleBuilder.Matches(regex);
        }

       
        [GeneratedRegex(@"\p{L}+\p{M}*")]
        private static partial Regex NameRegex();

        [GeneratedRegex(@"^[A-Za-z0-9]+$")]
        private static partial Regex LoginRegex();

        [GeneratedRegex(@"^[A-Za-z0-9]+$")]
        private static partial Regex PasswordRegex();
    }
}
