# Green Energy Hub

![Logo](Logo_small.png)

Welcome to the Green Energy Hub, a project developed to support the change toward decarbonized economies.

- [General Info](#general-info)
    - [What is Green Energy Hub](#what-is-green-energy-hub)
    - [What does Green Energy Hub do](#what-does-green-energy-hub-do)
    - [What is a TSO](#what-is-a-tso)
- [Architecture](#architecture)
    - [Technical Stack Diagram](#technical-stack-diagram)

## General Info

### What is Green Energy Hub

Green Energy Hub provides the framework for building a centralized data management system facilitating the operation of electric grids and customer meter points.

### What does Green Energy Hub do

All information about a TSO's user's consumption, production, and exchange of electricity runs through the Green Energy Hub.
All information and processes performed are stored according to GDPR standards.
It also handles business processes such as relocations, change of energy supplier, etc.

Green Energy Hub handles all calculations of metered data. This is done based on the metered data received from the relevant market participants (eg. Energy supplier and grid operator). The calculations are then propagated to all the relevant market participants.

### What is a TSO

A Transmission System Operator (TSO) is an entity entrusted with transporting energy in the form of natural gas or electrical power on a national or regional level, using fixed infrastructure. The term is defined by the European Commission.

## Architecture

### Technical Stack Diagram

![Technical Stack Diagram](./docs/images/TechStack.png)

## Prepare dev environment

As a tool for local development you can use [Visual Studio Code](https://code.visualstudio.com/)

### DataBricks development

There is a CI gate for DataBricks code that runs unit tests and lint checks. Follow this articles to configure IDE to check and validate your code before pull request:

- [Configure VS Code to show lint issue](./docs/local-development/python-static-check-vscode.md)

### Markdown development

There is CI gate for Markdown that runs spell, lint and links check. The following articles will help you to check your markdown before pull request:

- [Run spellcheck across your code](./docs/local-development/markdown-spellcheck.md)
