# Test data in Green Energy Hub
The ENDK business will generate and own a generic dataset containing the following master data:
* Metering point IDs

The dataset IDs will be unique to avoid problems when the test data migration from Datahub 2 commence.
As evident from the list of data properties above, this dataset will only contain a limited set of data. However, by using the dataset as the foundation, teams will be able to create mocked data and associate these with a meaningful set of test master data. Generation of mock data (and the ownership of these) will be the responsibility of the team that needs to run a test suite, e.g. infra domain integration tests.

## Where to find and how to update
The generic dataset will be made accessible in the green-energy-hub repository in the following folder: [green-energy-hub/docs/test-data/dataset/](green-energy-hub/docs/test-data/dataset/)


Arket skal være tilgængelig til alle teams og proces skal faciliteres (GITHUB)  
 
Der kan godt forekomme flere ark og der vil ske en evolution af data med tiden men scope for nu er kun master data. 
 
Mangler der properties i arket så skal det meldes tilbage til forfatter af arket så arket vedligeholdes og ikke bare tilføjes lokalt. 
