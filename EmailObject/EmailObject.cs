using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using VerQu.Formats;

namespace VerQu.Formats
{
	public class EmailObject : FileStreamObject
	{
		[JsonIgnore]
		private bool disposed = false;

		public bool Enveloped { get; set; } = false;

		internal EmailObject(Component SourceComponent) : base(SourceComponent)
		{

		}

		public EmailObject(Component SourceComponent, Stream stream, string Path) : base(SourceComponent, stream, Path)
		{

		}

		public EmailObject(Component SourceComponent, string Path, PathObjectSettings Settings) : base(SourceComponent, Path, Settings)
		{

		}

		protected EmailObject(Component SourceComponent, object o, string Path, PathObjectSettings Settings) : base(SourceComponent, o, Path, Settings)
		{

		}

		public override VerQu.DataObject Clone()
		{
			throw new NotImplementedException();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposed)
			{
				return;
			}
			if (disposing)
			{

			}
			disposed = true;
			base.Dispose(disposing);
		}

		public void DisposeStream()
		{
			stream?.Dispose();
			stream = null;
		}
	}
}
