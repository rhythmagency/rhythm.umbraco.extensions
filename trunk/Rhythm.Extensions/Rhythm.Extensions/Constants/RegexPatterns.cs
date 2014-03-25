namespace Rhythm.Extensions.Constants {
	public static class RegexPatterns {
        /// <summary>
        /// If validating an MVC model, consider using these attributes instead:
        ///     [DataType(DataType.EmailAddress)]
        ///     [EmailAddress()]
        /// </summary>
		public const string EmailAddress = @"^.+@.+\..+$";
        public const string Date = @"^(0?[1-9]|10|11|12)/(0?[1-9]|1[0-9]|2[0-9]|3[0-1])/[0-9]{4}$";
        public const string Phone = @"^(.*[0-9]){7}.*$";
	}
}