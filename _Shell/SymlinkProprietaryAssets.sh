#!/bin/sh
#
# Symlink contents of Proporietary _Prop* submodules into Assets directory,
# then .gitignore them to prevent proprietery assets from being
# accidentally included in our public syn3h-client repo.

# Symlink everything from the _Proprietary_* folder into our Assets folder
echo "Script executed from: ${PWD}"

ln -v -s _PropResources/Assets/* Assets/
ln -v -s _PropLibs/Assets/* Assets/

# Add everything from the _Proprietary_* folders to our root .gitignore as well
# Props:
# - https://stackoverflow.com/questions/8650871/telling-git-to-ignore-symlinks
# - https://www.cyberciti.biz/tips/handling-filenames-with-spaces-in-bash.html
# - https://stackoverflow.com/questions/3557037/appending-a-line-to-a-file-only-if-it-does-not-already-exist/28021305
SAVEIFS=$IFS
IFS=$'\n'
for f in $(find [Aa]ssets/* -maxdepth 1 -type l); do
	gitignore_line="/[Aa]ssets/${f:7}"
	grep -qxF "$gitignore_line" ./.gitignore || (echo "$gitignore_line" >>./.gitignore && echo "> $gitignore_line added to .gitignore")
done
IFS=$SAVEIFS

echo "Symlinks created. Gitignore updated. Mission accomplished."
