<xsl:stylesheet version="2.0" 
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:xp="http://www.xmlprime.com/"
                xmlns:ext="urn:extensions">
  
    <xp:script language="C#" implements-prefix="ext">
      <![CDATA[

        private static byte[] StringToBinary(string arg, string encoding)
        {
            var enc = System.Text.Encoding.GetEncoding(encoding);
            if (enc == null)
                throw new ArgumentNullException("encoding");

            return enc.GetBytes(arg);
        }
        
        [XdmFunction("string-to-utf8")]
        [XdmType(XmlTypeCode.Integer, Quantifier.ZeroOrMore)]
        public static IEnumerator<decimal> StringToUtf8(string arg)
        {
            foreach (var b in StringToBinary(arg, "utf-8"))
                yield return b;
        }
  ]]>
    </xp:script>
  <xsl:template match="/bib">
    <xsl:copy>
      <xsl:for-each select="book">
        <encoded-title>
          <xsl:value-of select="ext:string-to-utf8(title)"/>
        </encoded-title>
      </xsl:for-each>
    </xsl:copy>
  </xsl:template>

</xsl:stylesheet>