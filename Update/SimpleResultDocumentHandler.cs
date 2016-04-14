using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using XmlPrime.Serialization;

namespace XmlPrime.Samples.Update
{
	/// <summary>
	/// Handles the writing of updated or created documents to the file system.
	/// </summary>
	/// <remarks>
	/// <para>This class attempts to ensure that any updated documents are backed up.</para>
	/// </remarks>
	internal class SimpleResultDocumentHandler : IResultDocumentHandler
	{
		#region Private Static Methods

		/// <summary>
		/// Gets a temporary file in the same directory as the specified path.
		/// </summary>
		/// <param name="localPath">The local path.</param>
		/// <returns>A new <see cref="FileStream" />.</returns>
		private static FileStream GetTemporaryFile(string localPath)
		{
			Debug.Assert(localPath != null, "localPath is null");

			var directory = Path.GetDirectoryName(localPath);
			var fileName = Path.GetRandomFileName();
			var tmpPath = Path.Combine(directory, fileName);

			while (true)
			{
				try
				{
					return new FileStream(tmpPath, FileMode.CreateNew);
				}
				catch (IOException)
				{
					// The file already exists, so try a different name.
					tmpPath = Path.Combine(directory, Path.GetRandomFileName());
				}
			}
		}

		/// <summary>
		/// Undoes the creation of the specified file.
		/// </summary>
		/// <param name="filename">The filename.</param>
		/// <param name="stream">The stream.</param>
		private static void UndoCreate(string filename, Stream stream)
		{
			try
			{
				// Ensure that the stream is closed.
				stream.Close();

				// Delete the file.
				File.Delete(filename);
			}
			catch (Exception e)
			{
				Console.Error.WriteLine(e.ToString());
			}
		}

		/// <summary>
		/// Undoes the creation of the specified file.
		/// </summary>
		/// <param name="filename">The filename.</param>
		private static void UndoCreate(string filename)
		{
			try
			{
				// Delete the file.
				File.Delete(filename);
			}
			catch (Exception e)
			{
				Console.Error.WriteLine(e.ToString());
			}
		}

		/// <summary>
		/// Restores the specified file from a backup.
		/// </summary>
		/// <param name="filename">The path of the file to restore.</param>
		private static void UndoReplace(string filename)
		{
			var backup = filename + ".bak";
			File.Replace(backup, filename, null);
		}

		#endregion

		#region Private Fields

		private readonly List<Action> _log = new List<Action>();
		private readonly List<Action> _onComplete = new List<Action>();

		#endregion

		#region Private Methods

		/// <summary>
		/// Replaces the contents of a specified file with the contents of another file, deleting the original
		/// file and creating a backup of the replaced file.
		/// </summary>
		/// <param name="sourceFilename">The source filename.</param>
		/// <param name="destinationFilename">The destination filename.</param>
		private void Replace(string sourceFilename, string destinationFilename)
		{
			Debug.Assert(sourceFilename != null, "sourceFilename is null");
			Debug.Assert(destinationFilename != null, "destinationFilename is null");

			if (File.Exists(destinationFilename))
			{
				// Record that, on failure, the destination file should be restored.
				_log.Add(() => UndoReplace(destinationFilename));

				// Backup up the destination file.
				var backup = destinationFilename + ".bak";
				File.Replace(sourceFilename, destinationFilename, backup);
			}
			else
			{
				_log.Add(() => UndoCreate(destinationFilename));
				File.Move(sourceFilename, destinationFilename);
			}
		}

		#endregion

		#region IResultDocumentHandler Members

		/// <summary>
		/// Indicates that all operations within the scope are completed successfully.  Upon successful completion,
		/// all result documents will have been written.
		/// </summary>
		public void Complete()
		{
			foreach (var action in _onComplete)
				action();

			_onComplete.Clear();
			_log.Clear();
		}

		/// <summary>
		/// Discards any result documents in the event that <see cref="Complete" /> has not been called, or a 
		/// call to <see cref="Complete" /> has raised an exception.
		/// </summary>
		public void Dispose()
		{
			// In the even of failure. remove any temporary files and recover any overwritten files.
			foreach (var action in _log)
				action();
		}

		/// <summary>
		/// Returns an <see cref="T:System.Xml.XmlWriter"/> to which a result document can be written.
		/// </summary>
		/// <param name="resultDocumentUri">The base URI of the result document.</param>
		/// <param name="settings">The serialization settings.</param>
		/// <returns>
		/// A new <see cref="T:System.Xml.XmlWriter"/> to write output with the specified serialization settings to a resource with
		/// the specified base URI, or <see langword="null"/> if the specified result document is of no interest.
		/// </returns>
		public XmlWriter Resolve(string resultDocumentUri, XdmWriterSettings settings)
		{
			// This instance only handles 'file' scheme URIs.
			var uri = new Uri(resultDocumentUri);
			if (uri.Scheme != Uri.UriSchemeFile)
			{
				Console.Error.WriteLine("Cannot create result document " + resultDocumentUri +
				                        ".  Result document URIs must use the 'file' URI scheme");
				return null;
			}

			// To allow rollback, we write to a temporary file.
			var localPath = uri.LocalPath;
			var stream = GetTemporaryFile(localPath);
			var name = stream.Name;

			// Record that, on failure, we must delete the newly created file.
			_log.Add(() => UndoCreate(name, stream));

			// Record that, on completion, we need to move the temporary file to the actual file.
			_onComplete.Add(() => Replace(name, localPath));

			// Ensure that the underlying stream will be closed when a result document has been written.
			settings.CloseOutput = true;
			return XdmWriter.Create(stream, settings);
		}

		#endregion
	}
}
