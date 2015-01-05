using Umbraco.Core.Events;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
namespace Rhythm.Extensions.Events {
	public partial class RhythmEventHandler {

		/// <summary>
		/// The Umbraco 6 version of ContentService_Moved.
		/// This version can't use MoveEventArgs.MoveInfoCollection, so it uses
		/// MoveEventArgs.Entity instead.
		/// </summary>
		void Specialized_ContentService_Moved(IContentService sender, MoveEventArgs<IContent> e) {
			var aliases = new [] { e.Entity.ContentType.Alias };
			HandleChangedContent(aliases);
		}

		/// <summary>
		/// The Umbraco 6 version of MediaService_Moved.
		/// This version can't use MoveEventArgs.MoveInfoCollection, so it uses
		/// MoveEventArgs.Entity instead.
		/// </summary>
		void Specialized_MediaService_Moved(IMediaService sender, MoveEventArgs<IMedia> e) {
			var ids = new[] { e.Entity.Id };
			HandleChangedMedia(ids);
		}

	}
}