# Instructions for local .editorconfig

As a developer, you're free to set up your own `.editorconfig` for your IDE - we have provided a `.editorconfig.ci`, which is used as a part of the build pipeline.
In order to use these settings or to make changes to this configuration for local development, copy this file to a new file called `.editorconfig`, and StyleCop will automatically use those settings when running `dotnet build` locally.
Note that all code will still have to comply with the settings in `.editorconfig.ci` by the time that you make a PR in order to enter the `main` branch of the repository.

There's a bash script the help automate this.

- Run `.\init-editorconfig.sh` to use the same `.editorconfig.ci` as your local `.editorconfig`.
- Run `.\init-editorconfig.sh loose` to overwrite `warning` and `errors` with `suggestion` severity.
