for $book in /bib/book
order by xs:decimal($book/price)
return $book