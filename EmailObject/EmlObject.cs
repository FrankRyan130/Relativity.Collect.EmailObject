using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using VerQu.EMLForge;
using VerQu.Formats;
using VerQu.Utility.Streams;

namespace VerQu.Formats.DataObject
{
	public class EmlObject : EmailObject, IStreamable, IFlushable, IPathObjectConstructable, IStreamConstructable
	{
		private static EmlObject StaticInstance { get; } = new EmlObject(null);

		[JsonIgnore]
		private bool disposed = false;

		private EML eml;

		public static new string AllowedExtension = "eml";
		public override bool HasChildren => false;
		public override bool InMemory => eml != null || (Stream != null && (Stream is MemoryStream || Stream is CopyLockedMemoryStream));
		public override long Size { get { return GetSize(); } }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public EML EML
		{
			get
			{
				if (eml == null)
				{
					using Stream stream = (this as ISharedStreamProducer).GetSharedStream();
					eml = new EML(stream);
				}
				return eml;
			}
			set
			{
				eml = value;
			}
		}

		protected EmlObject(Component SourceComponent) : base(SourceComponent)
		{

		}

		public EmlObject(Component sourceComponent, string Path) : this(sourceComponent, Path, new PathObjectSettings(sourceComponent.Core.Config.Temp, Path))
		{

		}

		public EmlObject(Component sourceComponent, string Path, PathObjectSettings Settings) : base(sourceComponent, Path, Settings)
		{
			eml = new EML(Path);
		}

		public EmlObject(Component sourceComponent, EML eml) : this(sourceComponent, eml, sourceComponent.Core.GenerateTempFilePath("eml"))
		{

		}

		public EmlObject(Component sourceComponent, EML eml, string Path) : this(sourceComponent, eml, Path, new PathObjectSettings(sourceComponent.Core.Config.Temp, Path))
		{

		}

		public EmlObject(Component sourceComponent, EML eml, string Path, PathObjectSettings Settings) : base(sourceComponent, eml, Path, Settings)
		{
			this.eml = eml;
		}

		public EmlObject(Component sourceComponent, Stream stream) : this(sourceComponent, stream, FileStreamObject.DeterminePath(sourceComponent, stream, "eml"))
		{

		}

		public EmlObject(Component sourceComponent, Stream stream, string Path) : this(sourceComponent, stream, Path, new PathObjectSettings(sourceComponent.Core.Config.Temp, Path))
		{

		}

		public EmlObject(Component sourceComponent, Stream stream, string Path, PathObjectSettings Settings) : base(sourceComponent, Path, Settings)
		{
			eml = new EML(stream);
		}

		public override VerQu.DataObject Clone()
		{
			var str = new MemoryStream();
			EML.Save(str);
			str.Position = 0;

			var emlo = new EmlObject(SourceComponent, str, Path, Settings)
			{
				Core = Core,
				Id = Id,
				OriginalSize = OriginalSize,
				Source = Source
			};

			foreach (KeyValuePair<string, string> tag in Tags)
			{
				emlo.Tags.Add(tag.Key, tag.Value);
			}

			return emlo;
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

			eml?.DisposeAttachments();
			eml = null;

			disposed = true;
			base.Dispose(disposing);
		}

		void IFlushable.FlushToTemp()
		{
			if (eml != null)
			{
				if (Stream != null)
				{
					Stream.Dispose();
				}

				var path = GetAndUpdateSafePath();
				try
				{
					using (FileStream fs = Core.SafeFileStream(path, FileAccess.Write, FileShare.Read))
					{
						eml.Save(fs);
					}
				}
				catch (Exception e)
				{
					TryDeleteFile(path);
					throw e;
				}
				Stream = new FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.Read);
				eml.DisposeAttachments();
				eml = null;
			}
		}

		void IFlushable.FlushToDirectory(string destination)
		{
			TransformPath(destination);
			(this as IFlushable).FlushToTemp();
		}

		VerQu.DataObject IPathObjectConstructable.Construct(Component sourceComponent, PathObject pathObject)
		{
			return new EmlObject(sourceComponent, pathObject.Path, new PathObjectSettings(Core.Config.Temp, pathObject.Path, pathObject.IsRelative, pathObject.IsRelativeTo));
		}

		VerQu.DataObject IStreamConstructable.Construct(Component sourceComponent, Stream stream)
		{
			if (stream is FileStream fs)
			{
				try
				{
					return new EmlObject(sourceComponent, stream, fs.Name);
				}
				catch (Exception e)
				{
					Log(e + " " + fs.Name, MessageType.UnhandledException, LogLevel.Error);
					return null;
				}
			}
			return new EmlObject(sourceComponent, stream);
		}

		protected override Stream GetUnique()
		{
			if (Stream != null)
			{
				// call filestream get unique if stream is available
				return base.GetUnique();
			}

			var stream = new MemoryStream();
			EML.Save(stream);
			stream.Position = 0;
			return stream;
		}

		private long GetSize()
		{
			//Use base unless only the Forge object is available
			if (eml != null) //Stream is null and data is in memory
			{
				using var ms = new MemoryStream();
				EML.Save(ms);
				return ms.Length;
			}

			return base.Size;
		}

		private void TryDeleteFile(string path)
		{
			if (File.Exists(path))
			{
				try
				{
					File.Delete(path);
				}
				catch (Exception e)
				{
					Log($"Failed to delete bad data at: {path} | Exception: {e.ToString()}", MessageType.Dispose, LogLevel.Warning);
				}
			}
		}
	}
}
