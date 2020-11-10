# Running Spell Check for Markdown Files Locally

Markdown document spell check is being run for each Pull Request, in case there were changes to any markdown document in the repository made.
You can run spell check also locally, thanks to [pyspelling](https://facelessuser.github.io/pyspelling/) tooling installed to dev container used for local development.

To invoke markdown spell check in dev container terminal window navigate to **/workspaces/green-energy-hub/utilities/md-spellcheck** directory and run following command:

```bash
pyspelling -c spellcheck.yml
```
