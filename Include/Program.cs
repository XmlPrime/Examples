using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Resolvers;
using XmlPrime.Serialization;

namespace XmlPrime.Samples.Include
{
	/// <summary>
	/// A simple example demonstrating use of the <see cref="XInclude"/> class.
	/// </summary>
	/// <seealso href="http://www.xmlprime.com/xmlprime/doc/current/using-xinclude.htm">Using XML Inclusions (XInclude) 1.0</seealso>
	internal class Program
	{
		#region Private Static Methods

		/// <summary>
		/// Performs the query.
		/// </summary>
		/// <param name="inputPath">The filename of the input.</param>
		/// <param name="outputPath">The filename of the output.</param>
		private static void PerformInclusion(string inputPath, string outputPath)
		{
			Debug.Assert(inputPath != null, "inputPath in null");
			Debug.Assert(inputPath != null, "outputPath in null");

			// First we create an XmlResolver.  This will be used to resolve included resources.
			var xmlResolver = new XmlPreloadedResolver(new XmlUrlResolver());

			// Next we create an XmlReaderSettings instance and set its XmlResolver property. 
			var xmlReaderSettings = new XmlReaderSettings
			                        	{
			                        		XmlResolver = xmlResolver,
                                        NameTable = new NameTable()
			                        	};

			// We create a DocumentSet which ensures that multiple requests for the same document return the same response.
			var documentSet = new DocumentSet(xmlResolver, xmlReaderSettings);

			// We create an XIncludeSettings object, which describes all the settings used for XML inclusion. 
			// In particular, we specify the document set to use for during XML Inclusion processing, and 
			// specify that Base URI Fixup and Language Fixup should be performed.  These settings govern the 
			// introduction of xml:base and xml:lang attributes respectively.
			var includeSettings = new XIncludeSettings
			                      	{
			                      		DocumentSet = documentSet,
			                      		FixupXmlBase = true,
			                      		FixupXmlLang = true
			                      	};

			// We will serialize the results of performing the inclusion to the output file.
			// We use an XdmWriterSettings object which describes the serialization parameters used to write the results.
			// In particular we will set the character reference style to hex, and will specify indentation.
			var serializationSettings = new XdmWriterSettings
			                            	{
			                            		CharacterReferenceStyle = CharacterReferenceStyle.Hexadecimal,
			                            		Indent = true
			                            	};

			// Finally, we perform XML Inclusion processing on the input and serialize it to the output.
			XInclude.Process(inputPath, outputPath, serializationSettings, includeSettings);
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
			Debug.Assert(args != null, "args is null");

			if (args.Length != 2)
			{
				var command = Environment.GetCommandLineArgs()[0];
				var name = Path.GetFileNameWithoutExtension(command);

				Console.Error.WriteLine("Usage: {0} <input.xml> <output.xml>", name);
				return -1;
			}

			var inputPath = args[0];
			var outputPath = args[1];

			try
			{
				PerformInclusion(inputPath, outputPath);
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
