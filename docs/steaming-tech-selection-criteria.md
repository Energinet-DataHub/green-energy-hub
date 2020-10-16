
# Streaming technology selection criteria

## Data in transit

The chosen technology should be able to ingest data time series records coming at predefined
message rate per second and provide aggregated calculation results within certain time frame.

For Denmark case today the requirement is translated to:
_The chosen technology should be able to ingest data time series records coming at pace 60000 records per second and provide aggregated calculation results within 10 minutes._

See example calculation for incoming message rate below:

| country    | population  | households | message rate  **  |
|------------|-------------|------------|-------------------|
| Denmark    | 5.8 mln     | 3.6 mln    | 60k per sec       |
| Germany    | 83 mln      | 51 mln  *  | 850K per sec      |
| USA        | 330 mln     | 205 mln *  | 3,4 mln per sec   |

`*` factor 0.62 as per Denmark is used for illustration
`**` for sending measurements every minute

Time frame for calculation should be determined by business need and technology limitation.
It might depend on processing ratio and the cost we would like to maintain.

## Data at rest

The chosen technology should be able to provide aggregated calculation of data at rest within certain time frame.
Time frame for calculation should be determined by business need and technology limitation.
It might depend on processing ratio and the cost we would like to maintain.

## Developer confidence

- Steepness of learning curve
- Existing familiarity in the team with the technology
- Well-established and widely used, is it easy to find new hires with existing set of skills
- is technology mature: people already use it and there is community around it

## Cost

- estimated price per month

## Stability

- Guaranteed system uptime
- Fault tolerant and robust
- System can be recreated from scratch
- Calculation can be paused and resumed after maintenance

## DevOps

- automatically configurable and deployable : availability of CI/CD automation for technology

## Maintainability

- managed hardware, PaaS based infrastructure
- possibility to switch to provider-agnostic solution
- existing code base can be adjusted to other industry players needs
- management overhead
- extensibility: designed in extensible way, new functionality can be introduced
- easy maintainable framework for unit/integration testing

## OSS and Reuse

- reusable components available; i.e. drivers, connectors, prebuilt integrations with for cloud services and frameworks, for example: Azure EventHub/Kafka, Azure Data Lake, Delta Lake, SQL Server
- is technology open source making it easy to contribute/reuse others word

## Observability

- logging and fail-management: ability to see streaming logs, get notified when it fails, automatic and manual restarts

## Unique functionality

- what distinguishes solution from other options
