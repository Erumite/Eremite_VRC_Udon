# From the .git directory for a project
# Useful for using the GitHub for Unity Project outside of GitHub (ie: Keybase)

# Uninstall the LFS hooks from .git/hooks/
git lfs uninstall

# Disable anything that is already in LFS - pretty slow for lots of files
git lfs migrate export --everything --include .

# Disable the LFS handler in .gitattributes
sed -i '/filter=lfs/d'.gitattributes

# Verify that there are no files in LFS
git lfs ls-files

# Probably need to force-push with the mess this has made: 
git push origin main --force
