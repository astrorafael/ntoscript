C" /usr/share/doc/VfxForth/Lib" SetMacro LIB      \ VFX library base path
C" ../externals"                SetMacro EXTERNALS
C" ../src"                      SetMacro SRC

include %SRC%/console/filelist.fs

also nto definitions


IndiServer: indiserver localhost 7624
\ s" localhost" 7624 indi-server heap-new constant indiserver


1 [if]
   indiserver connect
   defNumberVector indiserver +trace

indiserver properties get
2 seconds waiting
   




: pepe
   [device] Camera
;

   
: popo
   property [vector] Camera Code
;

: pupu
   property [item] Camera Code Microcode
;



: photo
   5.0 property item Camera Exposure ExpDur set
   property vector  Camera Exposure send&wait
;

." #### NOW THE STATE SMART WORDS" cr

device Camera print
property vector Camera Code print
property item Camera Code Microcode print   
   

   : raca
       device Camera
   ;

   : rece
      property vector Camera Code
   ;

   : roco
      property item Camera Code Microcode  
   ;
   
   
[endif]
   