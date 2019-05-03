C" /usr/share/doc/VfxForth/Lib"    SetMacro LIB      \ VFX library base path
C" ../../Lib"                      SetMacro LIB2     \ VFX library base path
C" ../../externals"                SetMacro EXTERNALS
C" ../../src"                      SetMacro SRC
C" ../../build"                    SetMacro BUILD

include %BUILD%/console/filelist.fs

also nto definitions

: C1
   S" Annonymous,f, 0:58:10.4, 44:01:35,0.00,2000,0"
;

: C2
   S" And Phi-42A,f|D|B7, 1:09:30.1|5.55, 47:14:30|-13,4.26,2000,0"
;

: C3
   S" M32,f|H|E2, 0:42:41.8, 40:51:57,8.10,2000,510|390|179.294"
;

: C4
   S" Tuc Epsilon|SAO 255619|HD 224686|CP-66 3819|PPM 366744|J235954.97-653437.6,f|S|B9,23:59:54.98|48.62,-65:34:37.67|-22.3,4.50,2000"
;

EDB-Table: my-edb-parser

:noname
   ." Name: " type cr
; Subfield1 edb-handler-for my-edb-parser

:noname
   ." Other Name: " type cr
; Subfield1A edb-handler-for my-edb-parser

:noname
   ." Type: " type cr
; Subfield2 edb-handler-for my-edb-parser

:noname
   ." Subtype: " type cr
; Subfield2A edb-handler-for my-edb-parser

:noname
   ." Subfield 2B: " type cr
; Subfield2B edb-handler-for my-edb-parser

:noname
   ." RA: " type cr
; Subfield3 edb-handler-for my-edb-parser

:noname
   ." RA ppm: " type cr
; Subfield3A edb-handler-for my-edb-parser

:noname
   ." DEC: " type cr
; Subfield4 edb-handler-for my-edb-parser

:noname
   ." DEC ppm: " type cr
; Subfield4A edb-handler-for my-edb-parser

:noname
   ." Mag: " type cr
; Subfield5 edb-handler-for my-edb-parser

:noname
   ." Epoch: " type cr
; Subfield6 edb-handler-for my-edb-parser

:noname
   ." Field7: " type cr
; Subfield7 edb-handler-for my-edb-parser

:noname
   ." Subfield7A: " type cr
; Subfield7A edb-handler-for my-edb-parser

:noname
   ." Subfield7B: " type cr
; Subfield7B edb-handler-for my-edb-parser


: alt1
   ." DEC (alt): " type cr
;

: alt2
   ." R.A. (alt): " type cr
; 

: install-alt1
   ['] alt1 Subfield4 ['] my-edb-parser  (edb-handler-for)
;

: install-alt2
   ['] alt2 Subfield3 [edb-handler-for] my-edb-parser
;



: parse-edb-name			\ ca1 u1 -- ca2 u2
   edb-field  edb-subfield  Subfield1 my-edb-parser  \ -- ca2 u2 ca3 u3	
   begin dup while edb-subfield  Subfield1A my-edb-parser repeat
   2drop 
;

: parse-edb-type			  \ ca1 u1 -- ca2 u2
   edb-field edb-subfield  Subfield2 my-edb-parser
   dup 0= if 2drop  exit then edb-subfield  Subfield2A my-edb-parser
   dup 0= if 2drop  exit then edb-subfield  Subfield2B my-edb-parser
   dup 0= if 2drop  exit then edb-subfield  Subfield2C my-edb-parser
   2drop 
;

: parse-edb-ra			  \ ca1 u1 -- ca2 u2
   edb-field edb-subfield   Subfield3 my-edb-parser
   dup 0= if 2drop  exit then edb-subfield  Subfield3A my-edb-parser
   2drop 
;

: parse-edb-dec			  \ ca1 u1 -- ca2 u2
   edb-field edb-subfield  Subfield4 my-edb-parser
   dup 0= if 2drop  exit then edb-subfield  Subfield4A my-edb-parser
   2drop 
;

: parse-edb-mag			  \ ca1 u1 -- ca2 u2
   edb-field edb-subfield  Subfield5 my-edb-parser
   2drop 
;

: parse-edb-epoch			  \ ca1 u1 -- ca2 u2
   edb-field
   dup 0= if 2drop exit then edb-subfield  Subfield6 my-edb-parser
   2drop 
;

: parse-edb-field7			   \ ca1 u1 -- ca2 u2
   edb-field 
   dup 0= if 2drop  exit then edb-subfield  Subfield7  my-edb-parser
   dup 0= if 2drop  exit then edb-subfield  Subfield7A my-edb-parser
   dup 0= if 2drop  exit then edb-subfield  Subfield7B my-edb-parser
   2drop 
;

: parse-edb				  \ ca1 u1
   parse-edb-name
   parse-edb-type
   parse-edb-ra
   parse-edb-dec
   parse-edb-mag
   parse-edb-epoch
   parse-edb-field7
   2drop
;



