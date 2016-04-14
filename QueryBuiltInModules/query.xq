(: This query uses a function defined in XPath and XQuery Functions and Operators 3.0 :)

for $book in /bib/book[price > 100]
return path($book)