using System;
using System.IO;
using VerQu.Utility.Streams;
using VerQu.MSGForge;
using VerQu.Formats;
using VerQu;
using System.Collections.Generic;

namespace VerQu.Formats.DataObject
{
	public class MsgObject : EmailObject, IStreamable, IFlushable, IStreamConstructable, IPathObjectConstructable
	{
		private static MsgObject StaticInstance { get; } = new MsgObject(null);

		private bool disposed = false;
		private Email msg;

		public override bool HasChildren => false;
		public override bool InMemory => msg != null || (Stream != null && (Stream is MemoryStream || Stream is CopyLockedMemoryStream));
		public override long Size { get { return GetSize(); } }

		private bool FromPST;

		public Email Msg
		{
			get
			{
				if (msg == null)
				{
					if (Stream == null)
					{
						msg = new Email(Path);
					}
					else
					{
						msg = new Email(Stream);
					}
				}
				return msg;
			}
			set
			{
				if (msg != null)
				{
					try
					{
						msg.Dispose();
					}
					catch { }
				}
				msg = value;
			}
		}

		public static new string AllowedExtension = "msg";

		protected MsgObject(Component SourceComponent) : base(SourceComponent)
		{

		}

		public MsgObject(Component SourceComponent, string Path) : this(SourceComponent, Path, new PathObjectSettings(SourceComponent.Core.Config.Temp, Path), false)
		{

		}

		public MsgObject(Component SourceComponent, string Path, PathObjectSettings Settings, bool load = true) : base(SourceComponent, Path, Settings)
		{
			if (load)
			{
				Msg = new Email(Path);
			}
		}

		public MsgObject(Component SourceComponent, Email email) : this(SourceComponent, email, SourceComponent.Core.GenerateTempFilePath("msg"))
		{

		}

		public MsgObject(Component SourceComponent, Email email, string Path, bool FromPst = false) : this(SourceComponent, email, Path, new PathObjectSettings(SourceComponent.Core.Config.Temp, Path))
		{
			FromPST = FromPst;
		}

		public MsgObject(Component SourceComponent, Email email, string Path, PathObjectSettings Settings) : base(SourceComponent, email, Path, Settings)
		{
			Msg = email;
		}

		public MsgObject(Component SourceComponent, Stream stream) : this(SourceComponent, stream, DeterminePath(SourceComponent, stream, "msg"))
		{

		}

		public MsgObject(Component SourceComponent, Stream stream, string Path) : this(SourceComponent, stream, Path, new PathObjectSettings(SourceComponent.Core.Config.Temp, Path))
		{

		}

		public MsgObject(Component SourceComponent, Stream stream, string Path, PathObjectSettings Settings) : base( SourceComponent, null, Path, Settings)
		{
			Msg = new Email(stream);
		}

		~MsgObject()
		{
			Dispose(false);
		}
		public override VerQu.DataObject Clone()
		{
			var str = new MemoryStream();
			Msg.Save(str, msg.CompoundFile.RootStorage.Size == 0);
			str.Position = 0;

			var msgo = new MsgObject(SourceComponent, str, Path, Settings)
			{
				Core = Core,
				Id = Id,
				OriginalSize = OriginalSize,
				Source = Source
			};

			foreach (KeyValuePair<string, string> tag in Tags)
			{
				msgo.Tags.Add(tag.Key, tag.Value);
			}

			return msgo;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposed)
			{
				return;
			}

			if (disposing)
			{
				msg?.Dispose();
			}

			disposed = true;

			base.Dispose(disposing);
		}

		void IFlushable.FlushToTemp()
		{
			if (string.IsNullOrEmpty(OriginalPath))
			{
				OriginalPath = Path;
			}
			// TODO check size if we should just copy file stream or load into memory
			if (InMemory && msg != null)
			{
				if (!FromPST)
				{
					msg.Save(GetAndUpdateSafePath(), msg.CompoundFile.RootStorage.Size == 0);
				}
				else
				{
					msg.SaveFromPST(GetAndUpdateSafePath());
				}
				if (Stream != null)
				{
					Stream.Dispose();
				}
				msg.Dispose();
				msg = null;
			}
			else
			{
				// copy filestream
				using (FileStream fs = Core.SafeFileStream(GetAndUpdateSafePath(), FileAccess.Write, FileShare.Read))
				{
					Stream.CopyTo(fs);
				}
				Stream.Dispose();
			}
			Stream = new FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.Read);
		}

		void IFlushable.FlushToDirectory(string destination)
		{
			if (string.IsNullOrEmpty(OriginalPath))
			{
				OriginalPath = Path;
			}
			if (!FromPST)
			{
				TransformPath(destination);
				(this as IFlushable).FlushToTemp();
			}
			else
			{
				string s = Core.SafePath(System.IO.Path.Combine(destination, OriginalPath));
				s = s.Replace("/", "\\");
				msg.SaveFromPST(s);
				msg.Dispose();
				msg = null;
			}
		}

		Stream ISharedStreamProducer.GetSharedStream()
		{
			return GetShared();
		}

		private long GetSize()
		{
			//Use base unless only the Forge object is available
			if (msg != null) //Stream is null and data is in memory
			{
				using var ms = new MemoryStream();
				var writeProps = msg.CompoundFile.RootStorage.Size == 0; // We need to know if the message has been saved previously or not. do this by checking the root storage
				msg.Save(ms, writeProps);
				return ms.Length;
			}

			return base.Size;
		}

		protected override Stream GetUnique()
		{
			Stream destination;
			if (Stream == null)
			{
				if (msg == null)
				{
					// No stream means this content is on disk
					if (!string.IsNullOrEmpty(Path))
					{
						destination = new FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.Read);
					}
					else
					{
						throw new Exception("No content available to stream. Stream was not set and Path is missing.");
					}
				}
				else
				{
					destination = new MemoryStream();
					var temp = new MemoryStream();
					var writeProps = msg.CompoundFile.RootStorage.Size == 0;
					msg.Save(temp, writeProps);
					Stream = new CopyLockedMemoryStream(temp)
					{
						Position = 0
					};
					Stream.CopyTo(destination);
					msg.Dispose();
					msg = null;
				}
			}
			else
			{
				if (Stream is FileStream)
				{
					destination = new FileStream((Stream as FileStream).Name, FileMode.Open, FileAccess.Read, FileShare.Read);
				}
				else
				{
					// Should be safe to copy Stream as it is a CopyLockedStream. This will prevent any other threads from trying to copy simultaneously.
					destination = new MemoryStream();
					if (Stream is CopyLockedMemoryStream cls)
					{
						cls.CopyTo(destination);
					}
					else
					{
						Stream.CopyTo(destination);
					}
				}
			}
			return destination;
		}

		VerQu.DataObject IPathObjectConstructable.Construct(Component SourceComponent, PathObject pathObject)
		{
			//return new MsgObject(SourceComponent, pathObject.Path, new PathObjectSettings(pathObject.Path, pathObject.IsRelative, pathObject.IsRelativeTo), false);
			return new MsgObject(SourceComponent, pathObject.Path, new PathObjectSettings(SourceComponent.Core.Config.Temp, pathObject.Path, pathObject.IsRelative, pathObject.IsRelativeTo), false);
		}

		VerQu.DataObject IStreamConstructable.Construct(Component SourceComponent, Stream stream)
		{
			return new MsgObject(SourceComponent, stream);
		}

		public void CloseStream()
		{
			stream?.Dispose();
			stream = null;
		}
	}
}
