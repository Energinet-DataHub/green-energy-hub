# Non-functional requirements

Non-functional requirements are “Expected Product Properties and User Expectations”

This means that these are not requirements that can be implemented as a feature – but expectations that can be fulfilled when implementing features.

This is not a full list but can inspire to consider.

## Connectivity

- Expectations to how we ensure connectivity, and the needs for logging, and information on this.

## Continuity

### Reliability

- Expectations on a break-down free system

### Robustness

- Expectations to how easily (automatic?) the system can proceed after a breakdown or incident has occurred

### Recoverability

- Expectations to ease and speed with which the functionality/work can be resumed following a breakdown

### Degradation Factor

- Expectation to the ease with which parts of the system can continue running if other parts are shut down?

### Fail over possibilities

- Expectations to fail over possibilities for entire system or partly instances? (geo redundance is part of this, when talking disaster recovery)

## Data Controllability

- Expectations on checksums, cross checks, and audit trails
- Expectation on relevant measures, monitoring

## Effectivity

- Expectations of the system ability to be as effective as the organization and end user for achieving company goals
- Expectation on how the system can increase the efficiency of a business process
- Expectation on how the usability (Of GUI, support tools, debugging, Root Cause analysis etc.). of the systems supports efficient work.

## Efficiency

- Expectation on performance (transaction volume and total speed) and volume of resources (subscriptions, steel, network usage, memory etc.).

## Flexibility

- Expectations on how easy it is to introduce enhancements or variations of user work in the system without amending the software. (config, parameters changes, feature toggling, who, what when, how often, for what etc.).
- How easy should it be for operations to amend the use of the system without having the development teams involved?

## Functionality

- The expectation on the system’s ability to process information accurately and completely.

### Accuracy

- The expectation to the degree, that the system must correctly process supplied input data and mutations of it and place it into consistent collections and generate output.

### Completeness

- Expectation to certainty that all input and mutations are being processed.

## Suitability (of infrastructure)

- Expectation on technical architecture in a general sense – relevance and how it interconnects)

## Maintainability

- How easy should the system be able to adapt new requirements from users to test environments or repair faults in the system (bad usage, bad data).
- Measures could be, Average effort used on maintenance, Average duration of repair, Structure of Software – how easy is it to fix, and review of documentation needed for repair, setting up etc.

## Manageability

- Expectation on how easily the system can be placed and maintained in operational condition.
- Expectations to Installation and administrative procedures (backup, recovery etc.)

## Performance

- Expectations to the speed of which the system handles interactive and batch transactions
- Measures could be, Response time, Throughput, Utilization, Static Volume, Stress handling, Peak KPI’s

## Portability

- Expectation to the diversity of the hardware and software platform and how easy the system can be transferred from one environment to another.

## Reusability

- Expectations on how easy it should be to use the system or parts of it in other contexts or other applications. (This also benefits the maintainability)

## Security

- Expectations on authorization/user management
- Data security
- GDPR
- Audit

## Suitability

- How easily should the system and manual procedures interconnect and how should the workability be on these manual procedures?
- Expectations to timeliness in processes – how should the system ensure timeliness of data availability in manual processes?

## Testability

- The expectation to how easy the system can be tested both internal and externally
- Quality of documentation
- The expectation on automated test (regression or in general)
- Test tools – is the system possible for user testing?
- Expectation on how the test results is made, used, published and assessed.
- Possibility of the system can be manipulated for testing purposes.

## User-friendliness

- Expectations on the ease of the system to be used by end users (both B2B and GUI)
- How easy it is to learn to use
- This is often determined by subjective opinion/evaluation
