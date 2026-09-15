# Parley.NET

[![Website](https://img.shields.io/badge/Website-parleymaf.dev-orange)](https://parley-maf.dev)
[![License](https://img.shields.io/badge/License-MIT-orange)](LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Parley)](https://www.nuget.org/packages/Parley/)
[![Website](https://img.shields.io/badge/BuyMeACoffee-orange?logo=buymeacoffee)](https://buymeacoffee.com/sarefactor)

<p align="center">
  <a href="https://parley-maf.dev">
    <img width="3840" height="1990" alt="image" src="https://github.com/user-attachments/assets/86c893a1-ef32-48a5-9bee-6ed7fc5d181a" />
  </a>
</p>

<p align="center">
  <a href="http://parley-maf.dev"><strong>Documentation</strong></a> ·
  <a href="http://parley-maf.dev/documentation/v0/introduction"><strong>Getting Started</strong></a> ·
  <a href="https://github.com/Sarefactor/parley-waterworks"><strong>Project Template</strong></a> ·
  <a href="https://github.com/Sarefactor/parley-fe"><strong>Front-End Project</strong></a>
</p>

## What is Parley?

Parley is a .Net library built ontop of the Microsoft Agent Framework that enables the creation of workflows with a drag-and-drop editor.

It is designed to be extensible so developers can easily add specific fuctionality for their usecases to the workflows that they build.

Typical agent setups are driven by LLMs that are provided with access to a set of tools which are invoked dynamically by the LLM based on the conversation context. Workflows offer a deterministic structure when you need an agent or process to follow a specific flow. As Parley is build ontop of the Microsoft Agent Framework all the documentation on [MAF Workflows](https://learn.microsoft.com/en-us/agent-framework/workflows/) is applicable.

While modern LLM's are incredibly capable the token costs for the higher tier models are not cheap. Parley is intended to help with the construction of agents that can accomplish the same end goals but at a significantly lower cost by blending the flexibility of LLMs with deterministic workflows.

## Getting Started

Setting up Parley for both new and existing projects requires first installing the nuget package before following the steps below starting with the Program.cs file of your application.

```
var builder = WebApplication.CreateBuilder(args);

ParleyConfiguration.ConfigureParley(builder.Services,
                                    builder.Configuration,
                                    useDefaultMongoDb: true);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

await ParleyConfiguration.PreloadNodes(app);
```

Pass in IServiceCollection and IConfiguration to the ConfigureParley method and decide if you want to use the default MongoDb implementation that comes built in. If you do not wish to use it set useDefaultMongoDb to false but you will need to provide your own implementations of [IAgentSchemaRepository](https://github.com/Sarefactor/parley/blob/main/Parley/Core/DataAccess/Repositories/IAgentSchemaRepository.cs) and [IAgentConfigurationRepository](https://github.com/Sarefactor/parley/blob/main/Parley/Core/DataAccess/Repositories/IAgentConfigurationRepository.cs).

Following the above is the PreloadNodes method which is responsible for scanning the application assemblies for all inbuilt and custom nodes/validators marked with the ParleyNode and ParleyNodeValidator attributes.

### Application Settings

Next ensure that your appsettings are configured, the values below will need to be provided.

```
"Parley": {
  "AgentProvider": 1,
  "ProviderConfig": {
    "ApiKey": "",
    "Model": ""
  },
  "Databases": {
    "MongoDb": {
      "ConnectionString": "",
      "Database": "",
      "AgentSchemaCollection": "",
      "WorkflowCollection": "",
      "ConfigurationCollection": ""
    }
  }
}
```

```
public enum AgentProviderType
{
    None,
    OpenAi,
    Anthropic
}
```

Currently Parley supports OpenAi and Anthropic for it's LLM integrations, other providers will be added later.

### Databases

If using the default implementation of MongoDb ensure you set a connection string, database, and values for the Agent Schema, Workflow, and Configuration collections. The Configuration collection requires a document with the following properties to be created:

```
{
  "configurationId": "parley:configuration",
  "activeSchemaId": null
}
```

### Typegen

Parley also includes the Typegen package which is used to generate Typescript files when the project is built. This is useful for converting C# dtos that also need to be consumed by the front end.

There are a few steps you will need to take in order to make use of Typegen.

The first is to make sure the TypeGen CLI tool is installed, this can be done via the terminal in your project with:

```
dotnet tool install --local dotnet-typegen --version 7.0.0
```

Next include a typegen.json file in your project root directory:

```
{
  "assemblies": [ "bin/Debug/net10.0/Parley.dll" ],
  "outputPath": "bin/Debug/net10.0/parleyts"
}
```

Important! You must include any assemblies which hold custom parley nodes in the above json inside the assemblies list for Typegen to include them in its output.

Next you will need to add the following to your project file:

```
<Target Name="RunTypeGen" AfterTargets="Build" Condition="'$(Configuration)' == 'Debug'">
  <Exec WorkingDirectory="$(ProjectDir)" Command="dotnet-typegen generate -c  typegen.json" />
</Target>
```

After doing the above you will now be able to mark any DTOs you create for your custom nodes with [ExportTsClass] and they will be included in the output that Typegen produces. You can see an example of this in the [AgentSchemaDto](https://github.com/Sarefactor/parley/blob/main/Parley/Dtos/Schema/AgentSchemaDto.cs) class.

## Documentation

After completing the installation steps please see the documentation on [workflows](https://parley-maf.dev/documentation/v0/workflows) for how to get started on using Parley. Also, see [Custom Nodes](https://parley-maf.dev/documentation/v0/custom-nodes) for details on how to add your own project specific nodes to Parleys workflows.

## So It Begins

At the time of writing (Sept 10th 2026) Parley is in a very, very early stage so lots of things are sort of half working, others are half baked, but the general idea and foundations are in a place where I am happy for people to start using the project and seeing how far they get with it.

It is the first time I've built and released something like this so any feedback is really welcome, particularly around the following:

- Does the documentation make sense/is it easy to follow and are there any gaps in it?
- Bugs/issues, at this early stage I have a pile of them to work on.
- Were you able to add custom nodes and have them work easily enough?

Any contributions and/or feature requests are more than welcome.

All I ask is that if you do make anything cool using Parley to please share it! I would be over the moon if even just one person uses it to build something.

If you do find Parley useful please consider paying a visit to [Buy Me A Coffee](https://buymeacoffee.com/sarefactor).
