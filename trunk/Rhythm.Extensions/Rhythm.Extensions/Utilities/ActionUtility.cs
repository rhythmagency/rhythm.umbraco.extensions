namespace Rhythm.Extensions.Utilities {

	// Namespaces.
	using System;
	using System.Threading;

	/// <summary>
	/// Utility to help with actions.
	/// </summary>
	public class ActionUtility {

		/// <summary>
		/// Attempts to execute an action in the specified time limit,
		/// optionally executing an alternate action on failure.
		/// </summary>
		/// <param name="action">The action to attempt to execute.</param>
		/// <param name="limit">The duration of time the action is allowed to execute for.</param>
		/// <param name="alternate">
		/// Optional.
		/// The alternate action to execute if the first one fails to execute in the given time limit.
		/// </param>
		/// <returns>
		/// True, if the action executed within the given time limit; otherwise, false.
		/// </returns>
		/// <remarks>
		/// If the action does not execute within the given time limit, it will be aborted.
		/// If the abort operation doesn't complete quickly,
		/// the alternate action will be executed before the abort operation completes.
		/// </remarks>
		public static bool Attempt(Action action, TimeSpan limit, Action alternate = null) {

			// Validate input.
			if (action == null) {
				if (alternate != null) {
					alternate();
				}
				return false;
			}
			else {

				// Variables.
				Exception exception = null;
				object hadException = false;
				object requestingAbort = false;

				// Wrap in a thread so the duration can be limited.
				Thread t = new Thread(new ThreadStart(() => {

					// Attempt to execute the action.
					try {
						action();
					}
					catch (ThreadAbortException ex) {

						// Avoid thread abort exceptions.
						if ((bool)requestingAbort) {
							try {
								Thread.ResetAbort();
							}
							catch { }
						}
						else {
							exception = ex;
							hadException = true;
						}

					}
					catch (Exception ex) {

						// Save other exceptions.
						exception = ex;
						hadException = true;

					}

				}));
				
				// Try to execute the thread within the time limit.
				t.Start();
				var success = t.Join(limit);
				if (success) {

					// Propagate exceptions.
					if ((bool)hadException) {
						throw exception;
					}

					// Success.
					return true;

				}
				else {

					// Abort thread and try alternate.
					var abortLimit = TimeSpan.FromSeconds(Math.Max(0.1, Math.Min(limit.TotalSeconds / 2, 1)));
					requestingAbort = true;
					var abortThread = new Thread(() => {
						try {
							t.Abort();
						}
						catch (ThreadAbortException) {
							try {
								Thread.ResetAbort();
							}
							catch { }
						}
						catch { }
					});
					abortThread.Start();
					abortThread.Join(abortLimit);
					if (alternate != null) {
						alternate();
					}
					return false;

				}

			}
		}

		/// <summary>
		/// Runs an action in a thread an ensures no exceptions escape the thread.
		/// </summary>
		/// <param name="action">The action to run in the thread.</param>
		/// <param name="errorHandler">The action to call if an exception is caught. Optional.</param>
		/// <returns>The thread (already started).</returns>
		public static Thread SafeThread(Action action, Action<Exception> errorHandler = null)
		{
			var thread = new Thread(() =>
			{
				try {
					action();
				}
				catch (ThreadAbortException ex) {
					Thread.ResetAbort();
					if (errorHandler != null) {
						errorHandler(ex);
					}
				}
				catch (Exception ex) {
					if (errorHandler != null) {
						errorHandler(ex);
					}
				}
				catch {
					if (errorHandler != null) {
						errorHandler(null);
					}
				}
			});
			thread.Start();
			return thread;
		}

	}

}