# DeploymentConsole

## Purpose

During the cause of time, the structure contained in a SQL server will change, be it tables, views, stored procedures etc.

The purpose of the deployment console is to make sure that the database structures are up to date.

As such, it contains a number of scripts needed to go from an empty database to a database containing all the needed structure for the newest version. That is, the entire update history.

After the SQL server has been deployed this tool can be run and will then ensure that all the scripts available that has not already been run on the server will be executed.

## Running the tool manually

The tool can be run manually on any SQL server database using the following command:

Energinet.DataHub.MarketData.DeploymentConsole.exe <connection string>

<connection string> must be replaced with the correct connection string for the database, possibly encapsulated with "

## Adding a new script to be synced to SQL servers on deploy

When a new script should be added to the tracked updates, a new .sql file should be added to the Scripts folder using the next available number.

So if the folder already contains the following scripts:

* Script0001 - Some subject.sql
* Script0002 - Other subject.sql
* Script0003 - Yet another subject.sql

Then the new file should be prefixed "Script0004" and contain the TSQL to execute the update.
