((
$Date: 2008-05-14 22:03:33 +0200 (mié 14 de may de 2008) $
$Revision: 522 $
$Author: astrorafael $
$HeadURL: file:///home/desa/repos/ntoscript/trunk/src/core/dynbuffer.fs $

2008 By Rafael Gonzalez Fuentetaja
This file is placed in the public domain. NO WARRANTY.
))

\ ########################
\ *! dynbuffer
\ *R @node{Dynamic Buffer}
\ *T Dynamic Buffer
\ ########################

\ *P This module implements a dynamic buffer for XML parsing.
\ ** The buffer is dynamic in two ways:
\ *(
\ *B It may grow as necessary.
\ *B It may relocate its content from the buffer end to the beginning.
\ *)
\ ** The idea is to receive bytes from an stream (i.e. a socket) and
\ ** deposit them here for decoding. The decuding process should produce
\ ** a string *\i{c-addr len} suitable for XML parsing.

\ *P A rotating buffer should be inadecuate, because the contents may be split
\ ** into non-contiguous memory regions making it unsuitable for XML parsing.
\ ** Instead, shifting contents to the beginning of the buffer area is the
\ ** way to go.
\ ** The shifting policy (when to shift contents) is not defined in this file
\ ** and is deferred to the application (the background task in this case).

\ ***************
\ +S Design notes
\ ***************

\ +P The structure itself is quite simple containing pointers to the buffer
\ +* start, buffer end, a read pointer and a write pointer.
\ +* All these structure fields are exposed to the application for reading.
\ +* Updates are controlled by use of the proper words.

\ +P End pointers *\fo{xb,end} and *\fo{xb.wp} are technically one byte
\ +* past the area. This alows computing sizes and offsets by simple
\ +* substraction

\ ***********
\ *S Glossary
\ ***********

MODULE DynBuffer

\ =====================================
\ *N Buffer creation and initialization
\ =====================================

struct /XMLBuf				\ -- len
\ *G XML decoding buffer structure
\ *[   
   cell field xb.start			\ start of buffer area
   cell field xb.end			\ end of buffer area
   cell field xb.rp			\ read pointer
   cell field xb.wp			\ write pointer
end-struct
\ *]


4 KB Constant #XB-BYTES
\ *G Initial XML buffer size. Also serves as unit of further size increments.

: xb-init				  \ xmlbuf --
\ *G Initiates the XML reception buffer.
   >r
   #XB-BYTES ProtAlloc dup r@ xb.start !
   dup r@ xb.rp !
   dup r@ xb.wp !
   #XB-BYTES + r> xb.end !
;

: xmlbuf:				  \ "name" -- ; [child] -- xmlbuf
\ *G Use in the form *\fo{XMLBUF <name>}, creates a new xml buffer data
\ ** structure called *\fo{<name>} which returns the address of
\ ** the data structure *\i{xmlbuf} when executed.
   create here /XMLBuf allot xb-init
;

: new-xmlbuf				  \ -- xmlbuf
\ *G Creates an anonymous *\fo{/XMLBuf} in the heap.
\ ** Returns the *\i{xmlbuf} structure start address.
   /XMLBuf ProtAlloc dup xb-init
;

Export /XMLBuf
Export xmlbuf:
Export new-xmlbuf
Export xb-init
Export xb.rp

\ ------------
\ *H Accessors
\ ------------

: xb-wp-offset			 \ xmlbuf -- u1
\ +G Write pointer offset from beginning of buffer start area. Only used
\ +* in preserving the buffer state when growing the memory size.   
   dup xb.wp @ swap xb.start @ -
;

: xb-buffer				  \  xmlbuf -- c-addr u
\ *G Return the whole XMLBuf area.
   dup xb.start @ dup rot xb.end @ swap -
;

: xb-available				  \ xmlbuf -- c-addr u
\ *G Return the free space an the XMLBuf end.   
   dup xb.wp @ dup rot xb.end @ swap -
;

: xb-gap				  \  xmlbuf -- c-addr u
\ +G Gap between the start of XMLBuf and read pointer.   
   dup xb.start @ dup rot xb.rp @ swap -
;

: xb-used				  \ xmlbuf -- c-addr u
\ *G Return the used portion between the read and write pointers.   
   dup xb.rp @ dup rot xb.wp @ swap -
;

Export xb-buffer
Export xb-available
Export xb-used

\ ------------------
\ *H Pointer updates
\ ------------------

: xb-rp+!				  \ u1 xmlbuf --
\ *G Updates *\i{xmlbuf} read pointer after having processed *\i{u1} bytes. 
   xb.rp +!
;

: xb-wp+!				  \ u1 xmlbuf --
\ *G Updates *\i{xmlbuf} write pointer after having processed *\i{u1} bytes. 
   xb.wp +!
;

: xb-rp0				  \ xmlbuf --
\ +G Reset the read pointer to the buffer start.
   dup xb.start @ swap xb.rp !
;

: xb-wp0				  \ xmlbuf --
\ +G Reset the write pointer to the buffer start.
   dup xb.start @ swap xb.wp !
;

Export xb-rp+!
Export xb-wp+!

\ -----------------
\ *H Buffer growing
\ -----------------

: xb-save				  \ xmlbuf -- wp rp
\ *G Save state of XML buffer onto the data stack. Pointers are converted
\ ** to offsets *\i{wp} and *\i{rp} from the buffer start.
   dup >r xb-wp-offset r> xb-gap nip
;

: xb-restore			  \  wp rp xmlbuf --
\ *G Restore state of a previously saved XML Buffer.
\ ** Used in reallocating contents.  
   dup >r xb.start @			\ wp rp start
   tuck + r@ xb.rp !			\ wp start
   + r> xb.wp !
;

: (xb-grow)		      \ u1 xmlbuf1 --
\ *G Resize the buffer to new size *\i{u1}. New size should be greater
\ ** than current size.
   2dup xb.start @			\ u1 xmlbuf u1 start1
   swap ProtResize			\ u1 xmlbuf start2
   rot over + swap			\ xmlbuf end2 start2
   rot xb.start 2!			\ updates both pointers
;

: xb-grow			       \ u1 xmlbuf -- 
\ *G Grow XML buffer to *\i{u1} bytes , preserving state and contents.
\ ** Contents is preserved by *\fo{resize}.
   dup >r xb-save			\ u1 wp rp
   rot r@ (xb-grow)			\ wp rp
   r> xb-restore 
;

: xb-double				  \ xmlbuf --
\ *G Double the XMLBuf size.
   dup xb-buffer nip 2* swap xb-grow
;

Export xb-grow
Export xb-double

\ ----------------------
\ *H Buffer reallocation
\ ----------------------

: xb-move				  \ xmlbuf --
\ +G Moves the used contents between read and write pointers to buffer start.
   dup xb-used rot xb-gap drop swap move 
;

: xb-wp-shift				  \ xmlbuf --
\ +G Move the write pointer N positions from the buffer start,
\ +* where N was the former used area size.   
   dup xb-used nip over xb-wp0 swap xb-wp+!
;
   
: xb-realloc			          \ xmlbuf --
\ *G Realloc XML buffer contents by copying bytes bewteen the read and write
\ ** pointers to the buffer start and updating the pointers too.
   dup xb-move dup xb-wp-shift xb-rp0
;

: xb-?realloc				  \ xmlbuf --
\ +G If there is no available space at the end of the buffer for writting,
\ +* reallocate contents.
   dup xb-available nip 0<> if drop exit then xb-realloc
;

: xb-?grow				  \ xmlbuf --
\ +G If there is no available space at the end of the buffer for writting,
\ +* double the buffer size.   
   dup xb-available nip 0<> if drop exit then xb-double
;

   
: xb-realloc&grow			  \ xmlbuf --
\ *G Applies reallocation and buffer growing in this order if there is
\ ** no buffer space for writting.   
   dup xb-?realloc xb-?grow
;

Export xb-realloc
Export xb-realloc&grow

\ ------------
\ *H Debugging
\ ------------

: .xb-buffer			  \ xmlbuf --
\ *G Prints buffer structure for dubuggin purposes.
   base @ >r
   >r 
   ." Used = "  r@ xb-used nip decimal . hex
   ." START = " r@ xb.start @ u. 
   ." R = "     r@ xb.rp    @ u. 
   ." W = "     r@ xb.wp    @ u. 
   ." END = "   r> xb.end   @ u. 
   cr
   r> base !
;

Export .xb-buffer

END-MODULE

\ ======
\ *> ###
\ ======
