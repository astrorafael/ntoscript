((
$Date: 2008-08-24 12:31:35 +0200 (dom 24 de ago de 2008) $
$Revision: 563 $
$Author: astrorafael $
$HeadURL: file:///home/desa/repos/ntoscript/trunk/src/core/cli/datetime.fs $

2008 By Rafael Gonzalez Fuentetaja
This file is placed in the public domain. NO WARRANTY.
))

\ ################################
\ *> cli
\ *R @node{Command Line Interface}
\ *T Command Line Interface
\ ################################

\ *P An assorted collection of words for command line interface scripting.

\ ***********
\ *S Glossary
\ ***********

\ ========================
\ *N Calendar, date & time
\ ========================


[undefined] seconds [if]
: seconds		 \ +n1 -- +n2
\ *G Convert seconds to milliseconds.
   #1000 *
;
[then]


: minutes	       \ +n1 -- +n2
\ *G  Convert minutes to milliseconds.
   #60 * seconds
;


Alias: waiting	ms			  \ +n1 --
\ *G Wait a number of milliseconds. Calls *\fo{PAUSE}. Usage:
\ *C  34 seconds waiting
\ *C   5 minutes waiting



: sexag>split				  \ n1 u2 -- hh mm ss
\ +G Split a sexagesimal number *\i{n1 u2} representing time 
\ +* into hours, minutes and seconds.
\ +* Only valid for *\f{HH:MM} and *\f{HH:MM:SS} formats.  
   /mod
   dup 0= if 0 exit then
   swap 60 /mod
   dup 0= if exit then
   swap
;


: GMT					   \ +n1 u2  -- hh mm ss True 
\ *G Greenwich Mean Time (or UTC) specifier.
\ ** *\i{n1 u2} is the sexagesimal number representing time,
\ ** written as *\f{HH:MM} or *\fo{HH:MM:SS}.    
   sexag>split True
;


: LT					  \ +n1 u2 -- hh mm ss False
\ *G Local Time specifier.
\ ** *\i{n1 u2} is the sexagesimal number representing time,
\ ** written as *\f{HH:MM} or *\fo{HH:MM:SS}.   
   sexag>split False
;


: (at-local-time)			    \ ss mm hh dd mm yyyy --
\ +G Wait until a given local time & date.
\ +* Date is given in Forth time&date format.
   >timestamp begin 2dup time&date >timestamp d> while 1 ms repeat
   2drop
;


: (at-gmt-time)				   \ ss mm hh dd mm yyyy --
\ +G Wait until a given GMT time & date.
\ +* Date is given in Forth time&date format.   
   >timestamp begin 2dup gmtime&date >timestamp d> while 1 ms pause repeat
   2drop
;


: at-time				\ yyyy mm dd hh mm ss flag -- 
\ *G Wait until a given *\fo{GMT} or *\fo{LT} date&time. Calls *\fo{PAUSE}.
\ ** Usage:
\ *C    2008 November 11  15:23:30 GMT at-time
\ *C    2008 November 11  16:23:30 LT  at-time
{  year month day hour minute second timeflag -- }   
   second minute hour day month year
   timeflag if
      (at-gmt-time)
   else
      (at-local-time)
   then
;

\ ======= 
\ *> ####
\ =======
