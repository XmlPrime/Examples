<xsl:stylesheet version="2.0"
                exclude-result-prefixes="xs xp"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:xp="http://www.xmlprime.com/xpath-functions"
                xmlns:xs="http://www.w3.org/2001/XMLSchema">

  <!-- Constrived example using an XmlPrime extension function -->
  <xsl:template match="/bib">
    <xsl:copy>
      <xsl:for-each select="book/bib">
        <xsl:sort select="xs:decimal(.)"/>
        <xsl:value-of select="xp:sinh(.)"/>
      </xsl:for-each>
    </xsl:copy>
  </xsl:template>

</xsl:stylesheet>