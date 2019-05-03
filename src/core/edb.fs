((
$Date: 2008-05-14 22:03:33 +0200 (mié 14 de may de 2008) $
$Revision: 522 $
$Author: astrorafael $
$HeadURL: file:///home/desa/repos/ntoscript/trunk/src/core/edb.fs $

2008 By Rafael Gonzalez Fuentetaja
This file is placed in the public domain. NO WARRANTY.
))

\ ###########################
\ *> edb
\ *R @node{EDB Utility words}
\ *T EDB Utility words
\ ###########################

\ ***************
\ *S Introduction
\ ***************

\ *P Most catalogs in XEPhem are text files whose objects are encoded line
\ ** by linein a text format that is usually called an EDB line.
\ ** Thus, a catalog is a series of EDB lines.
\ ** The EDB file format is described in the XEphem help pages.

\ *P An EDB line  consist of various fields separated by the comma character.
\ ** There are different object types, so the information for each is different
\ ** and the edb line has a varying number of fields. 
\ ** Most objects have 7 fields, but comets and minor planets need up to 14.

\ *E  Annonymous,f, 0:58:10.4, 44:01:35,0.00,2000,0
\ **  And Phi-42A,f|D|B7, 1:09:30.1|5.55, 47:14:30|-13,4.26,2000,0
\ **  M32,f|H|E2, 0:42:41.8, 40:51:57,8.10,2000,510|390|179.294
\ **  Tuc Epsilon|SAO 255619,f|S|B9,23:59:54.98|48.62,-65:34:37.67|-22.3,4.50,2000

\ *P Within a field, there may be additional subfields, separated by the
\ ** pipe character *\f{|}, also depending on the object type and available
\ ** information.

\ *P It is really difficult to write an universal edb parser that matches all
\ ** needs. Instead, custom parsers can be written on demand depending on the
\ ** application purpose, using the words defined here.
\ ** The basic toolset consist of words *\fo{EDB-FIELD} and *\fo{EDB-SUBFIELD}.
\ ** These words take an edb line and produce a 'matched field' and the
\ ** 'remaining string'. The matched field must be consumed by the application,
\ ** leaving the remaining string ready for next invocation of these words.
\ ** This process may be applied until either the application does not need
\ ** to decode further subfields or the remaining string length is zero.

\ *P Another approach to parsing is "event driven parsing", similar to SAX
\ ** parser in Java. A main loop parse an EDB line and calls *\fo{DEFER}red
\ ** words for each field and subfield.
\ ** These hooks receive the input subfield string *\i{ca u}.
\ ** The subfield string is not trimmed
\ ** for leading or traling whitespaces, tabs or CRs to avoid this calling
\ ** overhead when sweeping large files. It's up to the application to do this
\ ** if necessary.

\ *P Again, it is not practical to write a generic main loop that parse
\ ** every possible field and subfield. With files of hundred thousand
\ ** lines, the loop must be as lightweight as possible.
\ ** However, utility words in this file provide the basic support for
\ ** registering application 'hooks' for every possible subfield.

\ *P The example below is an event-driven parser *\fo{my-edb-parser} that is
\ ** only interested in the main name, alternate names, type and subtype,
\ ** RA and DEC with their proper motions.

\ *E : parse-edb-name		   \ ca1 u1 -- ca2 u2
\ **    edb-field  edb-subfield   Subfield1 edb-exec
\ **    begin dup while edb-subfield  Subfield1A edb-exec repeat
\ **    2drop
\ ** ;
\ ** 
\ ** : parse-edb-type		  \ ca1 u1 -- ca2 u2
\ **    edb-field edb-subfield  Subfield2 edb-exec
\ **    dup 0= if 2drop exit then edb-subfield  Subfield2A edb-exec
\ **    dup 0= if 2drop exit then edb-subfield  Subfield2B edb-exec
\ **    2drop
\ ** ;
\ ** 
\ ** : parse-edb-RA		  \ ca1 u1 -- ca2 u2
\ **    edb-field edb-subfield  Subfield3 edb-exec
\ **    dup 0= if 2drop exit then edb-subfield  Subfield3A edb-exec
\ **    2drop
\ ** ;
\ ** 
\ ** : parse-edb-dec		  \ ca1 u1 -- ca2 u2
\ **    edb-field edb-subfield  Subfield4 edb-exec
\ **    dup 0= if 2drop exit then edb-subfield  Subfield4A edb-exec
\ **   2drop
\ ** ;
\ ** 
\ ** : my-edb-parser		  \ ca1 u1 --
\ **    parse-edb-name
\ **    parse-edb-type
\ **    parse-edb-ra
\ **    parse-edb-dec
\ **    2drop
\ ** ;


\ ***********
\ *S Glossary
\ ***********


\ =====================
\ *N Basic parser words
\ =====================

: edb-field				  \ ca1 u1 -- ca2 u2 ca3 u3
\ *G Parse one field (delimited by comma) of an edb line.
\ ** Return remaining string in *\i{ca2 u2}.   
\ ** Return string matched in *\i{ca3 u3}. If not found, *\i{u3} = 0.
   [char] , split 
;

: edb-subfield				\ ca1 u1 -- ca2 u2 ca3 u3
\ *G Parse one subfield (delimited by *\f{|}) in an edb line.
\ ** Return remaining string in *\i{ca2 u2}.   
\ ** Return string matched in *\i{ca3 u3}. If not found, *\i{u3} = 0.
   [char] | split
;

: skip-field	 \ ca1 u1 -- ca2 u2
\ *G From remaining string *\i{ca1 u1}, skip next field and produce next
\ ** remaining string *\i{ca2 u2}.
     edb-field 2drop 
;


\ =======================
\ *N Event driven parsing
\ =======================

31 Constant EDB-SUBFIELDS
\ *G Number of different subfields in all varieties of edb lines

: EDB-Table:				  \ "name" -- ; [child] ca1 u1 +n1 --
\ *G Create a vector of EDB handlers in the dictionary and initializes
\ ** it to empty handlers (*\fo{2DROP}).
\ ** At run time, child parser executes the hander given by subfield offset
\ ** *\i{n1}with the input string *\i{ca1 u1}.   
   Create here EDB-SUBFIELDS cells dup allot
   bounds DO  ['] 2drop I !    cell +LOOP
 Does>
    swap cells + @ execute 
;

: (edb-handler-for)				  \ xt1 +n1 xt2 --
\ *G Install handler *\{xt1} for sublabel offset *\i{n1} in edb parser
\ ** given by *\i{xt2} in the input stream.
   >BODY swap cells + !
;
: edb-handler-for				  \ xt +n1 "name" --
\ *G Install handler *\{xt} for sublabel offset *\i{n1} in edb parser
\ ** given by name in the input stream. Usage:
\ *C ' my-handler edb-handler-for my-edb-parser
   ' (edb-handler-for)
;

: [edb-handler-for]
\ *G Compile only counterpart for *\fo{edb-handler-for}. Usage:
\ *C ['] my-handler [edb-handler-for] my-edb-parser  
   ?comp postpone ['] postpone (edb-handler-for)
; immediate


\ ------------------
\ *H Handler offsets
\ ------------------

\ *P Constant representing offets in the table of handlers
\ ** Their meaning is totally depending on the object type/subtype.

0 Constant Subfield1
\ *G Object main name.

1 Constant Subfield1A
\ *G Object alternate name.

2 Constant Subfield2
\ *G Object type designation.

3 Constant Subfield2A
\ *G Object subclass (type = 'f'). 
\ ** Binary class code (type ='B').

4 Constant Subfield2B
\ *G Object spectral designation or additional info (type = 'f'). 
\ ** Primary star Spectral class (type ='B').

5 Constant Subfield2C
\ *G Secondary star Spectral class (type ='B').

6 Constant Subfield3
\ *G Right Ascension (type ='f'). 
\ ** Inclination     (type ='e'). 
\ ** Date at the epoch of perihelion (type ='h'). 
\ ** Date at the epoch of perihelion (type ='p'). 
\ ** Epoch of the other fields (type ='E').

7 Constant Subfield3A
\ *G Right Ascension *\f{ppm * cos(Dec)} (type ='f').
\ ** First date elements valid (type ='h').
\ ** First date elements valid (type ='p').
\ ** First date elements valid (type ='E').

8 Constant Subfield3B
\ *G Last date elements valid (type ='h').
\ ** Last date elements valid (type ='p').
\ ** Last date elements valid (type ='E').

9 Constant Subfield4
\ *G Declination (type ='f').
\ ** Longitude of ascending node (type ='e').
\ ** Inclination (type ='h').
\ ** Inclination (type ='p').
\ ** Inclination (type ='E').

10 Constant Subfield4A
\ *G Declination proper motion (type ='f').

11 Constant Subfield5
\ *G Magnitude (type ='f').
\ ** Argument of perihelion (type ='e').
\ ** Longitude of ascending node (type ='h').
\ ** Argument of perihelion (type ='p').
\ ** RA of ascending node (type ='E').


12 Constant Subfield5A
\ *G Primary star magnitude (type ='B').

13 Constant Subfield5B
\ *G Secondary star magnitude (type ='B').

14 Constant Subfield6
\ *G Reference epoch (type ='f').
\ ** Semi-major axis (type ='e').
\ ** Argument of perihelion (type ='h').
\ ** Perihelion distance (type ='p').
\ ** Eccentricity (type ='E').

15 Constant Subfield7
\ *G Galaxy major axis (type = 'f') (2A = G , H).
\ ** Star separation pair (type = 'f') (2A = B , D).
\ ** Object size (type = 'f') (other).
\ ** Mean daily motion (type ='e').
\ ** Eccentricity (type ='h').
\ ** Longitude of ascending node (type ='p').
\ ** Argument of perigee (type ='E').
\ ** Year of separation  (type ='B').
\ ** Semi-major axis     (type ='B').

16 Constant Subfield7A
\ *G Galaxy minor axis  (type = 'f') (2A = G , H).
\ ** Reserved, set to 0 (type = 'f') (2A = B , D).
\ ** Stars separation  (type ='B').
\ ** Inclination plane (type ='B').

17 Constant Subfield7B
\ *G Galaxy major axis PA (type = 'f') (2A = G , H).
\ ** Stars PA (type = 'f') (2A = B , D).
\ ** Longitude of node (type ='B').

18 Constant Subfield7C
\ *G Year of separation (type ='B').
\ ** Eccentricity (type ='B').

19 Constant Subfield7D
\ *G Stars separation  (type ='B').
\ ** Epoch of periastron (type ='B').

20 Constant Subfield7E
\ *G Stars PA (type ='B').
\ ** Argument of periastron (type ='B').

21 Constant Subfield7F
\ *G Period (type ='B').

22 Constant Subfield8
\ *G Eccentricity (type ='e').
\ ** Perihelion distance (type ='h').
\ ** Equinox year (type ='p').
\ ** Mean anomaly (type ='E').

23 Constant Subfield9
\ *G Mean anomaly (type ='e').
\ ** Equinox year (type ='h').
\ ** 'g' component of magnitude model (type ='p').
\ ** Mean motion (type ='E').

24 Constant Subfield10
\ *G Epoch date (type ='e').
\ ** 'g' component of magnitude model (type ='h').
\ ** 'k' component of magnitude model (type ='p').
\ ** Orbit decay rate (type ='E').

25 Constant Subfield10A
\ *G First date elements valid (type ='e').

26 Constant Subfield10B
\ *G Last date Elements valid (type ='e').

27 Constant Subfield11
\ *G Equinox year (type ='e').
\ ** 'k' component of magnitude model (type ='h').
\ ** Angular size at 1 AU (type ='p').
\ ** Integral reference orbit number at epoch (type ='E').

28 Constant Subfield12
\ *G First component of magnitude model (type ='e').
\ ** Angular size at 1 AU (type ='h').
\ ** Drag coefficient (type ='E').

29 Constant Subfield13
\ *G Second component of magnitude model (type ='e').

30 Constant Subfield14
\ *G Angular size at 1 AU (type ='e').


\ ======
\ *> ###
\ ======

