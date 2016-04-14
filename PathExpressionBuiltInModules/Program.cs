using System;
using System.Diagnostics;
using System.IO;
using XmlPrime.Serialization;

namespace XmlPrime.Samples.PathExpressionBuiltInModules
{
    /// <summary>
    /// A simple example demonstrating use of built-in modules in XPath.
    /// </summary>
    /// <seealso href="http://www.xmlprime.com/xmlprime/doc/current/built-in-modules.htm">Built-In Modules</seealso>
    internal class Program
    {
        #region Private Static Methods

        /// <summary>
        /// Performs the query.
        /// </summary>
        /// <param name="inputPath">The filename of the context item document.</param>
        /// <param name="pathExpression">An XPath 2.0 expression.</param>
        /// <param name="outputPath">The filename of the output.</param>
        private static void PerformQuery(string inputPath, string pathExpression, string outputPath)
        {
            Debug.Assert(inputPath != null, "inputPath in null");
            Debug.Assert(pathExpression != null, "pathExpression in null");
            Debug.Assert(inputPath != null, "outputPath in null");

            // In order to describe how the query should be compiled we need set up an XPathSettings object. 
            // This describes all the settings used for compilation. 
            // In particular, we will set the context item type. 
            // By default the context item type is set to none, and so the context item cannot be set 
            // unless we override the type here.
            var xpathSettings = new XPathSettings { ContextItemType = XdmType.Node };

            // We want to make XSLT 2.0 extension functions available.
            xpathSettings.ImportModule(XdmModule.XsltFunctions);

            // We can then compile the expression using the Compile method. 
            // This returns us an XPath object encapsulating the query.
            var xpath = XPath.Compile(pathExpression, xpathSettings);

            // Now we have our XPath object we now just need to evaluate it. 

            // We load the document to be queried into an XdmDocument.
            var contextDocument = new XdmDocument(inputPath);

            // We call the XPath.Evaluate method to evaluate the expression.
            // This returns a sequence of items. 
            var result = xpath.Evaluate(contextDocument.CreateNavigator());

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

            if (args.Length != 3)
            {
                var command = Environment.GetCommandLineArgs()[0];
                var name = Path.GetFileNameWithoutExtension(command);

                Console.Error.WriteLine("Usage: {0} <input.xml> <path expression> <output>", name);
                return -1;
            }

            var inputPath = args[0];
            var pathExpression = args[1];
            var outputPath = args[2];

            try
            {
                PerformQuery(inputPath, pathExpression, outputPath);
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
