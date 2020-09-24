# StyleCop

To enforce a common coding style and give quick feedback to developers directly in their editor, all VS projects must be configured to use StyleCop and the rules we have defined.

Some settings like indention has overlaps with settings in editorconfig and must be kept in sync. The difference between these two techniques is that VS automatically correct text according to EditorConfig settings while StyleCop Analyzers just create warnings for broken rules and doesn't automatically fix anything.

## Configuration

StyleCop's behaviour is configured by two files:

- ***.ruleset**; the code analysis rules set file determines which rules are enabled/disabled and their severity level when enabled.
- **stylecop.json**; this file is used to fine/tune the behaviour of certain rules.

We only specify settings in the stylecop.json file when they are different from their default values. For more information see [Getting Started with stylecop.json](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/Configuration.md#getting-started-with-stylecopjson).

To enable StyleCop correctly on new VS projects add the StyleCop.Analyzers NuGet package and add the following to the *.csproj file.

```xml
  <PropertyGroup>
    <!-- Ensure breaking rules will fail build -->
    <StyleCopTreatErrorsAsWarnings>false</StyleCopTreatErrorsAsWarnings>
    <!-- Specify rules that configure the behaviour of StyleCop (see also https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/DOCUMENTATION.md) -->
    <CodeAnalysisRuleSet>..\ddp.ruleset</CodeAnalysisRuleSet>
  </PropertyGroup>

  <ItemGroup>
    <!-- 
    Additional settings for specific rules (e.g. SA1200 specify namespaces must be placed correctly, the json file then defines what "correctly" means) 
    See also https://github.com/[stylecop.json](/.attachments/stylecop-7bcfd8c3-0d51-4c58-8a4e-5a765bd414b4.json)[stylecop.json](/.attachments/stylecop-e24d6a9d-974b-47c7-9958-859f8fa6200e.json)DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/Configuration.md
    -->
    <AdditionalFiles Include="..\stylecop.json">
      <Link>stylecop.json</Link>
    </AdditionalFiles>
  </ItemGroup>
```

Relevant links:
- [StyleCop Analyzers on GitHub](https://github.com/DotNetAnalyzers/StyleCopAnalyzers)
- [Configuring StyleCop Analyzers](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/Configuration.md#getting-started-with-stylecopjson)
