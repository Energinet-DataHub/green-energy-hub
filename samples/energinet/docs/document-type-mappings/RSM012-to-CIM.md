# Mapping RSM-012 from ebIX to CIM

## Introduction

In Denmark the Market Actors communicate with DataHub using a set of RSM-messages (SOAP) that follow the industrial standard of [ebIX](https://www.ebix.org/). Once received by DataHub, a message will be converted to an internal format, a Data Transfer Object, which conforms to the naming convention of the CIM standard. When a message is sent from DataHub to a Market Actor this will yet again be following the ebIX standard.

## Motivation

The purpose of this document is create a single mapping reference for RSM-012 messages containing time series data for metering points.

## Mapping table

The mapping covers the RSM-012 message **DK_MeteredDataTimeSeries** and adheres to the general [rsm-to-cim.md](https://github.com/Energinet-DataHub/green-energy-hub/blob/main/samples/energinet/docs/document-type-mappings/rsm-to-cim.md) document.

> For information on how the time series data is stored in the TimeSeries Delta Lake, please visit our [Parquet schema definition for Time Series](https://github.com/Energinet-DataHub/green-energy-hub/wiki/Parquet-schema-for-Time-Series-Points)

### Generic message content

All RSM-messages share a generic set of attributes, e.g. _MessageReference_, _Sender/Recipient information_ and _DocumentType_ to mention a few.
The mapping below covers the generic set of attributes.

> Do note, that some of the CIM names are identical and will require a context to achieve uniqueness, hence a suggested context is provided for those mappings.

| **EbIX attribute**|**CIM name**| **Suggested "context" if needed to obtain CIM name uniqueness** | **CIM path** |
|:-|:-|:-|:-|
| MessageReference | No CIM name, use ebIX |||
| DocumentType | No CIM name, use ebIX |||
| MessageType | No CIM name, use ebIX |||
| Identification | mRID | MarketDocument | MarketDocument/mRID |
| DocumentType | Type | MarketDocument | MarketDocument/Type |
| Creation | CreatedDateTime | MarketDocument | MarketDocument/CreatedDateTime |
| Sender/Identification | mRID | SenderMarketParticipant | MarketDocument/Sender_MarketParticipant/mRID |
| Recipient/Identification | mRID | ReceiverMarketParticipant | MarketDocument/Receiver_MarketParticipant/mRID |
| EnergyBusinessProcess | ProcessType | MarketDocument | MarketDocument/Process/ProcessType |
| EnergyBusinessProcessRole (Sender) | Type | SenderMarketParticipant<br>or SenderMarketParticipant_(MarketRole) | Sender_MarketParticipant/MarketRole/Type |
| EnergyBusinessProcessRole (Recipient) | Type | ReceiverMarketParticipant<br>or ReceiverMarketParticipant_(MarketRole) | Receiver_MarketParticipant/MarketRole/Type |
| EnergyIndustryClassification | Kind | MarketServiceCategory | Market_ServiceCategory/Kind |

### DK_MeteredDataTimeSeries

| **EbIX attribute**|**CIM name**| **Suggested "context" if needed to obtain CIM name uniqueness** | **CIM path** |
|:-|:-|:-|:-|
| Identification | mRID | TimeSeries | TimeSeries/mRID |
| Function | Status | MktActivityRecord | MktActivityRecord/status |
| ResolutionDuration | Resolution | | TimeSeries/Period/Resolution |
| Start | Start | TimeInterval | Series/Period/TimeInterval/Start |
| End | End | TimeInterval | Series/Period/TimeInterval/End |
| IncludedProductCharacteristic/Identification | Product | | TimeSeries/Product |
| IncludedProductCharacteristic/UnitType | Name | QuantityMeasurementUnit | TimeSeries/Quantity_Measurement_Unit/name |
| TypeOfMeteringPoint | MarketEvaluationPointType | | TimeSeries/MarketEvaluationPoint/MarketEvaluationPointType |
| SettlementMethod | SettlementMethod | | TimeSeries/MarketEvaluationPoint/SettlementMethod |
| MeteringPointDomainLocation/Identification | mRID | MarketEvaluationPoint | MarketEvaluationPoint/mRID |
| IntervalEnergyObservation | Period | | TimeSeries/Period |
| Position | Position | Point | TimeSeries/Period/Point/Position |
| EnergyQuantity | Quantity | | Energy_Quantity/Quantity |
| QuantityQuality | Quality | | TimeSeries/Period/Point/Quality |
| QuantityMissing* | Quality | | TimeSeries/Period/Point/Quality |

*QuantityMissing is a CIM quality value.
