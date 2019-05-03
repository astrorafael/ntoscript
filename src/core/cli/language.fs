((
$Date: 2008-05-14 22:03:33 +0200 (mié 14 de may de 2008) $
$Revision: 522 $
$Author: astrorafael $
$HeadURL: file:///home/desa/repos/ntoscript/trunk/src/core/cli/language.fs $

2008 By Rafael Gonzalez Fuentetaja
This file is placed in the public domain. NO WARRANTY.
))

\ ######
\ *> cli
\ ######

\ ================================
\ *N Script programming constructs
\ ================================

\ *P Syntactic sugar to make scritps more readable for newcomers.


Alias: definition: :
\ *G Start a definition. 

Alias: end-definition ;
\ *G End a definition. 


Alias: fetch @				\ addr -- n
\ *G Fetch an integer given its address.

Alias: store !				\ n addr --
\ *G Store an integer in specified address.

Alias: 2fetch 2@			\ addr -- lo hi
\ *G Fetch a double integer starting an given address.

Alias: 2store 2!			\ lo hi addr --
\ *G Store a double integer at given address.

Alias: cfetch c@			\ addr -- c
\ *G Fetch a character given its address.

Alias: cstore c!			\ c addr --
\ *G Fetch a character given its address.

\ ======= 
\ *> ####
\ =======
