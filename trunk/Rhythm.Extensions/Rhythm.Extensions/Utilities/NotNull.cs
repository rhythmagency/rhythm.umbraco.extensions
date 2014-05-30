namespace Rhythm.Extensions.Utilities
{
	/// <summary>
	/// Wraps an instance to ensure it can be passed around with a non-null instance.
	/// </summary>
	/// <typeparam name="T">The type of the instance to wrap.</typeparam>
	/// <remarks>
	/// This is used to work around an MVC bug/limitation,
	/// which seemed to be causing an exception when a null model was passed to a partial view:
	///     https://aspnet.codeplex.com/workitem/8872
	///     http://stackoverflow.com/questions/650393/renderpartial-with-null-model-gets-passed-the-wrong-type
	/// </remarks>
	public class NotNull<T>
	{
		/// <summary>
		/// The wrapped value.
		/// </summary>
		public T Value { get; set; }

		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="source">The value to wrap.</param>
		public NotNull(T source)
		{
			Value = source;
		}
	}

	/// <summary>
	/// Non-generic version of NotNull so that type inference can be used when wrapping the instance.
	/// </summary>
	/// <remarks>
	/// Type inference makes it possible to do, for example, NotNull.From(someVariable).
	/// </remarks>
	public class NotNull
	{
		/// <summary>
		/// Returns a NotNull instance wrapping the specified value.
		/// </summary>
		/// <typeparam name="T">The type of the value to wrap.</typeparam>
		/// <param name="source">The value to wrap.</param>
		/// <returns>The wrapped value.</returns>
		public static NotNull<T> From<T>(T source)
		{
			return new NotNull<T>(source);
		}
	}
}