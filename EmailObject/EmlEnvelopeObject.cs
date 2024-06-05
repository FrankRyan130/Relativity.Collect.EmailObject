using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MimeKit;
using Newtonsoft.Json;
using VerQu.EMLForge;

namespace VerQu.Formats.DataObject
{
	public class EmlEnvelopeObject : EmlObject
	{
		private static EmlEnvelopeObject StaticInstance { get; } = new EmlEnvelopeObject(null);

		[JsonIgnore]
		private bool disposed = false;

		public List<MailboxAddress> To { get; } = new List<MailboxAddress>();
		public List<MailboxAddress> Cc { get; } = new List<MailboxAddress>();
		public List<MailboxAddress> Bcc { get; } = new List<MailboxAddress>();
		public List<MailboxAddress> Recipient { get; } = new List<MailboxAddress>();

		// TODO distribution lists, forwarded by

		public EmlEnvelopeObject(Component sourceComponent, string Path) : base(sourceComponent, Path)
		{
		}

		public EmlEnvelopeObject(Component sourceComponent, EML eml) : base(sourceComponent, eml)
		{
		}

		public EmlEnvelopeObject(Component sourceComponent, Stream stream) : base(sourceComponent, stream)
		{
		}

		public EmlEnvelopeObject(Component sourceComponent, string Path, PathObjectSettings Settings) : base(sourceComponent, Path, Settings)
		{
		}

		public EmlEnvelopeObject(Component sourceComponent, EML eml, string Path) : base(sourceComponent, eml, Path)
		{
		}

		public EmlEnvelopeObject(Component sourceComponent, Stream stream, string Path) : base(sourceComponent, stream, Path)
		{
		}

		public EmlEnvelopeObject(Component sourceComponent, EML eml, string Path, PathObjectSettings Settings) : base(sourceComponent, eml, Path, Settings)
		{
		}

		public EmlEnvelopeObject(Component sourceComponent, Stream stream, string Path, PathObjectSettings Settings) : base(sourceComponent, stream, Path, Settings)
		{
		}

		protected EmlEnvelopeObject(Component SourceComponent) : base(SourceComponent)
		{
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
	}
}
