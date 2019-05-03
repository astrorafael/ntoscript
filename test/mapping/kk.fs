C" /usr/share/doc/VfxForth/Lib" SetMacro LIB      \ VFX library base path
C" ../../Lib"                      SetMacro LIB2
C" ../../externals"                SetMacro EXTERNALS
C" ../../src"                      SetMacro SRC
C" ../../build"                    SetMacro BUILD

include %BUILD%/console/filelist.fs

foreground logging on
background logging on
indi logging on
5 to property-timeout

IndiServer: indiserver localhost 7624

include xephem.fs

DEFINITION: discover   ( -- )
  indiserver connect
  indi-message    indiserver -trace
  defNumberVector indiserver -trace
  setNumberVector indiserver -trace
  indiserver properties get
  2 seconds waiting
END-DEFINITION


discover




DEFINITION: BadFocus  ( -- )
 523.0     property item   OTA Focuser Focus set
      property vector OTA Focuser send&ok
END-DEFINITION





   


