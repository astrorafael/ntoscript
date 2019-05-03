((
$Date: 2008-05-14 22:03:33 +0200 (mié 14 de may de 2008) $
$Revision: 522 $
$Author: astrorafael $
$HeadURL: file:///home/desa/repos/ntoscript/trunk/src/core/zpipe.fs $

2008 By Rafael Gonzalez Fuentetaja
This file is placed in the public domain. NO WARRANTY.
))

\ #############################
\ *! zpipe
\ *R @node{Compress/Uncompress}
\ *T Compress/Uncompress
\ #############################

\ *P This module defines all necesary words for compression/uncompression
\ ** using the Linux *\i{zlib} package. It is based on the *\i{zpipe}
\ ** standalone C utility by Mark Adler
\ ** that shows a canonical example of *\i{zlib} use.
\ ** It has been modified to suit *\f{NTOScript} needs. Input/Output is always
\ ** done from one file to another file.
\ ** This module internally use thread-safe *\fo{USER} variables and
\ ** using it is straightforward.

\ *P To compress a file named
\ ** *\i{bertie.jpg} and produce and other one called  *\i{bertie.jpg.z}
\ *+ simply call:
\ *E
\ **  s" bertie.jpg" zDeflate

\ *P To uncompress *\i{bertie.jpg.z} back again to *\i{bertie.jpg}
\ *+ simply call:
\ *E
\ **  s" bertie.jpg" zInflate

\ *P Output files are created in the same directory as the input files.

\ *P *\b{Warning:} The following module is for 32 bits cells, not 64. 


\ ***********
\ *S Glossary
\ ***********

MODULE ZPipe
\ *G


\ ==============
\ +N C Interface
\ ==============

[undefined] libz.so [if]
   Library: libz.so
[then]

\ -------------
\ +H Structures
\ -------------

\ +P Field types to help portability.

\ +[
cell field-type Bytef*			\ pointer to byte function
cell field-type uInt			\ unsigned int
cell field-type uLong			\ unsigned long
cell field-type char*			\ pointer to bytes
cell field-type struct*			\ pointer to ak an struct
cell field-type alloc_func		\ pointer to an alloc function
cell field-type free_func		\ pointer to an free function
cell field-type voidpf			\ pointer to an user defined object
\ +]


struct /z_stream
\ *G The z_stream C structure handled by zlib.   
\ +[   
   Bytef*   next_in	      \ next input byte 
   uInt     avail_in	      \ number of bytes available at next_in 
   uLong    total_in	      \ total nb of input bytes read so far 
   
   Bytef*   next_out	       \ next output byte should be put there 
   uInt     avail_out	       \ remaining free space at next_out 
   uLong    total_out	       \ total nb of bytes output so far 
   char*    msg		       \ last error message, NULL if no error 
   struct*  state	       \ not visible by applications 
   
   alloc_func zalloc	      \ used to allocate the internal state 
   free_func  zfree	      \ used to free the internal state 
   voidpf     opaque          \ private data object passed to zalloc and zfree 
   
   int     data_type          \ best guess about the data type: binary or text 
   uLong   adler	      \ adler32 value of the uncompressed data 
   uLong   reserved	      \ reserved for future use
end-struct
\ +]

\ -----------
\ +H zlib API
\ -----------

\ +P External C functions accessed by Forth.

\ +[
Extern: void * zlibVersion( void );
Extern: int deflateInit_(void * strm, int level , const char * version, int stream_size );
Extern: int deflate( void * strm, int flush );
Extern: void deflateEnd( void * strm );
Extern: int inflateInit_(void * strm, const char * version, int stream_size );
Extern: void inflateEnd( void * strm );
Extern: int inflate( void * strm, int flush );
\ +]

\ ============
\ *N Constants
\ ============

\ *P As defined by the *\i{zlib} library.

#define Z_DEFAULT_COMPRESSION  -1
\ *G Compression level.

#define CHUNK 16384
\ +G Memory size for block inflate/deflate

\ -----------------------
\ +H Allowed flush values
\ -----------------------

\ +[
#define Z_NO_FLUSH      0
#define Z_FINISH        4
\ +]

\ ---------
\ +H Events
\ ---------

\ +[
#define Z_OK            0
#define Z_STREAM_END    1
#define Z_NEED_DICT     2
\ +]

\ --------------
\ +H Error codes
\ --------------

\ +[
#define Z_ERRNO         -1
#define Z_STREAM_ERROR  -2
#define Z_DATA_ERROR    -3
#define Z_MEM_ERROR     -4
#define Z_BUF_ERROR     -5
#define Z_VERSION_ERROR -6
\ +]

NextError @
Value zErrBase
\ +G Base value to map error codes to exceptions below.

\ +[
ErrDef Z_ERRNO-EXCP   "Error reading input or output stream"
ErrDef Z_STREAM-EXCP  "Invalid comprssion level"
ErrDef Z_DATA-EXCP    "Invalid or incomplete deflate data"
ErrDef Z_MEM-EXCP     "Out of memory"
ErrDef Z_BUF-EXCP     "No progress possible in deflate (non fatal)"   
ErrDef Z_VERSION-EXCP "zlib version mismatch"
\ +]
   
: >zexcp				  \ n1 -- n2
\ +G Convert Zlib error codes into exceptions to throw.
   negate 1- zErrBase +
;

      
\ =================
\ +N User Variables
\ =================

cell +User zIn		
\ +G Reference to input buffer.

cell +User zOut
\ +G Reference to output buffer.

cell +User zInFd			
\ +G Input file descriptor.

cell +User zOutFd			
\ +G Output file descriptor

/z_stream +User strm
\ +G Per task stream structure.

\ ===============
\ +N Common words
\ ===============

\ +P Low level factors for both compression and uncompression.

: zInfile				\  ca1 u1 --
\ +G Open the input file in read-only, binary mode.
\ +* Input file descriptor left in *\fo{zInFd} USER variable.
   r/o bin open-file OPEN-FILE-EXCP ?throw
   zInFd !
;


: zOutfile				\  ca1 u1 -- 
\ +G Create the output file.
\ +* Output file descriptor left in *\fo{zOutFd} USER variable.
   w/o bin create-file CREATE-FILE-EXCP ?throw
   zOutFd !
;


: zInitUser				  
\ +G Initialize User variables.
   strm /z_stream erase  zIn off zOut off  -1 dup zInFd ! zoutFd !
;


: zAllocMem				  \ addr --
\ +G Allocate a *\fo{CHUNK} of memory and store it into variable *\i{addr}.   
   CHUNK ProtAlloc swap !
;


: zReleaseMem				  \  addr --
\ +G Release memory and initializes *\i{addr} to *\fo{NULL}.
   dup @ ?dup if swap off ProtFree exit then
   drop
;


: zCloseFile				  \ addr --
\ +G Close a file and set descriptor pointed by *\i{addr} to -1
   dup @ dup -1 = if 2drop exit then
   -1 rot !  close-file CLOSE-FILE-EXCP ?throw
;


: zFinish
\ +G Release allocated resources in an orderly manner.
\ +* Used both with normal and abnormal termination (exception caught)   
   strm /z_stream erase
   zIn    zReleaseMem
   zOut   zReleaseMem
   zInFd  zCloseFile
   zOutFd zCloseFile
;

: zRead				  \ ifd strm -- 
\ +G Read a *\fo{CHUNK} of data from input file descriptor *\i{ifd} and
\ +* store them into buffer pointed by  *\fo{zIn}.   
\ +* Also update *\i{strm} fields *\fo{next_in} and *\fo{avail_in}.   
   swap zIn @ CHUNK rot read-file  READ-FILE-EXCP ?throw
   over avail_in !			\ stores number read
   next_in zIn @ swap !			\ stores buffer start
;


: zWrite				  \ nbytes ofd --
\ +G Write *\i{nbytes} from buffer pointed by *\fo{zOut} to output file
\ +* *\i{ofd}, throwing exceptions if necessary.
   zOut @ -rot write-file WRITE-FILE-EXCP ?throw
;


\ ==============
\ +N Compression
\ ==============

\ -----------------
\ +H Initialization
\ -----------------

: zAppendPath	\  ca1 u1 -- ca2 u2
\ +G Append a *\f{.z} extension to input path given by *\i{ca1 u1}.
\ +* Leaves new string  *\i{ca2 u2} at the *\fo{PAD}
   PAD place s" .z" PAD append PAD count 
;

   
: zDefFinish				  \ -- 
\ +G Same as above but including first a call to *\i{deflateEnd()}.
\ +* Used both with normal and abnormal termination (exception caught)      
   strm deflateEnd
   zFinish
;


: zDefInit				  \  ca1 u1 --
\ +G Initializes variables, library and allocates resources for deflate.
\ +*Input file path given by *\i{ca1 u1}   
   zInitUser
   strm Z_DEFAULT_COMPRESSION zlibVersion /z_stream
   deflateInit_ dup >zexcp ?throw
   zIn  zAllocMem
   zOut zAllocMem
   2dup zInfile zAppendPath zOutFile
;

\ ---------------
\ +H Reading data
\ ---------------

: zFlush?				  \ strm -- strm flush
\ +G Return the flush (EOF) state, either *\fo{Z_FINISH} or *\fo{Z_NO_FLUSH}.
\ +** depending on the *\fo{CHUNK} size and the number of bytes read
\ +* (and stored in field *\fo{strm avail_in}).
   dup avail_in @ CHUNK <> if Z_FINISH exit then Z_NO_FLUSH
;


\ ------------------
\ +H Compresion loop
\ ------------------

: zDeflatePass				  \ strm flush -- nbytes
\ +G Invoke a single pass over *\fo{deflate} and return the number of bytes
\ +* *\i{nbytes} to write to output file.   
   over avail_out CHUNK  swap !
   over next_out  zOut @ swap !
   over swap deflate Z_STREAM_ERROR = Z_STREAM-EXCP ?throw
   avail_out @ CHUNK swap - 
;


: zDeflateChunk		        \ strm flush -- strm flush
\ +G Invoke *\fo{zDeflatePass} and write to file until no more available
\ +* bytes for output.
   zOutFd @ >r
   Begin
      2dup zDeflatePass			\ -- strm flush nbytes
      r@  zWrite			\ -- strm flush
      over avail_out @ 0<>		\ -- strm flush flag
   Until
   r> drop
;


\ ------------
\ +H Main loop
\ ------------

: (zDeflate)				\ ca1 u1 --
\ +G The actual deflating loop process (except resource releasing)
\ +* is done here.
   zDefInit
   zInFd @ strm				\ -- inFd strm
   Begin
      2dup zRead zFlush?		\ -- inFd strm flush
      zDeflateChunk			\ -- inFd strm flush
   Z_FINISH  =  Until			\ -- inFd strm
   2drop
;

   
: zDeflate				  \ ca1 u1 --
\ *G Take a file specified by path *\i{ca1 u1} and compress it
\ ** producing another file with the same path and .z extension appended.
\ ** Catch exceptions, releases resources and re-throws them again.
   ['] (zDeflate) catch zDefFinish throw
;


EXPORT zDeflate

\ ================
\ *N Uncompression
\ ================


\ -----------------
\ +H Initialization
\ -----------------

: zTrimPath	\  ca1 u1 -- ca1 u2
\ +G Trim 3 caracters at the end of the input path given by *\i{ca1 u1}.
\ +* These characters are supposed to be the *\f{.z} extension.
   [ 2 chars ] literal - 
;

   
: zInfFinish				  \ -- 
\ +G Same as above but including first a call to *\fo{inflateEnd}.
\ +* Used both with normal and abnormal termination (exception caught)      
   strm inflateEnd
   zFinish
;


: zInfInit				  \  ca1 u1 --
\ +G Initialize variables, library and allocate resources for inflate.
\ +*Input file path given by *\i{ca1 u1}.   
   zInitUser
   strm zlibVersion /z_stream inflateInit_ dup >zexcp ?throw
   zIn  zAllocMem
   zOut zAllocMem
   2dup zInfile zTrimPath zOutFile
;


\ --------------------
\ +H Uncompresion loop
\ --------------------

: zInfThrow				  \ retcode -- retcode
\ +G Analize return code from *\fo{inflate} and throw exceptions
\ +* for 4 possibles error cases, otherwise leave the retcode intact.
   case
      Z_STREAM_ERROR of Z_STREAM-EXCP throw endof
      Z_NEED_DICT    of Z_DATA-EXCP   throw endof
      Z_DATA_ERROR   of Z_DATA-EXCP   throw endof
      Z_MEM_ERROR    of Z_MEM-EXCP    throw endof
      dup
   endcase
   
;

: zInflatePass				  \ strm -- retcode nbytes
\ +G Invoke a single pass over *\fo{inflate} and returns the number of bytes
\ +* *\i{nbytes} to write to output file. Return *\i{retcode} that
\ +* is be used in the loop end detection in *\fo{(zInflate)}.
\ +* Also update *\i{strm} fields *\fo{avail_out} and *\fo{next_out}
   dup avail_out CHUNK  swap !
   dup next_out  zOut @ swap !
   dup Z_NO_FLUSH inflate zInfThrow
   swap avail_out @ CHUNK swap - 
;


: zInflateChunk		        \ strm -- strm retcode 
\ +G Invoke *\fo{zInflatePass} and write to file until
\ +* no more available bytes for output.
   zOutFd @ >r Z_OK			\ Z_OK is just a dummy value
   Begin				\ -- strm retcode
      drop dup zInflatePass		\ -- strm retcode nbytes
      r@  zWrite			\ -- strm retcode
      over avail_out @ 0<>		\ -- strm retcode flag
   Until				\ -- strm retcode
   r> drop
;

\ ------------
\ +H Main loop
\ ------------


: (zInflate)				\ ca1 u1 --
\ +G The actual inflating loop process (except resource releasing)
\ +* is done here.
   zInfInit
   zInFd @ strm				\ -- inFd strm
   Begin
      2dup zRead			\ -- inFd strm
      zInflateChunk			\ -- inFd strm retcode
   Z_STREAM_END = Until			\ -- inFd strm
   2drop
;


: zInflate				  \ ca1 u1 --
\ *G Take a *\f{.z} file specified by path *\i{ca1 u1} and uncompress it
\ ** producing another file with the same path but without the .z extension.
\ ** Catch exceptions, releases resources and re-throws them again.   
   ['] (zInflate) catch zInfFinish throw
;


EXPORT zInflate


END-MODULE


\ ======
\ *> ###
\ ======
