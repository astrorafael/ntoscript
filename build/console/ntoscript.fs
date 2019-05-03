((
$Date: 2008-08-24 03:45:43 +0200 (dom 24 de ago de 2008) $
$Revision: 559 $
$Author: astrorafael $
$HeadURL: file:///home/desa/repos/ntoscript/trunk/build/console/ntoscript.fs $

2008 By Rafael Gonzalez Fuentetaja
This file is placed in the public domain. NO WARRANTY.
))

\ #################
\ *> build
\ *T Forth Makefile
\ #################

\ ***********
\ *S Glossary
\ ***********

\ ==========================
\ *N Application compilation
\ ==========================

\ *[
C" /usr/share/doc/VfxForth/Lib" SetMacro LIB      \ VFX library base path
C" ../../externals/Lib"            SetMacro LIB2 \ Customized VFX library
C" ../../src"                      SetMacro SRC
C" ../../externals"                SetMacro EXTERNALS

include filelist.fs		          \ The master file list.
include turnkey.fs		          \ Turnkey generation

set-app-vectors

also forth 
save ntoscript.elf cr
previous

update-build
bye
\ *]

\ ======
\ *> ###
\ ======


