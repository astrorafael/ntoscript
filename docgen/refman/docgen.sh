#!/bin/sh
# 
# $Date: 2008-07-11 13:17:48 +0200 (vie 11 de jul de 2008) $
# $Revision: 545 $
# $Author: astrorafael $
# $HeadURL: file:///home/desa/repos/ntoscript/trunk/docgen/refman/docgen.sh $
#
# 2008 By Rafael Gonzalez Fuentetaja
# This file is placed in the public domain. NO WARRANTY.


userman="userman"
refman="refman"
doc="../../doc"

if test $# -ne 1
then
    echo "Usage: $0 ( $userman | $refman )"
    exit 0
fi

if test $1 != "$userman" -a $1 != "$refman" 
then
    echo "Incorrect argument: $1"
    echo "Usage: $0 ( $userman | $refman )"
    exit 0
fi

mode=$1

if test ! -d $doc
then
    mkdir $doc || exit
fi


# generate .tex and html files by VFX's DocGen
vfxlin create $mode include docgen.dgs

# Rerun twice to generate proper TOC

# To generate an index texify -p manual.tex must be run instead of pdftex
# texify -p manual.tex
# texify -p manual.tex

pdftex refman.tex 
pdftex refman.tex

echo "copying manuals to the doc folder"
mv refman.pdf $doc
mv *.html     $doc


echo "cleaning again temporary files"
rm refman.* 
rm *.tex    
