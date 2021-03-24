# Domain language guide book

## Why this document?

With a consistent naming through out the application, it is easier for developers to navigate in different domains. This guide will provide a description of concepts that can be applied in multiple domains, but it is not meant to be a single source of truth. Foundation for the guide is found in the publication [The Harmonised Electricity Role Model](https://eepublicdownloads.entsoe.eu/clean-documents/EDI/Library/HRM/Harmonised_Role_Model_2020-01.pdf) from [ENTSO-E](https://www.entsoe.eu/). `The harmonised  electricity role model` provides a starting point from where the different concepts can evolve.

### Contributions to domain language

The content in this document is owned by everyone working with Green-Energy-Hub. If a domain has evolved a language concept that can be used in more general terms, please open a pull request and propose the change to this document.

## Concepts

Concepts in this context is general construct that can be applied across multiple domains. Each concept is described and listed with sample attributes of what is expected to be bound to each concept.

A domain is not mandated to use everything from a single concept if it is not applicable. E.g. if a concept has a notion of `StreetName` but it's not needed within a domain, then it is not expected to be implemented - *only use what is needed*.

### Actor

*Description:* An actor is interacting with the system. The interaction can be directly or via a third party.

Attributes related to an actor:

- Name

### Communication

*Description:* Defines means of communication among parties. This could be data recorded on a party for how to get in touch with customer support, contact details for a consumer etc.

Attributes related to communication:

- Email
- Phone
- Fax

### Physical location

*Description:* A physical location is as the name applies, a location that is bound to a physical location e.g. `building, street name, gps-coordinates`.

Attributes related to a physical location:

- BuildingNumber
- CityName
- CitySubDivisionName
- CountryCode
- CountryName
- FloorNumber
- GpsCoordinate
- PostalCode
- StreetCode
- StreetNameX (X is to be substituted with a consecutively number e.g. StreetName1, StreetName2, StreetName3 etc.)
- SuiteNumber

## Definitions extracted from the harmonised electricity role model

This section is meant as inspiration when naming objects and properties in domains. This is not a complete list, for all details referrer to the PDF. [Full description](https://eepublicdownloads.entsoe.eu/clean-documents/EDI/Library/HRM/Harmonised_Role_Model_2020-01.pdf).

| Type | Name | Description |
| :---: | --- | ----------- |
| Entity | Accounting Point | A domain under balance responsibility where Energy Supplier change can take place and for which commercial business processes are defined. |
| Entity | Bidding Zone | The largest geographical area within which market participants are able to exchange energy without capacity allocation. |
| Entity | Bidding Zone Border | Defines the aggregated connection capacity between two Bidding Zones. A market area (Which defines the aggregated connection capacity between two Bidding Zones) where the transmission capacity between the Bidding Zones is given to the Balance Responsible Parties according to rules carried out by a Transmission Capacity Allocator. Trade between Bidding Zones is carried out on a bilateral or unilateral basis. |
| Entity | Capacity Calculation Region | The Capacity Calculation Region is the geographic area in which coordinated capacity calculation is applied. |
| Entity | Exchange Point | A domain for establishing energy exchange between two Metering Grid Areas. **Additional information:** This is a type of Metering Point. |
| Entity | Metering Grid Area | A Metering Grid Area is a physical area where consumption, production and exchange can be measured. It is delimited by the placement of meters for continuous measurement for input to, and withdrawal from the area. **Additional information:** It can be used to establish volumes that cannot be measured such as network losses. |
| Entity | Metering Point | An entity where energy products are measured or computed. |
| Role | Balance Responsible Party | A Balance Responsible Party is responsible for its imbalances, meaning the difference between the energy volume physically injected to or withdrawn from the system and the final nominated energy volume, including any imbalance adjustment within a given imbalance settlement period. **Additional information:** Responsibility for imbalances (Balance responsibility) requires a contract proving financial security with the Imbalance Settlement Responsible of the Scheduling Area entitling the party to operate in the market. |
| Role | Billing Agent | The party responsible for invoicing a concerned party. |
| Role | Capacity Trader | A party that has a contract to participate in the Capacity Market to acquire capacity through a Transmission Capacity Allocator. |
| Role | Consumer | A party that consumes electricity. |
| Role | Consumption Responsible Party | A Consumption Responsible Party is responsible for its imbalances, meaning the difference between the energy volume physically withdrawn from the system and the final nominated energy volume, including any imbalance adjustment within a given imbalance settlement period. **Additional information:** This is a type of Balance Responsible Party. |
| Role | Consent Administrator | A party responsible for administrating a register of consents for a domain. The Consent Administrator makes this information available on request for entitled parties in the sector. |
| Role | Data Provider | A party that has a mandate to provide information to other parties in the energy market. |
| Role | Energy Service Company (ESCO) | A party offering energy-related services to the Party Connected to Grid, but not directly active in the energy value chain or the physical infrastructure itself. The ESCO may provide insight services as well as energy management services. |
| Role | Energy Supplier | An Energy Supplier supplies electricity to or takes electricity from a Party Connected to the Grid at an Accounting Point. **Additional information:** An Accounting Point can only have one Energy Supplier. When additional suppliers are needed the Energy Supplier delivers/takes the difference between established (e.g. measured production/consumption and contracts with other suppliers. |
| Role | Energy Trader | A party that is selling or buying energy. |
| Role | Grid Access Provider | A party responsible for providing access to the grid through an Accounting Point for energy consumption or production by the Party Connected to the Grid. The Grid Access Provider is also responsible for creating and terminating Accounting Points. |
| Role | Imbalance Settlement Responsible | A party that is responsible for settlement of the difference between the contracted quantities with physical delivery and the established quantities of energy products for the Balance Responsible Parties in a Scheduling Area. **Note:** The Imbalance Settlement Responsible may delegate the invoicing responsibility to a more generic role such as a Billing Agent. |
| Role | Market Information Aggregator | A party that provides market related information that has been compiled from the figures supplied by different actors in the market. This information may also be published or distributed for general use. **Note:** The Market Information Aggregator may receive information from any market participant that is relevant for publication or distribution. |
| Role | Meter Administrator | A party responsible for keeping a database of meters. |
| Role | Meter Operator | A party responsible for installing, maintaining, testing, certifying and decommissioning physical meters. |
| Role | Metered Data Administrator | A party responsible for storing and distributing validated measured data. |
| Role | Metered Data Aggregator | A party responsible for the establishment and qualification of measured data from the Metered Data Responsible. This data is aggregated according to a defined set of market rules. |
| Role | Metered Data Collector | A party responsible for meter reading and quality control of the reading. |
| Role | Metered Data Responsible | A party responsible for the establishment and validation of measured data based on the collected data received from the Metered Data Collector. The party is responsible for the history of metered data for a Metering Point. |
| Role | Metering Point Administrator | A party responsible for administrating and making available the Metering Point characteristics, including registering the parties linked to the Metering Point. |
| Role | Party Administrator | A party responsible for maintaining party characteristics for the energy sector. |
| Role | Party Connected to the Grid | A party that contracts for the right to consume or produce electricity at an Accounting Point. |
| Role | Producer | A party that generates electricity. **Additional information:** This i a type of Party Connected to the Grid. |
| Role | Production Responsible Party | A Production Responsible Party is responsible for its imbalances, meaning the difference between the energy volume physically injected to the system and the final nominated energy volume, including any imbalance adjustment within a given imbalance settlement period. **Additional information:** This is a type of Balance Responsible Party. |
| Role | Reconciliation Accountable | A party that is financially accountable for the reconciled volume of energy products for a profiled Accounting Point. |
| Role | Reconciliation Responsible | A party that is responsible for reconciling, within a Metering Grid Area, the volumes used in the imbalance settlement process for profiled Accounting Points and the actual measured quantities. **Note:** The Reconciliation Responsible may delegate the invoicing responsibility to a more generic role such as a Billing Agent. |
| Role | System Operator | A party responsible for operating, ensuring the maintenance of and, if necessary, developing the system in a given area and, where applicable, its interconnections with other systems, and for ensuring the long-term ability of the system to meet reasonable demands for the distribution or transmission of electricity. |

## Suffix on common properties

Properties that share a common trait, is expected to be easy identified. For a reader of the source code, it should be easy to identify the intent of the property.

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
