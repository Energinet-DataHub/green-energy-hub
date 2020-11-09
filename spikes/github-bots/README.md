# Introduction

In managing a large GitHub repository, developers want issues to be labeled and tracked clearly, so that work discovered does not fall through the cracks.
To this end, we should automate common tasks, and build ceremonies around processes which we cannot automate.
This spike serves as a list of the GitHub bots considered, and the ultimate decision as to which bots were accepted into the repository.

## Accepted

The following bots are accepted and will be added to the repository.

### [protobot-stale](https://github.com/probot/stale)

This bot will prevent stale issues and pull requests from piling up.
Configuration will be done through [.github/stale.yml](../../.github/stale.yml).

### [dependabot](https://dependabot.com/)

This bot will automatically make pull requests to update dependencies.
Configuration will be done through [.github/dependabot.yml](../../.github/dependabot.yml).

## Rejected

The following bots were considered and rejected, for the reason(s) provided.

### [msftbot](https://github.com/probot/stale)

While Microsoft is collaborating on this project, the intent is for the project to be able to live anywhere.
Therefore, while msftbot was considered, ultimately it will not work for the project, as we want to be able to port this to another organization.
