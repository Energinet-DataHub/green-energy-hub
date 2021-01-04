# Mapping of RSM to CIM

## Dictionary

This dictionary can be used for looking up what the name of your CIM attribute should be, when coming from a different document standard.

| RSM Name | CIM Name |
| -------- | -------- |
| AssetType | **Technology** |
| Attention | **Name** |
| BalanceResponsiblePartyID | **Mrid** |
| EnergySupplierID | **Mrid** |
| BuildingNumber | **Number** |
| ChargeOccurrence | **Factor** |
| ChargeType  | **Type** |
| ChargeOwner | **Mrid** |
| CityName | **Code** |
| CitySubDivisionName | **Name** |
| ConsumerCategory | **Kind** |
| CPR | **Mrid** |
| CVR | **Mrid** |
| CustomerName | **Name** |
| Multiplier | **Multiplier** |
| Country | **TownDetailCountry** |
| Currency Unit | **Name** |
| DARReference | **GeoInfoReference** |
| ChargeName | **Name** |
| DisconnectionType | **DisconnectionMethod** |
| ElectricalHeating | **Kind** |
| ElectricalHeatingDate | **DateTime** |
| Email | **ElectronicAddress** |
| End | **End** |
| Price | **Amount** |
| Quantity | **Quantity** |
| PriceAmount | **Quantity** |
| EstimatedAnnualVolume | **Quantity** |
| FirstCustomerCPR | **Mrid** |
| FirstCustomerCVR | **Mrid** |
| FirstCostumerName | **Name** |
| FloorIdentification | **FloorIdentification** |
| OutArea | **Mrid** |
| Function | **Status** |
| HasEnergySupplier | **HasBalanceSupplier** |
| ParentMeteringPoint | **ParentDomain** |
| MPDescription | **Description** |
| ChargeDescription | **Mrid** |
| DomainArea | **Mrid** |
| RatedCapacity | **ContractedConnectionCapacity** |
| RatedCurrent | **RatedCurrent** |
| MeterID | **Meter** |
| MeteringGridArea | **Mrid** |
| MeteringPointID | **Mrid** |
| MeterReadingPeriodicity | **MeterReadingPeriodicity** |
| MeterType | **Accumulation** |
| MPCapacity | **PhysicalConnectionCapacity** |
| MPConnectionType | **MPConnectionType** |
| CollectionMethod | **MeteredDataCollectionMethod** |
| ContactAddressType | **AdministrativePartyUsagePointLocation** |
| MunicipalityCode | **Code** |
| FirstContactName | **Name** |
| NetSettlementGroup | **NetSettlementGroup** |
| ValidityDate | **DateTime** |
| OriginalBusinessDocument | **Mrid** |
| ChargeID | **Mrid** |
| Mobile | **MobilePhone** |
| ConnectionState | **ConnectionState** |
| Position | **Position** |
| PostalCode | **PostalCode** |
| LinkedPowerPlant | **LinkedDomain** |
| PriceMissing | **Point** |
| ProductID | **Product** |
| ProductObligation | **ProductionObligation** |
| ProtectedAddress | **Remark** |
| ProtectedName | **ProtectedName** |
| Resolution | **Resolution** |
| ReasonCode | **Reason** |
| SuiteNumber | **SuiteNumber** |
| NextReadingDate | **NextReadingDate** |
| SecondCustomerCPR | **Mrid** |
| SecondCustomerCVR | **Mrid** |
| SecondCustomerName | **Name** |
| SettlementMethod | **SettlementMethod** |
| Start | **Start** |
| StartDate | **Date** |
| StatusType | **Code** |
| Street Code | **Code** |
| StreetName | **Name** |
| MeteringMethod | **MeteringMethod** |
| SupplyStart | **DateTime** |
| TaxIndicator | **TaxIndicator** |
| TimeseriesID | **Mrid** |
| InArea | **Mrid** |
| TransactionID | **Mrid** |
| MPType | **MarketEvaluationPointType** |
| Unit | **Name** |
| VATClass | **VATPayer** |
| Version | **Version** |
| WebAccessCode | **WebAccessCodeDescription** |
| PriceUnit | **Name** |
| POBox | **PoBox** |
| Phone | **Phone** |
| BusinessReasonCode | **ProcessType** |
| DocumentID | **Mrid** |
| DocumentType | **Type** |
| CreationDate | **CreatedDateTime** |
| SenderID | **Mrid** |
| RecipientID | **Mrid** |
| TransparentInvoicing | **TransparentInvoicing** |
| IndustryClassification | **MarketServiceCategory** |
| SenderRole | **Type** |
| ReceiverRole | **Type** |

## CIM Structure

Below is the initial document structure for CIM documents.

### MktActivityRecord

```ruby
{
  "MktActivityRecord": {
    "Status": "",
    "Reason": "",
    "Mrid": "",
    "DKExtMarketEvaluationPoint": {
        "HasBalanceSupplier": "",
        "ParentDomain": "",
        "Description": "",
        "MeterReadingPeriodicity": "",
        "PhysicalConnectionCapacity": "",
        "MPConnectionType": "",
        "MeteredDataCollectionMethod": "",
        "NetSettlementGroup": "",
        "RatedCurrent": "",
        "ContractedConnectionCapacity": "",
        "Meter": "",
        "ConnectionState": "",
        "NextReadingDate": "",
        "LinkedDomain": "",
        "MeteringMethod": "",
        "InMeteringGridAreaDomain": "",
        "ProductionObligation": "",
        "ProtectedName": "",
        "EnergyTechnologyAndFuel": {
            "Technology": ""
        },
        "EnergySupplierMarketParticipant": {
            "Mrid": ""
        },
        "Register": {
            "Channel": {
                "ReadingType": {
                    "Multiplier": ""
                }
            },
            "ReadingType": {
                "Accumulation": ""
            },
            "LeftDigitCount": "",
            "RightDigitCount": ""
        },
        "DisconnectionMethod": "",
        "TimeSeries": {
            "EstimatedAnnualVolumeQuantity": {
                "Quantity": ""
            }
        },
        "FirstCustormerMarketParticipant": {
            "MRID": "",
            "Name": ""
        },
        "OutMeteringGridAreaDomain": {
            "Mrid": ""
        },
        "MeteringGridAreaDomain": {
            "Mrid": ""
        },
        "SecondCustormerMarketParticipant": {
            "Mrid": "",
            "Name": ""
        },
        "TimeSeries": {
            "Product": "",
            "MeasurementUnit": {
                "Name": ""
            }
        }
    },
    "MarketEvaluationPoint": {
        "Mrid": "",
        "AdministrativePartyUsagePointLocation": {
            "AttnNames": {
                "Name": ""
            },
            "MainStreetAddress": {
                "TownDetail": {
                    "Code": ""
                },
                "PoBox": ""
            },
            "ElectronicAddress": "",
            "MobilePhone": "",
            "Phone": "",
            "Remark": ""
        },
        "UsagePointLocation": {
            "MainStreetAddress": {
                "StreetDetail": {
                    "Number": "",
                    "FloorIdentification": ""
                },
                "TownDetail": {
                    "Name": "",
                    "Country": ""
                }
            },
            "StreetDetail": {
                "SuiteNumber": "",
                "Code": "",
                "Name": ""
            },
            "GeoInfoReference": "",
            "PostalCode": ""
        },
        "CustomerMarketParticipant": {
            "Mrid": "",
            "Name": "",
            "WebAccessCodeMarketParticipant": {
                "WebAccessCodeDescription": ""
            }
        }
    },
    "ChargeType": {
        "Factor": "",
        "Name": "",
        "Mrid": "",
        "VATPayer": "",
        "TaxIndicator": "",
        "TransparentInvoicing": ""
    },
    "ServiceCategory": {
        "CustomerAgreement": {
            "Customer": {
                "Kind": ""
            }
        },
        "Kind": ""
    },
    "EletricalHeatingDateAndOrTime": {
        "DateTime": ""
    },
    "ValidityStartDateAndOrTime": {
        "DateTime": ""
    },
    "OriginalTransactionReferenceMktActivityRecord": {
        "Mrid": ""
    },
    "StartDate": {
        "Date": ""
    },
    "SupplyStartDateAndOrTime": {
        "DateTime": ""
    }
  }
}
```

### TimeSeries

```ruby
  "TimeSeries": {
    "Mrid": "",
    "Product": "",
    "BalanceResponsiblePartyMarketParticipant": {
        "Mrid": ""
    },
    "EnergySupplierMarketParticipant": {
        "Mrid": ""
    },
    "CurrencyUnit": {
        "Name": ""
    },
    "Period": {
        "Point": {
            "Quality": "",
            "Price": {
                "Amount": "",
            },
            "EnergyQuantity": {
                "Quantity": ""
            },
            "EnergySumQuantity": {
                "Quantity": ""
            }
        },
        "Resolution": ""
    },
    "MarketEvaluationPoint": {
        "SettlementMethod": "",
        "MarketEvaluationPointType": ""
    },
    "QuantityMeasurementUnit": {
        "Type": ""
    }
  }
```

### Series

```ruby
    {
      "Series": {
        "Version": "",
        "Product": "",
        "ChargeType": {
            "Mrid": ""
            "Type": "",
            "ChargeTypeOwnerMarketParticipant": {
                "Mrid": ""
            }
        },
        "Period": {
            "TimeInterval": {
                "End": "",
                "Start": ""
            },
            "Point": {
                "Position": "",
            }
        },
        "QuantityMeasurementUnit": {
            "Name": ""
        },
        "MeteringGridAreaDomain": {
            "Mrid": ""
        },
        "BiddingZoneDomain": {
            "Mrid": ""
        }
      }
    }
```

### MarketDocument

```ruby
    }
      "MarketDocument": {
        "Mrid": "",
        "CreatedDateTime": "",
        "Type": "",
        "Process": {
            "ProcessType": "",
        },
        "MarketServiceCategory": {
            "Kind": ""
        },
        "SenderMarketParticipant": {
            "Mrid": "",
            "MarketRole": {
                "Type": ""
            }
        },
        "RecieverMarketParticipant": {
            "Mrid": "",
            "MarketRole": {
                "Type": ""
            }
        }
      }
    }
```

### NotifyAggregatedTimeSeriesMarketDocument

```ruby
    }
      "NotifyAggregatedTimeSeriesMarketDocument": {
        "DocStatus": ""
      }
    }
```

### AdministrativeCustomerMarketParticipant

```ruby
    }
      "AdministrativeCustomerMarketParticipant": {
        "Name": ""
      }
    }
```

### ConfirmRequestChangeOfSupplierMarketDocument

```ruby
    }
      "ConfirmRequestChangeOfSupplierMarketDocument": {
        "Reason": {
          "Code": ""
        }
      }
    }
```
