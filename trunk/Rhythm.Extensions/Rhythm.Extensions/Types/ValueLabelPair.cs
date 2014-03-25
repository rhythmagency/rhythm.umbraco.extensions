using System;

namespace Rhythm.Extensions.Types {
	
	/// <summary>
	/// Defines a value/label pair that can be set or retrieved.
	/// </summary>
	/// <typeparam name="TValue">The type of the value.</typeparam>
	/// <typeparam name="TLabel">The type of the label.</typeparam>
	[Serializable]
	public struct ValueLabelPair<TValue, TLabel> {
		
		private readonly TValue value;
		private readonly TLabel label;
		
		/// <summary>
		/// The value.
		/// </summary>
		public TValue Value {
			get {
				return value;
			}
		}
		
		/// <summary>
		/// The label.
		/// </summary>
		public TLabel Label {
			get {
				return label;
			}
		}
		
		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="label">The label.</param>
		public ValueLabelPair(TValue value, TLabel label) {
			this.value = value;
			this.label = label;
		}
		
	}
	
}