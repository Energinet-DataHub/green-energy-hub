# Mapping RSM-033 from ebIX to CIM

## Introduction

In Denmark the Market Actors communicate with DataHub using a set of RSM-messages (SOAP) that follow the industrial standard of [ebIX](https://www.ebix.org/). Once received by DataHub, a message will be converted to an internal format, a Data Transfer Object, which conforms to the naming convention of the CIM standard. When a message is sent from DataHub to a Market Actor this will yet again be following the ebIX standard.

## Motivation

The purpose of this document is create a single mapping reference for RSM-033 messages.

## Mapping table

The mapping covers the following RSM-033 messages and adheres to the general [rsm-to-cim.md](https://github.com/Energinet-DataHub/green-energy-hub/blob/main/docs/document-type-mappings/rsm-to-cim.md) document.

>- **DK_RequestUpdateChargeInformation**
>- **DK_ConfirmUpdateChargeInformation**
>- **DK_RejectUpdateChargeInformation**

All RSM-messages share a generic set of attributes, e.g. _MessageReference_, _Sender/Recipient information_ and _DocumentType_ to mention a few.
The mapping below covers the generic set of attributes.

> Do note, that some of the CIM names are identical and will require a context to acquire uniqueness, hence a suggested context is provided for those mappings.

### Generic message content

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

### DK_RequestUpdateChargeInformation

| **EbIX attribute**|**CIM name**| **Suggested "context" if needed to obtain CIM name uniqueness** | **CIM path** |
|:-|:-|:-|:-|
| PayloadChargeEvent/Identification | mRID | | MktActivityRecord/mRID |
| Occurrence | DateTime | ValidityStart_| MktActivityRecord/ValidityStart_DateAndOrTime/DateTime |
| Function | Status | MktActivityRecord_ | MktActivityRecord/status |
| ChargeType | Type |  | Series/ChargeType/type|
| PartyChargeTypeID | mRID | ChargeType_ | Series/ChargeType/mRID |
| Description | Name | | MktActivityRecord/ChargeType/name |
| LongDescription | Description | | MktActivityRecord/ChargeType/description |
| VATClass | VATPayer | | MktActivityRecord/ChargeType/VATPayer |
| TransparentInvoicing | TransparentInvoicing | | MktActivityRecord/ChargeType/TransparentInvoicing |
| TaxIndicator | TaxIndicator | | MktActivityRecord/ChargeType/TaxIndicator |
| Position | Position | | Period/Point/Position |
| EnergyPrice | Amount | | Period/Point/Price/Amount |
| ResolutionDuration | Resolution | | Period/Resolution |
| ChargeTypeOwnerEnergyParty/Identification | mRID | ChargeTypeOwner_| Series/ChargeType/ChargeTypeOwner_MarketParticipant/mRID |

### DK_ConfirmUpdateChargeInformation

| **EbIX attribute**|**CIM name**| **Suggested "context" if needed to obtain CIM name uniqueness** | **CIM path** |
|:-|:-|:-|:-|
| Identification | mRID | | MktActivityRecord/mRID |
| OriginalBusinessDocument | mRID | OriginalTransactionReference | MktActivityRecord/OriginalTransactionReference_MktActivityRecord/mRID |
| StatusType | Code || Reason/Code |

### DK_RejectUpdateChargeInformation

| **EbIX attribute**|**CIM name**| **Suggested "context" if needed to obtain CIM name uniqueness** | **CIM path** |
|:-|:-|:-|:-|
| Identification | mRID | |MktActivityRecord/mRID |
| OriginalBusinessDocument | mRID | OriginalTransactionReference | MktActivityRecord/OriginalTransactionReference_MktActivityRecord/mRID |
| StatusType | Code || Reason/Code |
| ResponseReasonType | Reason || MktActivityRecord/Reason|
