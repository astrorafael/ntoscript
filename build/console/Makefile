# $Date: 2008-08-24 03:45:43 +0200 (dom 24 de ago de 2008) $
# $Revision: 559 $
# $Author: astrorafael $
# $HeadURL: file:///home/desa/repos/ntoscript/trunk/build/console/Makefile $
# 
# 2008 By Rafael Gonzalez Fuentetaja
# This file is placed in the public domain. NO WARRANTY.

DIRS= . src src/core src/core/dbase src/skymapping test test/dbase \
test/mapping scripts Lib docgen docgen/userman docgen/refman

binaries: ntoscript.i686 ntoscript.i386

all: ntoscript.i686 ntoscript.i386 doc

ntoscript.i686:  
	vfxlin include ntoscript.fs
	mv ntoscript.elf ntoscript.i686


ntoscript.i386:  
	vfxlin386 create vfxlin386 include ntoscript.fs
	mv ntoscript.elf ntoscript.i386

doc:
	cd ../../docgen/userman ; ./docgen.sh userman 
	cd ../../docgen/refman ; ./docgen.sh refman 

dist:
	./assemble.sh

clean:
	rm -f ntoscript.i686 ntoscript.i386
	rm -f ../../doc/*.pdf 
	rm -f ../../doc/*.html
