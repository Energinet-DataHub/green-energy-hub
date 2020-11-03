# StyleCop

To enforce a common coding style and give quick feedback to developers directly in their editor, all VS projects must be configured to use StyleCop and the rules we have defined.

Some settings like indention has overlaps with settings in EditorConfig and must be kept in sync. The difference between these two techniques is that VS automatically correct text according to EditorConfig settings while StyleCop Analyzers just create warnings for broken rules and doesn't automatically fix anything.

## Configuration

StyleCop's behaviour is configured by two files:

- **.editorconfig**; the code analysis rules set file determines which rules are enabled/disabled and their severity level when enabled.
- **stylecop.json**; this file is used to fine/tune the behaviour of certain rules.

We only specify settings in the stylecop.json file when they are different from their default values. For more information see [Getting Started with stylecop.json](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/Configuration.md#getting-started-with-stylecopjson).

We enable StyleCop via the .editorconfig and Directory.Build.props files in the root of the project.

Relevant links:

- [StyleCop Analyzers on GitHub](https://github.com/DotNetAnalyzers/StyleCopAnalyzers)
- [Configuring StyleCop Analyzers](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/Configuration.md#getting-started-with-stylecopjson)
