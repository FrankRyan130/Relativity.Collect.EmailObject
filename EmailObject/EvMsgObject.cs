using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using VerQu.Formats.DataObject;
using VerQu.MSGForge;

namespace VerQu.Formats
{
	public class EvMsgObject : MsgObject
	{
	    
	    public string IndexMetadata { get;set; }
	    
		private EvMsgObject(Component SourceComponent) : base(SourceComponent)
		{

		}

		public EvMsgObject(Component SourceComponent, string Path) : this(SourceComponent, Path, new PathObjectSettings(SourceComponent.Core.Config.Temp, Path), false)
		{

		}

		public EvMsgObject(Component SourceComponent, string Path, PathObjectSettings Settings, bool load = true) : base(SourceComponent, Path, Settings)
		{
			
		}

		public EvMsgObject(Component SourceComponent, Email email) : this(SourceComponent, email, SourceComponent.Core.GenerateTempFilePath("msg"))
		{

		}

		public EvMsgObject(Component SourceComponent, Email email, string Path) : this(SourceComponent, email, Path, new PathObjectSettings(SourceComponent.Core.Config.Temp, Path))
		{

		}

		public EvMsgObject(Component SourceComponent, Email email, string Path, PathObjectSettings Settings) : base(SourceComponent, email, Path, Settings)
		{
			
		}

		public EvMsgObject(Component SourceComponent, Stream stream) : this(SourceComponent, stream, DeterminePath(SourceComponent, stream, "msg"))
		{

		}

		public EvMsgObject(Component SourceComponent, Stream stream, string Path) : this(SourceComponent, stream, Path, new PathObjectSettings(SourceComponent.Core.Config.Temp, Path))
		{

		}

		public EvMsgObject(Component SourceComponent, Stream stream, string Path, PathObjectSettings Settings) : base(SourceComponent, stream, Path, Settings)
		{
			
		}

		~EvMsgObject()
		{
			Dispose(false);
		}
	}
}
