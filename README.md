# COMP30019_Project_1

.gitignore ignores lots of big static unity files

.gitattributes tracks big files that don't need to be transferred back and forth and instead will force git to just check the diffs and update locally

## Setup guide

1. Install Git Large File Storage (LFS) module https://git-lfs.github.com/
2. Clone the entire repo into the target directory
3. In a bash terminal (git bash will work) enter `git lfs install` inside the new git repo to turn on LFS mode
4. Open a scene in `COMP30019_Project_1\Assets\Scenes\` and Unity will build the rest of the project
