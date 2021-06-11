# Test data in Green Energy Hub

The ENDK Datahub business will generate and own a generic dataset containing the master data as listed below. This dataset is intented to be the foundation for testing activities of Green Energy Hub on the DEV and TEST environments.

* Organisation name and IDs (GLN and EIC - see Footnote 1) for a number of:
    * Energy Suppliers
    * Balance Responsible parties
    * Grid Access Providers
    * eSett (the Nordic imbalance settlement)
    * System operators
* 10 Grid areas
* Metering points randomly distributed across the grid areas

The dataset's IDs will be unique to avoid problems when the test data migration from the ENDK Datahub 2 commence.

As evident from the master data attributes listed above, this dataset will only contain a limited set of data. However, using the dataset as the foundation, teams will be able to create mocked data and associate these with a meaningful set of master data. Generation of mock data (and the ownership of these) will be the responsibility of the team that needs to run a test suite, e.g. intra domain integration tests.

## Where to find and how to update

The generic dataset will be made accessible as an Excel spreadsheet in the green-energy-hub repository in the [dataset](./dataset/) folder.

Thereby the generic dataset will be available to the GEH community. As Green Energy Hub grows and matures, this dataset is expected to do so as well. But as of now, June 2021, the dataset is scoped to the aforementioned master data.

However, if additional generic attributes needs be added to the dataset, please send this request to team Mighty Ducks by raising an issue in the green-energy-hub repository and assign it to: ASQ-EN, djorgensendk, and Renetnielsen. For the community to be able to work with a consistent dataset, updates to it needs to be coordinated.

Footnote 1: The EIC field will be available, but it value is not yet mandatory, as the format is yet to be determined.
