using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using XmlPrime.Serialization;

namespace XmlPrime.Samples.Key
{
    /// <summary>
    /// A example demonstrating the use the XSLT <c>fn:key</c> called from the <see cref="XPath"/> class.
    /// </summary>
    /// <seealso href="http://www.xmlprime.com/xmlprime/doc/current/using-xpath.htm">Using XPath 2.0</seealso>
    internal class Program
    {
        #region Private Static Methods

        /// <summary>
        /// Performs the query.
        /// </summary>
        /// <param name="inputPath">The filename of the context item document.</param>
        /// <param name="outputPath">The filename of the output.</param>
        private static void PerformQuery(string inputPath, string outputPath)
        {
            Debug.Assert(inputPath != null, "inputPath in null");
            Debug.Assert(inputPath != null, "outputPath in null");

            // First, we create a new XmlNameTable instance. This will be used to share information such 
            // as element and attribute names between the XML documents and the query.
            var nameTable = new NameTable();

            // Next we create an XmlReaderSettings instance and set its NameTable property. 
            // In order for XmlPrime to work correctly all documents passed in to XPath must be loaded
            // with the XmlNameTable used to compile the query. 
            var xmlReaderSettings = new XmlReaderSettings { NameTable = nameTable };

            // In order to query the document we load it into an XdmDocument.
            XdmDocument document;
            using (var reader = XmlReader.Create(inputPath, xmlReaderSettings))
            {
                document = new XdmDocument(reader);
            }

            // In order to describe how the query should be compiled we need set up an XPathSettings object. 
            // This describes all the settings used for compilation. 
            // In particular, we will set the name table used by the query to match the one we used earlier 
            // and we will set the context item type. 
            // By default the context item type is set to none, and so the context item cannot be set 
            // unless we override the type here.
            var xpathSettings = new XPathSettings(nameTable) { ContextItemType = XdmType.Node };

            xpathSettings.ImportModule(XdmModule.XsltFunctions);
            xpathSettings.DeclareKey(new XmlQualifiedName("book-by-year"), "book", "@year", null);

            const string pathExpression = "key('book-by-year', 1992)";

            // We can then compile the expression using the Compile method. 
            // This returns us an XPath object encapsulating the query.
            var xpath = XPath.Compile(pathExpression, xpathSettings);

            // NOTE: We could just have used XPath.Compile(pathExpression, nameTable) in this simple case.

            // Now we have our XPath object we now just need to evaluate it. 
            // We use a DynamicContextSettings object which describes the parameters used to evaluate the expression. 
            // In particular we will set the context item to be the document that we loaded earlier. 
            var contextItem = document.CreateNavigator();
            var settings = new DynamicContextSettings { ContextItem = contextItem };

            // We call the XPath.Evaluate method to evaluate the expression.
            // This returns a sequence of items. 
            var result = xpath.Evaluate(settings);

            // NOTE: We could just have used xpath.Evaluate(contextItem) in this simple case.

            // Finally, we will serialize the results to the output file.
            // We use an XdmWriterSettings object which describes the serialization parameters used to write the results.
            // In particular we will set the character reference style to hex, and will specify indentation.
            var serializationSettings = new XdmWriterSettings
            {
                CharacterReferenceStyle = CharacterReferenceStyle.Hexadecimal,
                Indent = true
            };

            // We serialize results of evlauating the XPath expression to the output file with the specified serialization settings.
            XdmWriter.Serialize(result, outputPath, serializationSettings);
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

            if (args.Length != 2)
            {
                var command = Environment.GetCommandLineArgs()[0];
                var name = Path.GetFileNameWithoutExtension(command);

                Console.Error.WriteLine("Usage: {0} <input.xml> <output>", name);
                return -1;
            }

            var inputPath = args[0];
            var outputPath = args[1];

            try
            {
                PerformQuery(inputPath, outputPath);
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
