using System;
using System.Diagnostics;
using System.IO;

namespace XmlPrime.Samples.Query
{
	/// <summary>
	/// A simple example demonstrating use of the <see cref="XQuery"/> class.
	/// </summary>
    /// <seealso href="http://www.xmlprime.com/xmlprime/doc/current/using-xquery.htm">Using XQuery 1.0</seealso>
	internal class Program
	{
		#region Private Static Methods

		/// <summary>
		/// Performs the query.
		/// </summary>
		/// <param name="inputPath">The filename of the context item document.</param>
		/// <param name="queryPath">The filename of the query.</param>
		/// <param name="outputPath">The filename of the output.</param>
		private static void PerformQuery(string inputPath, string queryPath, string outputPath)
		{
			Debug.Assert(inputPath != null, "inputPath in null");
			Debug.Assert(queryPath != null, "queryPath in null");
			Debug.Assert(inputPath != null, "outputPath in null");

			// In order to describe how the query should be compiled we need set up an XQuerySettings object. 
			// This describes all the settings used for compilation. 
			// We set the context item type which, by default, is set to none, and so the context item cannot be set 
			// unless we override the type here.
			// We'll also prvent use of the XQuery Update Facility, because this sample doesn't handle updating queries.
			var querySettings = new XQuerySettings
			                    	{
			                    		ContextItemType = XdmType.Node,
			                    		EnabledFeatures = XQueryOptionalFeatures.AllFeatures ^ XQueryOptionalFeatures.UpdateFeature
			                    	};

			// We can then compile the query using the Compile method. 
			// This returns us an XQuery object encapsulating the query.
			XQuery query;
			using (var fileStream = File.OpenRead(queryPath))
			{
				query = XQuery.Compile(fileStream, querySettings);
			}

			// Now we have our query object we now just need to evaluate it. 
            // We load the document to be queried into an XdmDocument.

            var document = new XdmDocument(inputPath);

            // Then use a DynamicContextSettings object which describes the parameters used to evaluate the query. 
            // In particular we will set the context item to be the document that we loaded earlier. 
			var contextItem = document.CreateNavigator();
			var settings = new DynamicContextSettings {ContextItem = contextItem};

			// It can often be more efficient to serialize the results directly to the output file.
			// This is possible using the Serialize method.
			query.Serialize(outputPath, settings);
			
			// NOTE: We could just have used query.Serialize(outputPath, contextItem) in this simple case.
		}

		#endregion

		#region Public Static Methods

		/// <summary>
		/// Performs a query on a specified file and serializes the result to a specified file.
		/// </summary>
		/// <param name="args">The command line arguments.</param>
		/// <returns>0 if the results of the query was serialized successfully; otherwise -1.</returns>
		public static int Main(string[] args)
		{
			Debug.Assert(args != null, "args in null");

			if (args.Length != 3)
			{
				var command = Environment.GetCommandLineArgs()[0];
				var name = Path.GetFileNameWithoutExtension(command);

				Console.Error.WriteLine("Usage: {0} <input.xml> <query.xq> <output>", name);
				return -1;
			}

			var inputPath = args[0];
			var queryPath = args[1];
			var outputPath = args[2];

			try
			{
				PerformQuery(inputPath, queryPath, outputPath);
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
