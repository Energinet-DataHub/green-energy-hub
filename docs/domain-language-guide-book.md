# Domain language guide book

To have a consistent naming through out the application, we provide guidance on naming of common concepts. The foundation for the list is found in the publication [The Harmonised Electricity Role Model](https://eepublicdownloads.entsoe.eu/clean-documents/EDI/Library/HRM/Harmonised_Role_Model_2020-01.pdf) from [ENTSO-E](https://www.entsoe.eu/).

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
public class AccountingPoint
{
    public string AccountingPointID { get; private set; }
}
```
