namespace ExternalNonChangableLibrary
{
	public class DataObjectInfo
	{
		public string Id { get; set; }

		public int Version { get; set; } = 10;

		public bool IsLocked { get; set; }
	}
}
