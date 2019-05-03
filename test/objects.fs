
C" /usr/share/doc/VfxForth/Lib" SetMacro LIB      \ VFX library base path
C" ../Lib"                  SetMacro LIB2     \ VFX library base path
C" ../externals"                SetMacro EXTERNALS
C" ../src"                      SetMacro SRC
C" ../build"                    SetMacro BUILD

include %LIB%/Ndp387.fth
include %EXTERNALS%/objects/struct.fs
include %EXTERNALS%/objects/objects.fs

interface
   selector sel1
   selector sel2
end-interface i-test1 persistant

interface
   selector sel3
   selector sel4
end-interface i-test2 persistant

object class
   i-test2 implementation
   
   Cell% inst-var m-count

   selector foo

end-class per1

per1 methods
public

  m:
     m-count off
  ;m  overrides construct

  m:
     ." count is " m-count ? cr
     1 m-count +!
  ;m  overrides print

  m:
     ." I'm foo selector" cr
  ;m overrides foo

  :m bar
     ." I'm bar method"
  ;m

  m:
     ." I'm selector 4" cr
  ;m overrides sel4
  
end-methods persistant
  
per1 dict-new constant p1

p1 print
  
p1 print
  
also forth save persis.elf cr previous
bye
  \ include objdbg.fs
