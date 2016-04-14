using System;
using System.Diagnostics;
using System.IO;
using System.Xml;

namespace XmlPrime.Samples.Transform
{
    /// <summary>
    /// A simple example demonstrating use of the <see cref="Xslt"/> class.
    /// </summary>
    /// <seealso href="http://www.xmlprime.com/xmlprime/doc/current/using-xslt.htm">Using XSLT 2.0</seealso>
    internal class Program
    {
        #region Private Static Methods

        /// <summary>
        /// Performs the transformation.
        /// </summary>
        /// <param name="inputPath">The filename of the context item document.</param>
        /// <param name="xsltPath">The filename of the XSLT transformation.</param>
        /// <param name="outputPath">The filename of the primary output.</param>
        private static void PerformTransformation(string inputPath, string xsltPath, string outputPath)
        {
            Debug.Assert(inputPath != null, "inputPath in null");
            Debug.Assert(xsltPath != null, "xsltPath in null");
            Debug.Assert(inputPath != null, "outputPath in null");

            // First, we create a new XmlNameTable instance. This will be used to share information such 
            // as element and attribute names between the XML documents and the transformation.
            var nameTable = new NameTable();

            // Next we create an XmlReaderSettings instance and set its NameTable property. 
            // In order for XmlPrime to work correctly all documents passed in to Xslt must be loaded
            // with the XmlNameTable used to compile the query. 
            var xmlReaderSettings = new XmlReaderSettings { NameTable = nameTable };

            // In order to transform  the document we load it into an XdmDocument.
            XdmDocument document;
            using (var reader = XmlReader.Create(inputPath, xmlReaderSettings))
            {
                document = new XdmDocument(reader);
            }

            // In order to describe how the transformation should be compiled we need set up an XsltSettings 
            // object.  This describes all the settings used for compilation.
            // In particular, we will set the name table used by the transformation to match the one we used 
            // earlier and we will set the context item type. 
            // By default the context item type is set to none, and so the context item cannot be set unless 
            // we override the type here. 
            var xsltSettings = new XsltSettings(nameTable) { ContextItemType = XdmType.Node };

            // We can then compile the transformation using the Compile method. 
            // This returns us an Xslt object encapsulating the transformation. 
            var xslt = Xslt.Compile(xsltPath, xsltSettings);

            // Now we have our transformation object we now just need to execute it. 
            // We use a DynamicContextSettings object which describes the parameters used to evaluate the query. 
            // In particular we will set the context item to be the document that we loaded earlier. 
            var contextItem = document.CreateNavigator();
            var settings = new DynamicContextSettings { ContextItem = contextItem };

            // We use the Parameters member of the DynamicContextSettings object to pass the values of parameters
            // declared in the transformation.
            settings.Parameters.Add(new XmlQualifiedName("price"), 60);

            // We will use the ApplyTemplates method to initiate a transformation by applying templates 
            // in the default mode and serializing the primary result document to a stream.
            using (var outputStream = File.Create(outputPath))
            {
                xslt.ApplyTemplates(settings, outputStream);
            }
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Performs a transformation on a specified file and serializes the result to a specified file.
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        /// <returns>0 if the transformation was executed successfully; otherwise -1.</returns>
        public static int Main(string[] args)
        {
            Debug.Assert(args != null, "args in null");

            if (args.Length != 3)
            {
                var command = Environment.GetCommandLineArgs()[0];
                var name = Path.GetFileNameWithoutExtension(command);

                Console.Error.WriteLine("Usage: {0} <input.xml> <transform.xsl> <output>", name);
                return -1;
            }

            var inputPath = args[0];
            var xsltPath = args[1];
            var outputPath = args[2];

            try
            {
                PerformTransformation(inputPath, xsltPath, outputPath);
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
