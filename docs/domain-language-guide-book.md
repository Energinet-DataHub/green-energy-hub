# Domain language guide book

## Why this document?

With a consistent naming through out the application, it is easier for developers to navigate in different domains. This guide will provide a description of concepts that can be applied in multiple domains, but it is not meant to be a single source of truth. Foundation for the guide is found in the publication [The Harmonised Electricity Role Model](https://eepublicdownloads.entsoe.eu/clean-documents/EDI/Library/HRM/Harmonised_Role_Model_2020-01.pdf) from [ENTSO-E](https://www.entsoe.eu/). `The harmonised  electricity role model` provides a starting point from where the different concepts can evolve.

## Concepts

Concepts in this context is general constructs that can be applied across multiple domains. Each concept is described and listed with sample properties of what is expected to be bound to each concept.

A domain is not mandated to use everything from a single concept if it is not applicable. Eg. if a concept has a notion of `StreetNames` but it's not needed, then it is not expected to be implemented - *only use what is needed*.

## Suffix on common properties

Properties that are commonly used across many entities is expected to be easy identified.

### Date suffix

This suffix is used when the property represents a point in time. It is not restricted to calendar date, but could also contain a time part.

Example:

``` csharp
// C#
public class AccountingPoint 
{
    public Instant CreatedDate { get; set; }
}
```

### ID suffix

A property with this suffix denotes a value that identifies an entity. This should not be interpreted as database record id or any other storage identification.

Example:

``` csharp
// C#
public class AccountingPoint
{
    public string AccountingPointID { get; private set; }
}
```
