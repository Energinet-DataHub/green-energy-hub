# ADR0000 Use Markdown Architectural Decision Records

* Status: accepted
* Deciders: @renetnielsen, @MrDach, @kft, @sondergaard, @erduna, @Korgath, @Christian-Meyer, @cmgrd
* Date: 2021-01-26

## Context and Problem Statement

We want to record architectural decisions made in this project.
Which format and structure should these records follow?

## Decision Outcome

Chosen option: "MADR 2.1.2", because

* Implicit assumptions should be made explicit.
  Design documentation is important to enable people understanding the decisions later on.
  See also [A rational design process: How and why to fake it](https://doi.org/10.1109/TSE.1986.6312940).
* The MADR format is lean and fits our development style.
* The MADR structure is comprehensible and facilitates usage & maintenance.
* The MADR project is vivid.
* Version 2.1.2 is the latest one available when starting to document ADRs.

## Considered Options

* [MADR](https://github.com/adr/madr) - The Markdown Architectural Decision Records.
* [Michael Nygard's template](http://thinkrelevance.com/blog/2011/11/15/documenting-architecture-decisions) – The first incarnation of the term "ADR"
* [Sustainable Architectural Decisions](https://www.infoq.com/articles/sustainable-architectural-design-decisions) – The Y-Statements
* Other templates listed at <https://github.com/joelparkerhenderson/architecture_decision_record>
* Formless – No conventions for file format and structure
