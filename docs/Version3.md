
# DependenSee Version 3

I am planning an overhaul of the project for the next major release. Below is what I have in mind. Note that some of this will unfortunately be breaking changes.

If you like to follow the progress, checkout the [`/v3` branch](https://github.com/madushans/DependenSee/tree/v3).

If you have any questions, come say hi 👋 in [Discussions](https://github.com/madushans/DependenSee/discussions)

## Overall Design

DepedenSee will be organized into 3 parts as below.

### **DependenSee Core**

As Requested in [Issue #28](https://github.com/madushans/DependenSee/issues/28) the core functonality will be available as a Nuget package. This will be a new andseparate nuget package (as opposed to a nuget tool) which will allow any custom scenarios the existing CLI does not support and/or is beyond the scope of DependenSee.

This is not a replacement for the existing CLI. Though most of the functionality will be moved to the Core package, and the CLI will serve as a shell for the Core.

## DependenSee (CLI)

This will be the evolution of the existing CLI tool and will be available as it is, just with a 3.x version number. However the CLI will not be backwards compatible as the philosophy has changed.

## DependenSee Visualizer

Currently the most used part of the project is the html based visualizer. This was originally built as a bare bones html page, and not much though was put into it. 

However it has clearly shown to be useful, and requires an overhaul. Going forward this is planned to be a React app, though still self-contained in a single html file. Moving to React allows to make a better interface than what we have now. For ore information see [Visualizer](/docs/Visualizer.md)

## Changes to Philosophy

The v2 DependenSee has grown to have an overwhelming amount of CLI options which made it depart from the original "easy-to-use" motto. Most of this is due to requirements of filtering the dependency graph in different ways. 

With these switches, DependenSee can be instructed to customize the discovery run, by including or excluding projects, namespaces, and packages. However if you want a different filter, you have to run DependenSee again which means you have to have access to the source. This may not be possible if the html output was distributed to individuals, who may not have access to the source.

The new v3 is being updated to have a minimal amount of filtering on the CLI. Filtering is only present to get around any issues the discovery may encounter, and not as a means of customizing the output. Instead the discovery will always emit a complete (as possible) representation of your projects. This will include more information than what the current tool is collecting, like Solution files and  Solution folders.

Along with the upcoming new Visualizer, all the filtering are to be moved to the Visualizer frontend. This allows DependenSee to be run on a repository only once, collect all the information it sees, and let the Visualizer handle filtering, even without having the source at hand. Since all the information will be presented to the visualizer, it will be able to provide filtering on folders, projects, project names, packages names, solution files, and solution folders, all without having to run the CLI or having to have the source at hand.

Unfortunately as you may have guessed, this means breaking many of the existing CLI switches. Since we would still like to generate a visualization with some preconfigured filters, ideally we will be able to point DependenSee to a file that has a "default filter" which can be sent to the visualizer. I plan to make this file extractable from the visualizer as well. My plan is to allow the following workflow

- Generate a visualization
- Open visualizer, setup filters (and possibly customizations like colors .etc.)
- Extract the configuration form the visualizer and save as some JSON file.
- Run the CLI again, but additionally providing the path to the JSON file.
- CLI will generate a new visualization with the specified data as the default filter.

This visualization can be distributed or stored in documentation where it will show the filtered and customized output as default, but allowing to change the filter if necessary.

# Progress

This is still very much in the early stages, so stay tuned.

So far, I think I have the Core and the CLI work mostly done. (Checkout whats already in the [/v3 branch](https://github.com/madushans/DependenSee/tree/v3)). 

I'll move to start working on the Visualizer soon. (no timeline)

 If you have any questions, come say hi 👋 in [Discussions](https://github.com/madushans/DependenSee/discussions)


