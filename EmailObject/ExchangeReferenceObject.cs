namespace VerQu.Formats.DataObject
{
	public class ExchangeReferenceObject : EmlObject
	{
		public string ItemId { get; set; }

		private ExchangeReferenceObject(Component sourceComponent, string Path) : base(sourceComponent, Path)
		{

		}
	}
}
