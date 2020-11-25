
# Expected data coming into calculation engine

The following table gives an overview of the object expected coming into the calculation engine.
The first column shows the properties of the individual time series objects.
The last column shows how data can be derived from originating [ebIX format](https://www.ebix.org/)

## Header values

- `Property` : The Name of the property
- `Type`: The originating data type
- `Converted Type`: The formatting of the conversion
- `Scale`: The scale of the decimal conversion
- `Precision`: The precision of the decimal conversion
- `C# Type`: The type in c# language
- `Spark Type`: Spark data type
- `Originating Xml Schema Name`: The field in the originating Xml Schema where this value can be found
- `Note`: Additional information about this property

| `Property`                                                                                   | `Type`    |`Converted Type`|`Scale`|`Precision` |`c# Type`   | `Spark type`  |  | `Originating Xml Schema Name`                                                              |`Note`              |
|----------------------------------------------------------------------------------------------|-----------|---------------|-------|-----------|-------------|---------------|--|------------------------------------------------------------------------------------------|--------------------|
| CorrelationId                                                                                | ByteArray | UTF8          |       |           | STRING      | StringType    |  |                                                                                    |                          |
| MessageReference                                                                             | ByteArray | UTF8          |       |           | STRING      | StringType    |  | MessageReference                                                                         |                    |
| HeaderEnergyDocument_mRID                                                                    | ByteArray | UTF8          |       |           | STRING      | StringType    |  | HeaderEnergyDocument\Identification                                                      |                    |
| HeaderEnergyDocumentCreation                                                                 | Int96     |               |       |           |             | TimestampType |  | HeaderEnergyDocument\Creation                                                            | UTC time           |
| HeaderEnergyDocumentSenderIdentification                                                     | ByteArray | UTF8          |       |           | STRING      | StringType    |  | HeaderEnergyDocument\SenderEnergyParty\Identification                                    |                    |
| EnergyBusinessProcess                                                                        | ByteArray | UTF8          |       |           | STRING      | StringType    |  | ProcessEnergyContext\EnergyBusinessProcess                                               | 4 Possible values  |
| EnergyBusinessProcessRole                                                                    | ByteArray | UTF8          |       |           | STRING      | StringType    |  | ProcessEnergyContext\EnergyBusinessProcessRole                                           | 5 Possible values  |
| TimeSeries_mRID                                                                               | ByteArray | UTF8          |       |           | STRING      | StringType    |  | PayloadEnergyTimeSeries\Identification                                                   |                    |
| MktActivityRecord_Status                                                                     | ByteArray | UTF8          |       |           | STRING      | StringType    |  | PayloadEnergyTimeSeries\Function                                                         | 2 Possible values  |
| Product                                                                                      | ByteArray | UTF8          |       |           | STRING      | StringType    |  | PayloadEnergyTimeSeries\IncludedProductCharacteristic\Identification                     | 6 Possible values  |
| UnitName                                                                                     | ByteArray | UTF8          |       |           | STRING      | StringType    |  | PayloadEnergyTimeSeries\IncludedProductCharacteristic\UnitType                           | 8 Possible values  |
| MarketEvaluationPointType                                                                    | ByteArray | UTF8          |       |           | STRING      | StringType    |  | PayloadEnergyTimeSeries\DetailMeasurementMeteringPointCharacteristic\TypeOfMeteringPoint | 34 Possible values |
| SettlementMethod                                                                             | ByteArray | UTF8          |       |           | STRING      | StringType    |  | PayloadEnergyTimeSeries\DetailMeasurementMeteringPointCharacteristic\SettlementMethod    | 3 Possible values  |
| MarketEvaluationPoint_mRID                                                                   | ByteArray | UTF8          |       |           | STRING      | StringType    |  | PayloadEnergyTimeSeries\MeteringPointDomainLocation\Identification                       |                    |
| Quantity                                                                                     | ByteArray | DECIMAL       | 6     | 35        | DECIMAL     | DecimalType   |  | PayloadEnergyTimeSeries\IntervalEnergyObservation\EnergyQuantity                         | 35 digits max      |
| Quality                                                                                      | ByteArray | UTF8          |       |           | STRING      | StringType    |  | PayloadEnergyTimeSeries\IntervalEnergyObservation\QuantityQuality                        | 4 Possible values  |
| ObservationTime                                                                              | Int96     |               |       |           |             | TimestampType |  |                                                                                          |                    |
| Fields above this line comes with the times series, fields below are added during enrichment |           |               |       |           |             |               |  |                                                                                          |                    |
| MeteringMethod                                                                               | ByteArray | UTF8          |       |           | STRING      | StringType    |  |  - SubTypeOfMeteringPoint                                                                |                    |
| MeterReadingPeriodicity                                                                      | ByteArray | UTF8          |       |           | STRING      | StringType    |  |  - MeterReadingOccurrence                                                                |                    |
| MeteringGridArea\_Domain_mRID                                                                 | ByteArray | UTF8          |       |           | STRING      | StringType    |  |  - GridArea                                                                              |                    |
| ConnectionState                                                                              | ByteArray | UTF8          |       |           | STRING      | StringType    |  |  - PhysicalStatusOfMeteringPoint                                                         |                    |
| EnergySupplier\_MarketParticipant_mRID                                                        | ByteArray | UTF8          |       |           | STRING      | StringType    |  |  - BalanceSupplier                                                                       |                    |
| BalanceResponsibleParty\_MarketParticipant_mRID                                               | ByteArray | UTF8          |       |           | STRING      | StringType    |  |  - BalanceResponsibleParty                                                               |                    |
| InMeteringGridArea\_Domain_mRID                                                               | ByteArray | UTF8          |       |           | STRING      | StringType    |  |  - ToGridArea                                                                            |                    |
| OutMeteringGridArea\_Domain_mRID                                                              | ByteArray | UTF8          |       |           | STRING      | StringType    |  |  - FromGridArea                                                                          |                    |
| Parent_Domain                                                                                | ByteArray | UTF8          |       |           | STRING      | StringType    |  |  - ParentMeteringPointId                                                                 |                    |
| SupplierAssociationId                                                                        | ByteArray | UTF8          |       |           | STRING      | StringType    |  |  - CustomerId (new thing, does not exist today)                                          |                    |
| ServiceCategoryKind                                                                          |           |               |       |           |             |               |  |  - IsElectricalHeating                                                                   |                    |
