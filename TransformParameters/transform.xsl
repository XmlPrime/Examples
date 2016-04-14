<xsl:stylesheet version="2.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:xs="http://www.w3.org/2001/XMLSchema">

  <xsl:param name="price" select="0" as="xs:decimal" />
             
  <xsl:template match="/bib">    
    <bib filter="books with price greater than {$price}">
      <xsl:for-each select="book[xs:decimal(price) gt $price]">
        <xsl:sort select="xs:decimal(price)"/>
        <xsl:copy-of select="."/>
      </xsl:for-each>
    </bib>
  </xsl:template>

</xsl:stylesheet>