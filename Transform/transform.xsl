<xsl:stylesheet version="2.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:xs="http://www.w3.org/2001/XMLSchema">

  <xsl:template match="/bib">
    <xsl:copy>
      <xsl:for-each select="book">
        <xsl:sort select="xs:decimal(price)"/>
        <xsl:copy-of select="."/>
      </xsl:for-each>
    </xsl:copy>
  </xsl:template>

</xsl:stylesheet>