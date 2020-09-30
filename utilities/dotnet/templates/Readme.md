# GreenEnergyHub dotnet templates

Right now GreenEnergyHub offers two templates:

- [GreenEnergyHub.TemplateSolution](#greenenergyhubtemplatesolution) (gehsln), used for creating a solution with the preferred project structure, stylecop and settings in place.
- [GreenEnergyHub.TemplateFunction](#greenenergyhubtemplatefunction) (gehfunc), used for creating an Azure Function with the preferred settings as the solution.

## GreenEnergyHub.TemplateSolution

To create a new solution in the GreenEnergyHub, you must use this template.

### Installation of GreenEnergyHub.TemplateSolution

Right know you can install it from your local machine.

- Open your Terminal.
- Navigate to the `GreenEnergyHub/Utilities/dotnet/Templates` folder.
- Use the following command `dotnet new -i GreenEnergyHub.TemplateSolution/`. For more help, follow [this](https://docs.microsoft.com/en-us/dotnet/core/tools/custom-templates#installing-a-template) link.
- Check if the template is installed correctly by checking the list of templates `dotnet new --list`.
- In your Terminal, navigate to the path you want to create the solution in.
- Create the solution folder, example: `GreenEnergyHub.ExampleSolution/`.
- To generate a new solution run this command: `dotnet new gehsln -n ExampleSolution`.

### Architecture

This solution will create the following folders.

#### Hosts (Solution folder, created by TemplateFunction)

Only code specific for each single host. If multiple hosts share logic, extract that to a appropriate layer or a common assembly in the hosts folder - if it's only valid for such hosts.

#### Application (Project)

Application specific code, think use cases. Application services orchestrates the system and should also hold domain logic. This is also where commands and queries would live, if that approach is taken.
Any external/infrastructure needs/contracts are defined here.

#### Domain (Project)

This is purely domain POCO classes.

#### Infrastructure (Project)

Covers all the external/infrastructure needs/contract implementations.

#### Tests (Project/Solution folder)

Unit tests. If the need for integration tests arise, they should be in their own assembly. Benchmark projects would also live here.

## GreenEnergyHub.TemplateFunction

To create a new azure function project in the GreenEnergyHub, you must use this template.

### Installation of GreenEnergyHub.TemplateFunction

Right know you can install it from your local machine.

- Open your Terminal.
- Navigate to the `GreenEnergyHub/Utilities/dotnet/Templates` folder.
- Use the following command  `dotnet new -i GreenEnergyHub.TemplateFunction/`. For more help, follow [this](https://docs.microsoft.com/en-us/dotnet/core/tools/custom-templates#installing-a-template) link.
- Check if the template is installed correctly by checking the list of templates `dotnet new --list`.
- In your Terminal, navigate to the path of the solution in which you want to create the project in.
- To generate a new project in this solution run this command: `dotnet new gehfunc --sln ExampleSolution -n ExampleProject`
- At the moment there's a manual step, from the IDE you'll have to add existing project and select the newly created project, see [known issues](#known-issues).
- To add the project use the following command: `dotnet sln GreenEnergyHub.ExampleSolution.sln add -s Hosts source/GreenEnergyHub.ExampleSolution.ExampleProject/`.

### Known issues

When running the Azure Function template, you'll have to add it to the solution manually, see this [issue](https://github.com/dotnet/templating/issues/1991)

## Maintenance

Example for a runnable project can be found in the [documentation](https://github.com/dotnet/templating/wiki/Runnable-Project-Templates).
