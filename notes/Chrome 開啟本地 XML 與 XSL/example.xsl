<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:template match="/">
        <html>
            <head>
                <title>XML 展示</title>
            </head>
            <body>
                <h1>XML 內容展示</h1>
                <xsl:for-each select="root/item">
                    <div>
                        <h2><xsl:value-of select="name"/></h2>
                        <p><xsl:value-of select="value"/></p>
                    </div>
                </xsl:for-each>
            </body>
        </html>
    </xsl:template>
</xsl:stylesheet>