
C" /usr/share/doc/VfxForth/Lib" SetMacro LIB      \ VFX library base path
C" ../Lib"                  SetMacro LIB2     \ VFX library base path
C" ../externals"                SetMacro EXTERNALS
C" ../src"                      SetMacro SRC
C" ../build"                    SetMacro BUILD

include %LIB%/Ndp387.fth
include %EXTERNALS%/objects/struct.fs
include %EXTERNALS%/objects/objects.fs



: .interface				\ ifce --
   >R
   cr
   r@ interface-map 2@ swap   ." interface-map (addr)    = " .LWORD cr
   ." interface-map (size)    = " . cr 
   r@ interface-map-offset @  ." interface-map-offset    = " . cr
   r@ interface-offset @      ." interface-offset        = " . cr
   r> drop
;

: .interfaces				  \ class
   dup interface-map-offset @ swap interface-map 2@ drop swap bounds
   ?do
      i @  if
      ." points to interface map = " i @ .lword
      ."  whose interface structure is " i @ @ .lword cr	 
      then
      
   4 +loop
;

: .class				  \ class --
   >r
   r@ .interface
   r@ class-parent   @ ." class-parent            = " .LWORD cr
   r@ class-wordlist @ ." class-wordlist          = " .lword cr
   r@ class-inst-size 2@ swap ." class-inst-size (align) = " . cr
   ." class-inst-size (size)  = " . cr
   r@ .interfaces
   r> drop
;

: .methods				  \ class --
   
;



interface
   selector sel1
   selector sel2
end-interface i-test1 persistant

interface
   selector sel3
   selector sel4
end-interface i-test2

interface
   selector sel
   selector sel6
end-interface i-test3

object class
   i-test1  implementation
   i-test3 implementation
   
   Cell% inst-var m-count

   selector foo

end-class per1 persistant
