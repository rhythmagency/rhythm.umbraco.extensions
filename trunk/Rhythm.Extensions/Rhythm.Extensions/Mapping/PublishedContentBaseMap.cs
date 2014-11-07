namespace Rhythm.Extensions.Mapping
{
	internal class PublishedContentBaseMap : ContentMapping<PublishedContentBase>
	{
		public PublishedContentBaseMap()
		{
			Property(x => x.Id, c => c.Id);
			Property(x => x.Name, c => c.Name);
			Property(x => x.Url, c => c.Url);
			Property(x => x.PublishedContent, c => c);
		}
	}
}