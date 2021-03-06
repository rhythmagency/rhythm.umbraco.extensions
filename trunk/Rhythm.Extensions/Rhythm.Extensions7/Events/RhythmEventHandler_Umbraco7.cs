﻿using System.Linq;
using Umbraco.Core.Events;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
namespace Rhythm.Extensions.Events {
	public partial class RhythmEventHandler {

		/// <summary>
		/// The Umbraco 7 version of ContentService_Moved.
		/// This version uses MoveEventArgs.MoveInfoCollection.
		/// </summary>
		void Specialized_ContentService_Moved(IContentService sender, MoveEventArgs<IContent> e) {
			var aliases = e.MoveInfoCollection.Select(x => x.Entity.ContentType.Alias)
				.Distinct().ToList();
			HandleChangedContent(aliases);
		}

		/// <summary>
		/// The Umbraco 7 version of MediaService_Moved.
		/// This version uses MoveEventArgs.MoveInfoCollection.
		/// </summary>
		void Specialized_MediaService_Moved(IMediaService sender, MoveEventArgs<IMedia> e) {
			var ids = e.MoveInfoCollection.Select(x => x.Entity.Id).ToList();
			HandleChangedMedia(ids);
		}

	}
}