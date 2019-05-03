((
$Date: 2008-05-14 22:03:33 +0200 (mié 14 de may de 2008) $
$Revision: 522 $
$Author: astrorafael $
$HeadURL: file:///home/desa/repos/ntoscript/trunk/src/core/base64.fs $

2008 By Rafael Gonzalez Fuentetaja
This file is placed in the public domain. NO WARRANTY.
))

\ ##################################
\ *! base64
\ *R @node{Base64 Encoding/Decoding}
\ *T Base64 Encoding/Decoding
\ ##################################


\ *P This module defines all necesary words for Base64 encoding and
\ ** encoding. It is based on a former work by Wil Baden and has been
\ ** modified to suit *\f{NTOScript} needs:
\ *(
\ *B file   => Encoding => standard output
\ *B Memory => Decoding => file.
\ *)


\ ***********************************
\ *S Base64 Content-Transfer-Encoding
\ ***********************************

\ *R See @uref{http://www.oac.uci.edu/indiv/ehood/MIME/1521/rfc1521ToC.html} @*

\ *P  The Base64 Content-Transfer-Encoding is designed to represent   
\ **  arbitrary sequences of octets in a form that need not be        
\ **  humanly readable.  The encoding and decoding algorithms are     
\ **  simple, but the encoded data are consistently only about 33     
\ **  percent larger than the unencoded data.  This encoding is       
\ **  virtually identical to the one used in Privacy Enhanced Mail    
\ **  (PEM) applications, as defined in RFC 1421. The base64          
\ **  encoding is adapted from RFC 1421, with one change: base64      
\ **  eliminates the "*" mechanism for embedded clear text.           
\ **                                                                  
\ *P  A 65-character subset of US-ASCII is used, enabling 6 bits to   
\ **  be represented per printable character. (The extra 65th         
\ **  character, "=", is used to signify a special processing         
\ **  function.)                                                      
\ **                                                                  
\ *P  *\b{NOTE:} This subset has the important property that it is    
\ **  represented identically in all versions of ISO 646, including   
\ **  US ASCII, and all characters in the subset are also             
\ **  represented identically in all versions of EBCDIC.  Other       
\ **  popular encodings, such as the encoding used by the uuencode    
\ **  utility and the base85 encoding specified as part of Level 2    
\ **  PostScript, do not share these properties, and thus do not      
\ **  fulfill the portability requirements a binary transport         
\ **  encoding for mail must meet.                                    
\ **                                                                  
\ *P  The encoding process represents 24-bit groups of input bits     
\ **  as output strings of 4 encoded characters. Proceeding from      
\ **  left to right, a 24-bit input group is formed by                
\ **  concatenating 3 8-bit input groups. These 24 bits are then      
\ **  treated as 4 concatenated 6-bit groups, each of which is        
\ **  translated into a single digit in the base64 alphabet. When     
\ **  encoding a bit stream via the base64 encoding, the bit stream   
\ **  must be presumed to be ordered with the most-significant-bit    
\ **  first.                                                          
\ **                                                                  
\ *P  That is, the first bit in the stream will be the high-order     
\ **  bit in the first byte, and the eighth bit will be the           
\ **  low-order bit in the first byte, and so on.                     
\ **                                                                  
\ *P  Each 6-bit group is used as an index into an array of 64        
\ **  printable characters. The character referenced by the index     
\ **  is placed in the output string. These characters, identified    
\ **  in Table 1, below, are selected so as to be universally         
\ **  representable, and the set excludes characters with             
\ **  particular significance to SMTP (e.g., ".", CR, LF) and to      
\ **  the encapsulation boundaries defined in this document (e.g.,    
\ **  "-").                                                           
\ **                                                                  
\ *E
\ **        Table 1: The Base64 Alphabet                             
\ **                                                                 
\ **  Value Encoding  Value Encoding  Value Encoding  Value Encoding 
\ **      0 A            17 R            34 i            51 z        
\ **      1 B            18 S            35 j            52 0        
\ **      2 C            19 T            36 k            53 1        
\ **      3 D            20 U            37 l            54 2        
\ **      4 E            21 V            38 m            55 3        
\ **      5 F            22 W            39 n            56 4        
\ **      6 G            23 X            40 o            57 5        
\ **      7 H            24 Y            41 p            58 6        
\ **      8 I            25 Z            42 q            59 7        
\ **      9 J            26 a            43 r            60 8        
\ **     10 K            27 b            44 s            61 9        
\ **     11 L            28 c            45 t            62 +        
\ **     12 M            29 d            46 u            63 /        
\ **     13 N            30 e            47 v                        
\ **     14 O            31 f            48 w         (pad) =        
\ **     15 P            32 g            49 x                        
\ **     16 Q            33 h            50 y                        
\ **
\ **                                                                 
\ *P  The output stream (encoded bytes) must be represented in        
\ **  lines of no more than 76 characters each.  All line breaks or   
\ **  other characters not found in Table 1 must be ignored by        
\ **  decoding software.  In base64 data, characters other than       
\ **  those in Table 1, line breaks, and other white space probably   
\ **  indicate a transmission error, about which a warning message    
\ **  or even a message rejection might be appropriate under some     
\ **  circumstances.                                                  
\ **                                                                  
\ *P  Special processing is performed if fewer than 24 bits are       
\ **  available at the end of the data being encoded.  A full         
\ **  encoding quantum is always completed at the end of a body.      
\ **  When fewer than 24 input bits are available in an input         
\ **  group, zero bits are added (on the right) to form an integral   
\ **  number of 6-bit groups.  Padding at the end of the data is      
\ **  performed using the '=' character.  Since all base64 input is   
\ **  an integral number of octets, only the following cases can      
\ **  arise: (1) the final quantum of encoding input is an integral   
\ **  multiple of 24 bits; here, the final unit of encoded output     
\ **  will be an integral multiple of 4 characters with no "="        
\ **  padding, (2) the final quantum of encoding input is exactly 8   
\ **  bits; here, the final unit of encoded output will be two        
\ **  characters followed by two "=" padding characters, or (3) the   
\ **  final quantum of encoding input is exactly 16 bits; here, the   
\ **  final unit of encoded output will be three characters           
\ **  followed by one "=" padding character.                          
\ **                                                                  
\ *P  Because it is used only for padding at the end of the data,     
\ **  the occurrence of any '=' characters may be taken as evidence   
\ **  that the end of the data has been reached (without truncation   
\ **  in transit).  No such assurance is possible, however, when      
\ **  the number of octets transmitted was a multiple of three.       
\ **                                                                  
\ *P  Any characters outside of the base64 alphabet are to be         
\ **  ignored in base64-encoded data.  The same applies to any        
\ **  illegal sequence of characters in the base64 encoding, such     
\ **  as "====="                                                      
\ **                                                                  
\ *P  Care must be taken to use the proper octets for line breaks     
\ **  if base64 encoding is applied directly to text material that    
\ **  has not been converted to canonical form. In particular, text   
\ **  line breaks must be converted into CRLF sequences prior to      
\ **  base64 encoding. The important thing to note is that this may   
\ **  be done directly by the encoder rather than in a prior          
\ **  canonicalization step in some implementations.                  
\ **                                                                  
\ *P  *\b{NOTE:} There is no need to worry about quoting apparent    
\ **  encapsulation boundaries within base64-encoded parts of         
\ **  multipart entities because no hyphen characters are used in     
\ **  the base64 encoding.                                            
\ **                                                                  
\ --------------------------------------------------------------------

\ ***********
\ *S Glossary
\ ***********

MODULE Base64
\ *G

\ ==================
\ *N Base64 Encoding
\ ==================

72 3 4 */
Constant #Bytes/Line
\ +G Bytes per line in the *\b{binary input} to encode.
\ +* The official INDI library seems to prefer 72 bytes/line
\ +* in base64 *\b{encoded} streams.


Create Base64-Alphabet  
\ +G Encoding alphabet.
65 allot

: /Base64-Alphabet			  \ --
\ +G Initializes Encoding alphabet array.   
   S" ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/="
   Base64-Alphabet swap move
; 

/Base64-Alphabet
' /Base64-Alphabet AtCold


Create Clipboard-Buffer  
\ +G Where to place bytes read from input file before encoding.
#Bytes/Line 2 + allot


: Bin-to-Ascii      ( n1 -- n2 )
\ +G   
   63 and  Base64-Alphabet +  c@
;


: zero-padding   ( ca u -- )
\ +G Pad 2 bytes with zero in the input buffer given by *\i{ca}.
\ +* whose lenght is *\i{u}.
   + >r 0 r@ c! 0 r> 1+ c!
;


: append-char   ( c c-addr -- )
\ +G Append char *\i{c} to the counted string denoted by *\i{c-addr}.
   dup >r count + c! 1 r> c+!
;


: =padding   ( -- )
\ +G Append the = character to the scratch pad buffer.    
   [char] = pad append-char
;


: 3Bytes-to-24bits   ( ca1 u1 -- ca2 u2 x1 )
\ +G Encode 3 bytes starting at *\i{ca1 u1}
\ +* leaving the remainder string *\i{ca2 u2}
\ +* and the encoded result *\i{x1}.
   >r
   COUNT 16 LSHIFT >R        ( str+1)( R: x)
   COUNT  8 LSHIFT R> OR >R  ( str+2)( R: xx)
   COUNT R> OR               ( str+3 xxx)( R: )
   r> 3 - swap
;


: 24bits-to-4bytes   ( x1 --  )
\ +G Takes the *\i{x1} integer and splits into 4 base64 encoded bytes
\ +* and appends them to the scratch pad buffer.
\ +* Using the Forth *\fo{PAD} as a temporary buffer for output gives better
\ +* performance that calling *\fo{EMIT} on the individual bytes.   
   dup 18 RSHIFT Bin-to-Ascii pad append-char 
   dup 12 RSHIFT Bin-to-Ascii pad append-char
   dup  6 RSHIFT Bin-to-Ascii pad append-char
   Bin-to-Ascii pad append-char
;


: 3Bin-to-4Ascii    ( ca1 u1 -- ca2 u2 )
\ +G Encode 3 bytes to base64 with no padding, starting at *\i{ca1 u1}
\ +* leaving the remainder string *\i{ca2 u2}   
   3Bytes-to-24bits 24bits-to-4bytes
;


: (base64-encode-line)   ( ca u -- )
\ +G Encode a line *\i{ca u} in the *\fo{PAD} using base64
\ +* and adjust for final padding.
   begin 3Bin-to-4Ascii dup 1 < until 
   dup -1 = if pad c+! =padding drop exit then	
   dup -2 = if pad c+! =padding =padding drop exit then
   2drop
;


: base64-encode-line   ( ca u --  )
\ *G Send *\i{ca u} string encoded as base64 plus a linefeed to standard output
\ ** using *\fo{TYPE} plus and ending *\fo{CR}.   
   0 pad c! 2dup zero-padding (base64-encode-line)
   pad count type cr
;


: base64-encode ( ca u -- )
\ *G Encode a file whose path is given by *\i{ca u}.
\ ** The result is printed to standard output line by line using
\ ** *\fo{base64-encode-line}.   
   r/o bin open-file OPEN-FILE-EXCP ?throw 	\ ifid
   begin
      >r Clipboard-Buffer #Bytes/Line r@
      read-file READ-FILE-EXCP ?throw	\ u2 R: ifid
      dup #Bytes/Line < >r		\ u2 R: flag fid
      Clipboard-Buffer swap		\ ca2 u2 R: flag fid
      base64-encode-line r> r> swap \ fid flag
   until
   close-file CLOSE-FILE-EXCP ?throw
;

EXPORT base64-encode-line
EXPORT base64-encode

\ ==================
\ *N Base64 Decoding
\ ==================


CREATE Inverse-Base64-Alphabet  
\ +G Inverse alphabet lookup table.
256 ALLOT

: SALT   ( -- )
\ +G Initialize inverse alphabet lookup table.
   Inverse-Base64-Alphabet 256 65 FILL
   65 0 DO
      I Base64-Alphabet I + C@ Inverse-Base64-Alphabet + C!
   LOOP
;

SALT					\ initializes at load time
' SALT AtCold				\ and also for turnkey systems


: Ascii-to-Bin      ( n1 -- n2 )
\ +G Convert ASCII character *\i{n1} in binary *\i{n2}.
   Inverse-Base64-Alphabet + C@
;


: 4Ascii-to-24bits   ( ca1 u1 -- ca2 u2 n1 )
\ +G Decodes 4 ASCII chars in base64 from *\i{ca1 u1} to form a 24 bit
\ +* binary integer *\i{n1}.
\ +* Update input string to *\i{ca2 u2}   
   >r
   COUNT Ascii-to-Bin 18 LSHIFT >R        ( str+1)( R: b)
   COUNT Ascii-to-Bin 12 LSHIFT R> OR >R  ( str+2)( R: bb)
   COUNT Ascii-to-Bin  6 LSHIFT R> OR >R  ( str+3)( R: bbb)
   COUNT Ascii-to-Bin           R> OR     ( str+4 bbb)( R: )
   r> 4 - swap
;


: 24bits-to-3bytes ( n1 --  )
\ +G Output the 24 bit integer as 3 bytes into the *\fo{PAD}.
   dup 16 RSHIFT pad append-char
   dup  8 RSHIFT pad append-char
   pad append-char
;

: 4Ascii-to-3Bin    ( ca1 u1 -- ca2 u2 )
\ +G   
   4Ascii-to-24bits 24bits-to-3bytes
;

: (base64-decode-line) ( ca1 u1 -- ca2 u2 )
\ +G Decode a base64 string *\i{ca1 u1} until a newline (*\fo{ALF}) is found
\ +* or the string is exhausted (u2=0).
\ +* Leave the remaining string at *\i{ca2 u2}   
   0 pad c! begin over c@ ALF <> over 0<> and while 4Ascii-to-3Bin repeat
;


: =trim   ( -- )
\ +G trims the final = padding if any   
   pad count 1- + c@ [char] = = if -1 pad c+! then
;


: base64-decode-line   ( ca1 u1 -- ca2 u2 )
\ *G Decodes the base64 string *\i{ca1 u1}, skip (*\fo{ALF} and trim the final
\ ** padding if necessary.
   (base64-decode-line) dup 0<> if 1- swap char+ swap exit then =trim =trim
;


: base64-decode ( ca1 u1 ca2 u2 --  )
\ *G Decode a base64 string *\i{ca1 u1} and store the result into a file
\ ** whose path is specfied by *\i{ca2 u2}.   
   w/o bin create-file CREATE-FILE-EXCP ?throw
   begin
      >r base64-decode-line		\ caN uN
      pad count r@ write-file WRITE-FILE-EXCP ?throw \ caN uN
      dup 0= r> swap			\ caN uN fid flag
   until
   close-file CLOSE-FILE-EXCP ?throw 2drop
;

EXPORT base64-decode-line
EXPORT base64-decode

END-MODULE

\ ======
\ *> ###
\ ======
