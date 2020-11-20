# Running License Check Locally

A license check is being run for each Pull Request, in case there were changes to or additions of any code file in the repository.
You can run this check also locally, thanks to [deno license checker](https://github.com/kt3k/deno_license_checker) tooling.

To invoke the license check, [install deno](https://deno.land/) and run the following command:

```bash
deno run --unstable --allow-read https://deno.land/x/license_checker@v3.1.0/main.ts
```
