# Contributing

Contributions to this project are welcome. You can participate in various ways.

## Writing SettingServices
If you implemented a setting service backend, you can add it to the list on the readme.md page. Just create a pull request to the `master` branch with your endpoint added.
To prevent trigger an automatically release please include `[skip ci]` in your commit message or desscription.

## Reporting bugs and feature request
If you encounter a bug, please open a issue on this repository. Please provide a short reproduction snippet.
You may want to create a unit test, that tests for the bug and create a pull request for a new bugfix branch. Please see below for git flow guidelines.

## Solving bugs / implementing features
This project uses git flow for development. This means almost all development is done on the `develop` branch. You should however not directly commit to the `develop` branch.

## Features
A feature is some new code unit, that provides additional functionality. A refactor can also be considered as a feature, but it should be pointed out, that this feature could break other code.
If you are working on a feature, please create a branch `feature/xx` where `xx` is the number of the github issue, the feature corresponds to.
If a commit breaks functionality allready present in the develop branch
(you can change the behaviour and interface of your feature at any time, if it hasn`t been merged yet), please add `+semver: breaking` to the commit description.

When you're done or willing to share your work with others, you can create a pull request to the develop branch. If the HEAD commit of your pr contains `WIP` in the commit message or description,
you can indicate, the pr should not be merged now. In the description of the pr please explain what your feature does, and how it works. Please include a link to the original issue and summarize 
the results of that issue. So the pr contains a description how the new feature is actually to use.

Plase ensure the following criterias are met, before creating a non `WIP` pull request.
- [ ] Is the code documented (xml comments on all public, internal and protected members)
- [ ] Has the code unit test (if applicable)
- [ ] If the code contains breaking changes, are they described in the pull request decription and the release notes.md
- [ ] Was the changelog updated with the new feature?

## Bugs
Almost everything, that aplies to features also applies to bugs. Please use a `bugfix\xx` branch, created from the current master branch as a start. Your pr should target the master brach.
The master branch will be merged back to the develop branch, by me.