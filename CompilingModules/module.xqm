module namespace m1="urn:module";

declare variable $m1:variable := 1;

declare variable $m1:external as xs:integer external;

declare function m1:function($arg)
{
   $arg + $m1:external
};