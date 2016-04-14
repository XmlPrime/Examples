using System;
using System.Diagnostics;
using System.IO;
using System.Xml;

namespace XmlPrime.Samples.Update
{
	/// <summary>
	/// A simple example demonstrating use of the <see cref="XQuery"/> class.
	/// </summary>
	/// <seealso href="http://www.xmlprime.com/xmlprime/doc/current/using-xquery.htm">Using XQuery 1.0</seealso>
	internal class Program
	{
		#region Private Static Methods

		/// <summary>
		/// Performs the update.
		/// </summary>
		/// <param name="inputPath">The filename of the context item document.</param>
		/// <param name="queryPath">The filename of the updating query.</param>
		private static void PerformUpdate(string inputPath, string queryPath)
		{
			Debug.Assert(inputPath != null, "inputPath in null");
			Debug.Assert(queryPath != null, "queryPath in null");

			// We create an XQuerySettings instance, which describes all the settings used for compilation. 
			// In particular, we will set the context item type. 
			// By default the context item type is set to none, and so the context item cannot be set 
			// unless we override the type here.
			var querySettings = new XQuerySettings { ContextItemType = XdmType.Node };

			// We can then compile the query using the Compile method. 
			// This returns us an XQuery object encapsulating the query.
			XQuery query;
			using (var fileStream = File.OpenRead(queryPath))
			{
				query = XQuery.Compile(fileStream, querySettings);
			}

			// This example only deals with updating queries.
			if (query.IsUpdate == false)
				throw new NotSupportedException("Sorry, updates only please!");

			// Now we have our query object we now just need to evaluate it

            // We load the input into an XdmDocument.
            var document = new XdmDocument(inputPath, XmlSpace.Preserve);

            // We use a DynamicContextSettings object which describes the parameters used to evaluate the query. 
            // In particular we will set the context item to be the document that we loaded earlier. 
			var contextItem = document.CreateNavigator();
			var settings = new DynamicContextSettings { ContextItem = contextItem };

			using (var resultDocumentHandler = new SimpleResultDocumentHandler())
			{
				// We call the EvaluateUpdate method to perform the update.
				query.EvaluateUpdate(settings, resultDocumentHandler);

				// Write updated/created documents.
				resultDocumentHandler.Complete();
			}
		}

		#endregion

		#region Public Static Methods

		/// <summary>
		/// Performs an update on a specified file.
		/// </summary>
		/// <param name="args">The command line arguments.</param>
		/// <returns>0 if the update was successful; otherwise -1.</returns>
		public static int Main(string[] args)
		{
			Debug.Assert(args != null, "args in null");

			if (args.Length != 2)
			{
				var command = Environment.GetCommandLineArgs()[0];
				var name = Path.GetFileNameWithoutExtension(command);

				Console.Error.WriteLine("Usage: {0} <input.xml> <update.xq>", name);
				return -1;
			}

			var inputPath = args[0];
			var queryPath = args[1];

			try
			{
				PerformUpdate(inputPath, queryPath);
			}
			catch (Exception e)
			{
				Console.Error.WriteLine(e.ToString());
				return -1;
			}

			return 0;
		}

		#endregion
	}
}
