# XML schema validation

This project can help validate XML to known XSD schemas.

## Usage

Create an instance of ```XmlSchemaValidator``` and use the ```ValidateStreamAsync``` method to validate a xml-stream. The return object tells whether or not the validation was successful.
XSD schema to use for validation is based on the root-element and default namespace of the xml-stream. By default all collections are scanned, but it is possible with an overload to select a specific schema collection.

## Extending with new collections

If a new collection is needed the enum SchemaCollections must be extended with the new schema. Also the class SchemaCollection needs to be extended to include the new collection in the static Find method.

## Adding new xsd-files

In the schemas folder find the matching collection and paste the new xsd-file. Locate the corresponding SchemaCollection class and insert a SchemaDefinition entry. Ensure that the new xsd file is included as an EmbeddedResource in the assembly.

## Example usage

```csharp
public async Task ProcessData(Stream xmlStream) {
   IXmlSchemaValidator xmlValidator = new XmlSchemaValidator();
   var result = await xmlValidator.ValidateStreamAsync(xmlStream);

   if (!result.IsSuccess) throw new ArgumentException("Invalid XML");
}
```

## DataHub folder structure maintenance

The DataHub schemas comes from three different sources and as such, a full set cannot be found at a single location

### First source

The first is the general SOAP specification.

This has been manually extracted from the SOAP website and placed in the file \DataHub\wsdl\soap-envolope-manual.xsd

### Second source

The second part is the general B2B wsdl interface of DataHub.

The schemas for this has been extracted from the wsdl-file published for parties which want to interface to the DataHub. It is important to only extract the schema part of the file

This has been placed in the file \DataHub\wsdl\DataHUBB2B-manual.xsd

### Third source

The third and final source is the RSM-schemas from the DataHub, defining the individual messages.

These are based on the ebiX-standard and comes as fragments in a folder structure.

The generic folder contains schema fragments shared between documents.
The document folder contains a subfolder for each individual RSM document used.

## RSM Model

The generated directory contains a model file containing all RSM models that have been generated from the XSD files in `Schemas / DataHub / document`.
The file can  be used to create and serialize RSM messages to XML and also to deserialize them into their respective model.

To update the `RsmModel.cs` open Powershell in the root project directory and execute the following command:  
`Remove-Item .\Generated\*.cs;xsd $(Get-ChildItem -Path .\Schemas\DataHub\generic\*.xsd,.\Schemas\DataHub\document\*.xsd -recurse | Resolve-Path -Relative) /classes /out:Generated;Get-ChildItem -Path .\Generated | Where {$_ -like "*.cs"} | Rename-Item -NewName RsmModels.cs`

If the XSD command is not found add "C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8 Tools\x64\" to your env path.
