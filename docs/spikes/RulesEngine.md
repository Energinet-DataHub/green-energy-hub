# Rules Engine Library

This document outlines the design decision around the choice of a rules engine library for the project.

## Problem Statement

As meter readings and other requests are received by the Green Energy Hub, certain validations will need to be performed on the input data before accepting it for further processing.

Some of these validations are static and act on the data shape or values, for example validating that input fields are of the proper data type and or that the values are within range. Other validations require calls to external systems or to enrich the incoming message first, such as checking that a customer ID is valid or that the customer has no outstanding balance/amount owing.

Each of these validations can be treated as an arbitrary rule, and an engine could apply these rules in parallel, collecting any validation errors and returning them to the caller.

Rule-based execution against data is a well-known pattern, and not something that should be re-invented here.

## Criteria

The criteria used for evaluation of the available rules engines are:

1. Rules can be run in a specified order
2. Supports inter-rule dependencies (rule A executes only if rule B succeeded/failed)
3. Rules can accept additional arguments or context data
4. Rules can make calls to external code (e.g., to retrieve data from SQL or perform arbitrary logic using another library)
5. Rules can serialized to an external data store (e.g. as JSON or SQL)
6. Rules can produce an output state to be marked as successful/failed

Additionally, the following special considerations are made for the rules engines projects:

1. Project licensing
2. Support and maintenance of the repository

## Approaches

We considered a few different approaches to solving this problem.

### RulesEngine (MSRE)

[RulesEngine](https://github.com/microsoft/RulesEngine) is an open-source library distributed as a NuGet package and maintained by Microsoft under an MIT License. It aims to abstract business logic and any relevant rules/policies out of the core system - rules and relevant metadata are stored in a JSON object conforming to a [schema](https://github.com/microsoft/RulesEngine/blob/master/schema/workflowRules-schema.json) and then passed in to initialize the rules engine, where rules are mainly comprised of lambda expressions limited to the `System` namespace.

RulesEngine was first released around one year ago, and continues to be actively maintained and added to by its core team of contributors, though documentation and code samples are fairly limited apart from the [Getting Started Guide](https://github.com/microsoft/RulesEngine/wiki/Getting-Started#getting-started-with-rules-engine) and the provided [demo app](https://github.com/microsoft/RulesEngine/tree/master/demo). All community interaction with the project and main contributors is done via [GitHub Issues](https://github.com/microsoft/RulesEngine/issues).

Evaluating it against the criteria defined above:

| Concern | Has Feature | Summary |
|---|---|---|
| 1. Ordering | ✅ | Rules execute in the order provided in the schema |
| 2. Inter-rule dependencies | ⚠️ | Limited. Rule nesting is [supported](https://github.com/microsoft/RulesEngine/blob/af53705ab527f3b9502205cbdb9703f65c2980b5/demo/DemoApp/Workflows/Discount.json#L37-L87), but implementation of forward chaining/inter-rule dependencies is currently in [PR](https://github.com/microsoft/RulesEngine/pull/56)  |
| 3. Context data | ⚠️ | Not natively supported, these would need to be separately inserted into the schema |
| 4. External calls | ✅ | Yes, but requires external code be [registered first](https://github.com/microsoft/RulesEngine/wiki/Getting-Started#resettings) in a custom class |
| 5. Serialized rules | ✅ | As JSON |
| 6. Produces output | ✅ | Rule engine call produces state of every rule |

More details:

1. Rules execute in the order provided in the schema. The [default](https://github.com/microsoft/RulesEngine/wiki/Getting-Started#successfailure) success/failure behavior of RulesEngine is slightly nonintuitive - it returns success for the first rule encountered in execution order, and failure only if all rules fail (not, as we might want, failure if even one rule fails). One option around this would be for us to write our own methods to parse the output [`RuleResultTree`](https://github.com/microsoft/RulesEngine/blob/master/src/RulesEngine/RulesEngine/Models/RuleResultTree.cs) and customize success/failure behavior (see [default implementation](https://github.com/microsoft/RulesEngine/blob/c0488f111310578ec5cecb9f0246b8fd5dcb0947/src/RulesEngine/RulesEngine/Extensions/ListofRuleResultTreeExtension.cs) as a potential starting point).

2. Rule nesting is natively supported - here, it seems to allow for several rules to exist on the same level and evaluated together based on a logical operator (see [here](https://github.com/microsoft/RulesEngine/blob/af53705ab527f3b9502205cbdb9703f65c2980b5/demo/DemoApp/Workflows/Discount.json#L37-L87) for a sample rules schema, though this behavior isn't thoroughly documented).

    However, the lack of forward chaining is a [known issue](https://github.com/microsoft/RulesEngine/issues/23), and there is currently a [PR](https://github.com/microsoft/RulesEngine/pull/56) out to add support for this feature to the RulesEngine library.

3. The documentation seems to suggest that passing in arguments to rules would require directly updating the rules schema input into the engine.

    Note: there is a `localParams` property of the [Rule class](https://github.com/microsoft/RulesEngine/blob/c0488f111310578ec5cecb9f0246b8fd5dcb0947/src/RulesEngine/RulesEngine/Models/Rule.cs), but its main purpose is to act as a [logical intermediate parameter](https://github.com/microsoft/RulesEngine/wiki/Getting-Started#localparams) to simplify the final rule expression, not a way to pass in any context data.

4. Calling outside libraries within rules is [supported](https://github.com/microsoft/RulesEngine/wiki/Getting-Started#resettings) by creating a custom helper class utilizing outside libraries, which can then be registered and passed in to initialize the engine.

5. There is a high degree of flexibility in where rules are stored, since the engine only requires that a valid list of rules conforming to the schema is passed in - our sample code currently reads a .json file from Azure Blob Storage, but is extensible.

6. As discussed in (1) above, each call to evaluate a provided input against the provided set of rules results in a [`RuleResultTree`](https://github.com/microsoft/RulesEngine/blob/master/src/RulesEngine/RulesEngine/Models/RuleResultTree.cs).

### NRules

[NRules](https://github.com/NRules/NRules) is an open-source library distributed as a NuGet package and maintained by Sergiy Nikolayev under an MIT License. It uses the Rete matching algorithm to match rules to 'facts' (input data) inserted into the engine. Rules are re-evaluated every time a new fact is inserted, and sometimes when existing fact is updated.

NRules has had regular releases spanning several years and has several contributors, but remains maintained primarily by the sole author. The [NRules website](http://nrules.net/) has links to available documentation, including a wiki, API docs, discussion groups that the author actively participates in.

Evaluating it against the criteria defined above:

| Concern | Has Feature | Notes |
|---|---|---|
| 1. Ordering | ✅ | *Matched* rules are executed according to relative priority. Matching process occurs automatically and rule matching cannot be re-ordered. |
| 2. Inter-rule dependencies | ✅ | Yes, [forward chaining](https://github.com/NRules/NRules/wiki/Forward-Chaining) using intermediate facts |
| 3. Context data | ✅ | Yes, but the context data must be inserted as a fact |
| 4. External calls | ✅ | Yes, matched rules can execute arbitrary code |
| 5. Serialized rules | ⚠️ | No native support for serialization. [Rule#](https://github.com/NRules/NRules.Language) is experimental. Rules can be [built at runtime](https://github.com/NRules/NRules/wiki/Rule-Builder) if we implement our own serialization. |
| 6. Produces output | ⚠️ | No native support. Rules can insert facts or mutate state of another object to store results. |

More details:

1. As facts are inserted or updated, a list of rules to execute is generated automatically based on their matching criteria. The matched rules then execute in the order of their relative assigned priorities.
2. Although inter-rule dependencies are not explicitly supported, rules can yield facts back into the engine which could cause other rules to trigger if they depend on the intermediate fact. Essentially, by convention rules need to yield their execution result as a fact which other rules take a dependency on. See discussions [here](https://stackoverflow.com/a/32598385/1389817) and [here](https://stackoverflow.com/q/49946688/1389817) for details.
3. To support additional context in matching criteria, the context must be made available in the rule engine as a fact first. A rule can then bind to the fact and match against its properties.
4. External calls in rule actions are supported easily, as all rules actions are just arbitrary code.
5. No native serialization is supported, but building rules at runtime is possible so a custom implementation is possible with additional effort. If a schema were to be created, it could be parsed and corresponding rules built at runtime.
6. Rules actions are simply C# code and do not produce any result, unless a fact is yielded back into the rules engine. By convention, a `RuleResult` object should be yielded by each rule so that the collection of `RuleResult` facts can be queried out of the engine after rule execution.

    Effectively, by convention rules generate facts and facts of that type are used to store result sets and to facilitate inter-rule dependencies.

    Note that this also means that all facts must be cleared from the engine between input evaluations. See [this discussion](https://gitter.im/NRules/NRules?at=5ae7cd0cdea1b95c10edae6a) for more details.

### Writing Our Own

Worth mentioning is the fact that, if we chose to, we could write our own rules engine and use that as part of the Green Energy Hub. This would take significant engineering effort, and maintaining the system moving forward would fall under the purview of the project maintainers.

Given these considerations and the strength of the other candidate solutions, this option was not explored further.

## Recommendation

Ultimately, we recommend NRules over MSRE.

For us, the main strengths of MSRE were the simplicity of the lambda expressions used to construct its rules, as well as the fact that it has native support for storing rules externally.
However, MSRE was first released on GitHub one year ago, and from our communication with the Microsoft product group maintaining it, it was initially built for an internal use case, not for OSS use.
As such, support for broader features such as forward chaining, etc. are currently in development without a clear release schedule or documentation of future ongoing library maintenance.
In addition, its documentation and sample code is less robust.

Overall, as developers, we found creating rules and generally working with NRules to be an intuitive experience.
There are two criteria out of the ones we established above that are not natively supported by NRules: (1) storing rules externally and (2) producing output - however, we feel that both those concerns have been adequately addressed.

* Re: (1), our joint discussions over the last month have shown that, currently, there is no strong business need for storing rules externally - since there is most likely not a scenario where we would want a new rule or change to an existing rule to immediately go into effect without a broader review, it makes sense to store rules within the codebase.
If the business need to store rules in some external store does arise, some custom implementation to ensure conversion to the correct rule format would need to be built.
* Re: (2), as discussed above, our sample test harness includes a simple `RuleResult` object that captures the output of the NRules execution, which can be extended, if necessary, for different use cases.
After rule execution, one can query the rule engine for these `RuleResult` objects, which can be directly associated to the triggering message and include an explanation of why a validation failed.

The other features that NRules offers are well-suited for this application - the ability to take in context data and make calls to external libraries as part of a rule's execution would both be useful for Green Energy Hub.
While there currently isn't a business need for inter-rule dependencies, the existing support for for forward chaining in NRules allows for future extension if the need ever comes up for a market actor.

With NRules, we can group rules into collections that we define as a `RuleSet` and if necessary enforce an inheritance relationship (ex. relationship between a default `RuleSet` for Europe and a country-specific implementation which adds some country-specific rules) through a union of different sets of rules.

From an open-source maintenance standpoint, NRules has been around for over a large part of the last decade, with high engagement in the OSS community, robust documentation, and active support by its maintainer.
The release schedule has slowed in recent years as the library has reached a relatively stable state, but the main contributor has said that he intends to release the newest version by the end of this year.
