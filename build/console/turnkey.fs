((

$Date: 2008-07-11 12:46:33 +0200 (vie 11 de jul de 2008) $
$Revision: 544 $
$Author: astrorafael $
$HeadURL: file:///home/desa/repos/ntoscript/trunk/build/console/turnkey.fs $

2008 By Rafael Gonzalez Fuentetaja
This file is placed in the public domain. NO WARRANTY.
))
\ #################################
\ *> build
\ *T Turnkey application generation
\ #################################


\ ***********
\ *S Glossary
\ ***********

only forth definitions


\ =============================
\ *N Subversion release tagging
\ =============================

\ *P Extract the subversion release tag at run time to show in the banner.
\ ** These words rely on the fact that Subversion projects are usually
\ ** organized using the following convention:

\ *E svn://<host>/<project>/trunk/<real project directory tree>
\ ** svn://<host>/<project>/branches/<a branch>/<real project directory tree>
\ ** svn://<host>/<project>/tags/<a tag>/<real project directory tree>

$Constant URL $HeadURL: file:///home/desa/repos/ntoscript/trunk/build/console/turnkey.fs $"
\ *G The Subversion URL string.

$Constant trunk trunk/"
\ *G Tag mark.

$Constant tags tags/"
\ *G Tag mark.

$Constant branches branches/"
\ *G Tag mark.

: trunk?				  \ $url -- index true | false
\ *G Is *\i{$URL} in trunk URL ?
\ ** *\i{Index} shows where the *\i{"trunk"} substring starts.
   trunk $instr
;

: branches?				  \ $url -- index true | false
\ *G Is *\i{$URL} a branch URL ?
\ ** *\i{Index} shows where the *\f{"branches"} substring starts.   
   branches $instr
;

: tags?					  \ $url -- index true | false
\ *G Is *\i{$URL} a tag URL ?
\ ** *\i{Index} shows where the *\f{"tags"}  substring starts.      
   tags $instr
;

: /subdir/				\ $url n1 -- ca1 u1
\ *G Isolate the subdirectory string in a Subversion URL (counted string *\i{$url})
\ ** right after the *\f{"tags/"} or  *\f{"branches/"} portion, following
\ ** the convention that tags or branches are labelled as directories after
\ ** those substrings above mentioned.
\ ** *\{n1} is the string length of either  *\f{"tags/"} or  *\f{"branches/"}    
   >R swap count rot /string		\ skip up to "<subdir>/"
   R>  /string				\ skip "<usbdir>/" itself
   2dup [char] / scan nip - 		\ isolates the $tag subdirectory itself
;


: /tag/				  \ $url -- ca1 u1
\ *G Isolate the tag substring in a Subversion *\i{$url}, using *\fo{/subdir/} above.
   [ tags $len ] literal /subdir/ 
;

: /branch/				  \ $url -- ca1 u1
\ *G Isolate the branch substring in a Subversion *\i{$url},
\ ** using *\fo{/subdir/} above.   
   [ branches $len ] literal /subdir/ 
;

: /trunk/				  \ $url -- ca1 u1
\ *G Return the *\f{"trunk"} substring in a Subversion *\i{$url}.
   drop trunk count 1-
;

: svn-tag		      \ $url -- ca1 u1
\ *G Extract the SVN tag release string *\i{ca1 u1} at run-time from
\ ** counted string *\i{$url}.   
   dup branches? if /branch/ exit then
   dup tags?     if /tag/    exit then
   /trunk/
;

\ =====================
\ *N Turnkey generation
\ =====================

s" build.no" Set-BuildFile

#32 Buffer: MyBuild$
\ *G Build revision string.

MyBuild$ make-build


: .nto-release
   ."  Console version, " URL svn-tag type ."  release " MyBuild$ count type
;

: banner				  \ --
\ *G Print a welcome message to the main task default I/O device.
   page
   ."  ----------------------- NTOSCRIPT -------------------------"  cr
   ."  A Forth scripting and control environment for INDI devices."  cr
   .nto-release cr
   ."  © by Rafael Gonzalez Fuentetaja, 2008."                       cr
   ."  © INDI by Elwood Charles Downey, 2003-2008."                  cr
   cr
   ."  Powered by " PRODUCT-TITLE $. ."  for " HOST_TARGET $.        cr
   ."  © MicroProcessor Engineering Ltd, " COPYRIGHT-DATE $.
   .release cr
   61 dashes cr
;



: ntoscript				  \ HMODULE 0 COMMANDLINE SHOW -- RES
\ *G The *\f{NTOSCRIPT} application entry point. *\fo{EVALUATE}s the
\ ** command line, resets the data stack and enters the VFX Forth outer interpreter
\ ** by calling *\fo{QUIT}   
\ +* Apparently, there is one more item in the data stack before
\ +* calling *\fo{QUIT}.
   16 set-precision
   also nto
   [ also nto ] also ntoscript [ previous ] definitions
   drop asciiz>$ count			\ convert to ca u string
   BL scan evaluate		 \ skip program name and eval the rest
   s0 @ sp!
   quit
;

: set-app-initial				  \ --
\ *G Adds some words to the cold chain and assigns application entry point.
   ['] banner     AtCold
   ['] ntoscript is EntryPoint
; 

: set-app-final				  \ --
\ *G Adds some words to the exit chain.
   ( ['] term-multi AtExit )		\ causes a SIGSEGV
; 

: set-app-vectors			  \ --
\ *G Set all the application initial and final vectors.
   set-app-initial set-app-final
; 


\ ======
\ *> ###
\ ======
