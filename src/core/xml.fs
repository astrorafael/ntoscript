((
$Date: 2008-05-14 22:03:33 +0200 (mié 14 de may de 2008) $
$Revision: 522 $
$Author: astrorafael $
$HeadURL: file:///home/desa/repos/ntoscript/trunk/src/core/xml.fs $

2008 By Rafael Gonzalez Fuentetaja
This file is placed in the public domain. NO WARRANTY.
))

\ ###########################
\ *! xml
\ *R @node{XML Utility words}
\ *T XML Utility words
\ ###########################

\ ***************
\ *S Introduction
\ ***************

\ *P A set of XML manipulation words. There is also a very complete auxiliary
\ ** wordset for XML processing
\ ** in the standard  *\i{VFX Forth for Linux} toolbox *\i{Lib/XML.fth}.
\ ** But I had this one from my previous *\i{indistcript v1.0} and have not
\ ** needed to change so far.

\ *P Two kind of words are defined:
\ *(
\ *B Message extraction words. They help in isolaiting a complete root element
\ ** within the continuois stream of messages.
\ *B Message parsing words. They help in extracting attributes values, PCDATA
\ ** and so on.
\ *)

\ ***************
\ +S Design notes
\ ***************


\ +P Message extraction words work together with the custom XML Input buffer
\ +* structure *\fo{XMLBuf} and its read/write pointers.
\ +* For that reason, we no longer adopt the stack signature suggested by
\ +* Jenny Brien "remaining string, parsing string"
\ +* (for this kind of words only)

\ +P Words *\fo{SIMPLE-ELEMENT?} and *\fo{COMPOSITE-ELEMENT?} share the
\ +* same stack signature and extracts a complete XML INDI message
\ +* although start and final delimiters are not strictly needed to decode
\ +* its contents. The reason is for debugging purposes, for the time being.


\ +P I have not yet been able to get rid of *\fo{parse-element} and use
\ +* *\fo{open-tag} and *\fo{full-element?} instead in subelements parsing
\ +* because:
\ +(
\ +B There are several sublements in a row within an element and ...
\ +B the remaining string produced by open-tag and full-element? do not
\ +* include the following sublements, only characters till the end of the
\ +* subelement being parsed.
\ +)

\ ***********
\ *S Glossary
\ ***********

\ =========================
\ *N XML Message Extraction
\ =========================

\ *P The words below help to isolate and extract a full XML INDI message
\ ** from the stream of characters stored in the XML buffer.


: $1-				  \  ca1 u1 -- ca2-1 u2-1
\ *G Trim right string by one position.
\ ** *\i{ca1} is the *\b{string end address}, *\i{u1} its length
   1- swap 1 chars - swap ;			


: trim-tag				  \ ca1 u1 - ca2 u2 ca3 u3
\ *G Given a string *\i{ca1 u1} where a XML tag starts with *\fo{CHAR <},
\ ** trim it both left and right, yielding *\i{ca3 u3}.
\ ** *\i{ca2 u2} is the remaining string.  
   1 /string 2dup BL scan dup >r 2swap r> - 
;


: open-tag			    \ ca1 u1 -- ca2 u2 ca3 u3
\ *G Given a string *\i{ca1 u1}, finds an opening XML tag string *\i{ca3 u3}.
\ ** without *\fo{CHAR <}. *\b{Note:} *\i{u3} is 0 if no tag is found.
\ *\i{ca2 u2} is the string remaining.
   2dup [char] < scan dup if 2swap 2drop trim-tag exit then 
;


: />				  \ ca1 u1 --  ca2 u2 flag
\ *G Finds the *\f{/>} end delimiter for simple XML elements
\ ** in the string specified by  *\i{ca1 u1}.
\ ** Returns true if found and also the string  *\i{ca2 u2}
\ ** where *\fo{CHAR >} starts.
\ +* Probably faster than using *\fo{s" />" SEARCH}.   
   [char] > scan over 1- c@ [char] / = over 0<> and
;

   
: simple-element?		  \ ca1 u1 ca2 u2 -- ca3 u3 True | False
\ *G Search for a simple XML element *\f{<XXX />} in string  *\i{ca1 u1}
\ ** If found, flag is True and the element is given by given by *\i{ca2 u2}
\ ** String returned includes the *\f{/>} literal. Preconditions:
\ *(   
\ *B *\i{ca1 u1} is the string remaining after applying *\fo{open-tag}.
\ *B *\i{ca2 u2} is the tag string after applying *\fo{open-tag}.    
\ *)   
   >r >r				\ -- ca1 u1 ; R: -- u2 ca2
   /> not if 2r> 4drop False exit then	\ -- ca4 u4
   drop R> 1-				\ ca3 = ca2-1
   tuck - 1+ r> drop			\ u3 =  ca4-ca3+1
   True
;


: composite-element?		 \ ca1 u1 ca2 u2  -- ca3 u3 True | False
\ *G Search for an composite XML element <XXX></XXX> in string *\i{ca1 u1}.
\ ** Tag name is given by *\i{ca2 u2}.
\ ** If found, the composite element string is given in *\i{ca3 u3}
\ ** with both *\f{<XXX>} and *\f{</XXX>} included. Preconditions:
\ *(   
\ *B *\i{ca1 u1} is the string remaining after applying *\fo{open-tag}.
\ *B *\i{ca2 u2} is the tag string after applying *\fo{open-tag}.    
\ *)
   2dup >R >R			\ -- ca1 u1 ca2 u2 ; R: -- u2 ca2
   search not if 2r> 4drop False exit then \ -- ca4 u4
   drop r> 1-				\ ca3 = ca2-1
   tuck - r> 1+ +			\ u3 = ca4-ca3+u2+1
   True
;


: full-element?		       \ ca1 u1 ca2 u2  -- ca3 u3 True | False
\ *G Search for a full XML element, either being simple or composite.
\ ** Tag name is given by *\i{ca2 u2}.   
\ ** Uses *\fo{simple-element?} and *\fo{composite-element?} above.
   4dup simple-element? 0= if composite-element? exit then
   2>R 4drop 2R> True
;


\ ========================
\ *N Parse inside contents
\ ========================

\ *P The following words help parsing XML INDI messages.


: white?	      \ c -- flag   
\ *G True if *\i{c} is a whitespace-class character.
\ ** This means ASCII codes less or equal than 32.
\ ** So we conveniently skip spaces, TABs, LFs and CRs.
   BL > 0=
;


: -leader			 \ ca1 u1 -- ca2 u2
\ *G trims string to skip leading spaces, TABs, LFs and CRs,
\ ** adjusting length as necessary.
\ *+ VFX's *\fo{-LEADING} is similar but does not skip TABs, LFs or CRs. 
   begin  over c@ white? over 0<> and while 1 /string repeat
;


: -trailer		       \ ca1 u1 -- ca2 u2 
\ *G trims string to skip trailing spaces, TABs, LFs and CRs,
\ ** adjusting length as necessary.
\ *+ VFX's *\fo{-TRAILING} is similar but does not skip TABs, LFs or CRs. 
   2dup chars + 1 chars - swap		\ produce ending addr and length
   begin over c@ white? over 0<> and while $1- repeat nip
;


: trim			    \ ca1 u1 -- ca2 u2
\ *G Trim string left and right.
   -leader -trailer
;


: string/			 \ ca1 u1 u -- ca2 u2 
\ *G Gets the string matched *\i{ca2 u2} from the string
\ ** remaining *\i{ca1,u1} and the original string length *\i{u}.
\ ** Original from Jenny Brien.   
   swap - tuck - swap
;


: attr-list			      \ ca u -- ca1 u1 ca2 u2 
\ *G Extract the attributes list string (> included) for an composite
\ ** element definition. Useful to parse the individual element attributes.
\ ** Matched string is *\i{ca2 u2}.
\ ** Remaining string is *\i{ca1 u1}.
   dup >r s" >" search drop 1 /string 2dup r> string/
;


: (attr-value)			       \ ca1 u1 -- ca2 u2
\ *G Do the actual value substring extraction from a name='value' XML
\ ** attribute string given by *\i{ca1 u1}. Assumes name already found.   
   [char] = scan 1 /string		\ finds = and skips it
   over c@ >r 1 /string			\ saves ' or " delim and trims string
   2dup r> scan				\ parse string until new delim
   drop nip over - 			\ computes contained value string
; 


: attr-value ( ca1 u1 ca2 u2 --  ca3 u3 flag )
\ *G Search the value substring from a name='value' XML attribute.
\ ** *\i{ca1 u1} is the element string.
\ ** *\i{ca2 u2} is the atribute name string to look for.
\ ** *\i{ca3 u3} is the value substring extracted.
\ ** *\i{flag} is true if attribute 'name=' was found.
   search dup >r if (attr-value) then r>
; 


: #pcdata		     \ ca1 u1 -- ca2 u2 
\ *G extracts the parsed character data ca1 u1 inside an full XML element
\ ** denoted by *\i{ca1 u1}.
\ ** PCdata is trimmed for leading and trailing spaces, tabs, crs & lfs ..
   [char] > scan 1 /string 2dup [char] < scan \ find > and next <
   drop nip over -			\ compute length from pointers
   trim
;


: parse-element ( ca1 u1 ca2 u2  -- ca3 u3 ca4 u4 )
\ *G Search for an full element <XXX></XXX> in string *\i{ca1 u1}.
\ ** The closing tag (angles included) is given in *\i{ca2 u2}.
\ ** If found, the whole element is given in *\i{ca4 u4} up to the closing tag
\ ** included.
\ ** The remaining string is always given in *\i{ca3 u3}.
\ ** If not found u4 is zero.
\ ** This word is useful to search sub elements for a given element.   
   2swap -leader dup >r 2swap dup >r search
   if r> /string else r> drop then 2dup r> string/
;


\ ======
\ *> ###
\ ======

