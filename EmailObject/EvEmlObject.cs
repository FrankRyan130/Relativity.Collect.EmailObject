using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using VerQu.EMLForge;
using VerQu.Formats.DataObject;

namespace VerQu.Formats
{
	public class EvEmlObject : EmlObject
	{
	    
	    public string IndexMetadata { get;set; }
	    
		private EvEmlObject(Component SourceComponent) : base(SourceComponent)
		{

		}

		public EvEmlObject(Component sourceComponent, string Path) : this(sourceComponent, Path, new PathObjectSettings(sourceComponent.Core.Config.Temp, Path))
		{

		}

		public EvEmlObject(Component sourceComponent, string Path, PathObjectSettings Settings) : base(sourceComponent, Path, Settings)
		{
		}

		public EvEmlObject(Component sourceComponent, EML eml) : this(sourceComponent, eml, sourceComponent.Core.GenerateTempFilePath("eml"))
		{

		}

		public EvEmlObject(Component sourceComponent, EML eml, string Path) : this(sourceComponent, eml, Path, new PathObjectSettings(sourceComponent.Core.Config.Temp, Path))
		{

		}

		public EvEmlObject(Component sourceComponent, EML eml, string Path, PathObjectSettings Settings) : base(sourceComponent, eml, Path, Settings)
		{

		}

		public EvEmlObject(Component sourceComponent, Stream stream) : this(sourceComponent, stream, FileStreamObject.DeterminePath(sourceComponent, stream, "eml"))
		{

		}

		public EvEmlObject(Component sourceComponent, Stream stream, string Path) : this(sourceComponent, stream, Path, new PathObjectSettings(sourceComponent.Core.Config.Temp, Path))
		{

		}

		public EvEmlObject(Component sourceComponent, Stream stream, string Path, PathObjectSettings Settings) : base(sourceComponent, Path, Settings)
		{

		}

		~EvEmlObject()
		{
			Dispose(false);
		}
	}
}
