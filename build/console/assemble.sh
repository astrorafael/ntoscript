#!/bin/sh
# --------------------------------------------------------
# $Date: 2008-07-11 20:10:31 +0200 (vie 11 de jul de 2008) $
# $Revision: 547 $
# $Author: astrorafael $
# $HeadURL: file:///home/desa/repos/ntoscript/trunk/build/console/assemble.sh $
# 
# 2008 By Rafael Gonzalez Fuentetaja
# This file is placed in the public domain. NO WARRANTY.
# --------------------------------------------------------

# Purge previous packages
rm *.deb
rm *.gz

# Find out the latest build string from the VFX file.
BUILD=`grep build build.no | cut -c8-11`
BUILD=`expr $BUILD - 2` 



# Set the common branch and SVN version for all binaries
BRANCH=`grep HeadURL ntoscript.fs | cut -d/ -f6`
VERSION=`grep HeadURL ntoscript.fs | cut -d/ -f7`


DIR=ntoscript
mkdir -p $DIR/usr/bin
mkdir -p $DIR/usr/share/$DIR/scripts
mkdir -p $DIR/usr/share/doc/$DIR/
cp ../../scripts/*.fs                   $DIR/usr/share/$DIR/scripts
cp ../../doc/userman.pdf                $DIR/usr/share/doc/$DIR/
rm -fr  $DIR/usr/bin/ntoscript
  

# Assemble pure i386 and Pentium IV binaries

for ARCH in i686 i386
do

# Make appropiate directory tree

  if [ "$BRANCH" = "trunk" ]
      then
      VERSION=${BRANCH}_build_${BUILD}
  elif  [ "$BRANCH" = "branches" ]
      then
      VERSION=${VERSION}_build_${BUILD}
  fi
  
  PKG=${DIR}_${VERSION}_${ARCH}   # Package name includes build number
  
# Generates a dynamic control file because the version must be retrieved from
# the subversion tags subtree.
  cat <<EOF > $DIR/DEBIAN/control
Package: ntoscript-$ARCH
Version: $VERSION
Architecture: i386
Maintainer: Rafael Gonzalez <astrorafael@yahoo.es>
Installed-Size: 1088
Depends: libc6 (>= 2.3.4), libmpeparser (>=3.0), zlib1g (>=1.2.3)
Section: non-free/interpreters
Priority: extra
Description: NTOScript. An INDI Scripting environment.
 NTOScript is an INDI scripting environment to control telescopes and
 related astronomical devices. This package contains the executable,
 sample scripts and a user's manual.
 Needs libmpeparser package available at
 ftp://public:@soton.mpeforth.com/libmpeparser_3.0b-3_i386.deb
 (or higher version)
EOF
  
  
# Populate directory tree with contents
  cp ntoscript.$ARCH  $DIR/usr/bin/ntoscript
  
# Make Debian package
  dpkg -b  $DIR $PKG.deb
  
# Make raw binary package
  cd $DIR ; tar cvf $PKG.tar usr/* ; mv $PKG.tar .. ; cd ..
  tar uvf $PKG.tar INSTALL
  gzip  $PKG.tar

# When generating two binaries we make two builds ! 
# i686 comes first and then i386
  BUILD=`expr $BUILD + 1` 
done
