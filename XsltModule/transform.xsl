<xsl:stylesheet version="2.0"
                exclude-result-prefixes="ext"
                xmlns:ext="http://www.xmlprime.com/xpath"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:xs="http://www.w3.org/2001/XMLSchema"
                xmlns="http://www.w3.org/1999/xhtml">

  <xsl:template match="/bib">
    <xsl:copy>
      <xsl:for-each select="book">
        <xsl:sort select="xs:decimal(price)"/>
        <xsl:value-of select="ext:string-to-utf8(title)"/>
      </xsl:for-each>
    </xsl:copy>
  </xsl:template>

</xsl:stylesheet>