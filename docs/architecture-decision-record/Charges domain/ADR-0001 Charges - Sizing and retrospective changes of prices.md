# Sizing and retrospective changes of prices

Prices consist of two separate pieces of data in the danish market for DataHub 2 today. These are price lists and price list links.

Each of these will be described separately.

## Price lists

Price list contains master data about the prices including the type of the prices (tariff, fee or subscription) and the prices themselves.

The prices can either be a fixed price or exactly 24 prices each representing a specific hour of the day.

### Unique price lists in DataHub 2

| Price type | Amount |
|:-----------|-------:|
|Tariffs|510|
|Fees|973|
|Subscriptions|731|

### Retrospective changes to price lists

In the current setup very few price lists are changed back in time.

Since DataHub 2 went live, the master data of the price list has only been changed retrospectively 1 or 2 times.

The prices themselves are changed retrospectively 3-5 times per year.

### Tariffs as a time series

The use of tariff price lists in DataHub 2 has some limitations, which are caused by the model used.

When we go from standard time to summer time there are only 23 hours in the day. When we go from summer time to standard time there are 25 hours in a day. Both of these can be hard to express with a price list consisting of exactly 24 hours in a day.

Another common problem of a tariff is to have separate prices for weekends and work days. In the current setup this means that the price list has to change every Saturday and Monday to reflect these changes. Naturally this causes alot of updates to price lists.

To counter these disadvantages a new design of the price lists allows the prices of tariffs to be represented by a time series instead of 1 or 24 prices. This allows the owner of the price list to make a tariff that changes based on amount of hours in the day, type of day or any other sort of distinction they want to represent their prices.

As a result of this the tariff price lists are expected to have their master data change much less frequent.

Of course, this also means that a single tariff will have approximately 8,760 (365 days times 24 hours) prices for a year instead of now where there is 1 or 24 prices per historical change (2,496 for 24 hour tariffs that change every Saturday and Monday (24 hours, 52 weeks, 2 changes per week))

So with 510 tariffs there will be approximately 4,467,600 individual prices for tariffs in a year.

### Future changes of tariff price lists

It is currently being discussed whether the market could benefit from tariffs based on the metering grid area the market evaluation point is located in.

Should this change happen it is expected that the amount of tariffs will increase by around a factor for 100, meaning the number of tariffs would be around 51,000.

This brings the amount of individual prices in a year up to approximately 446,760,000 per year.

## Links

To determine, which price lists to use for a specific market evaluation point the price list links are used.

A link is simply a representation of the period that a specific market evaluation point is using a specific price list. As such a market evaluation point has links to multiple price lists at any given time.

### Amount of links

Since most market evaluation points will have multiple links to price lists, the amount of links is some factor times the amount of market evaluation points.

This means that there are in the matter of millions of links in DataHub 2.

### Retrospective changes of links

The various market participants are allowed to change links to the market evaluation points they are responsible for up to 90 calendar days in the past.

Besides this there is a window every 14 days where the participants are allowed to change links further back in time. This is not for "forgotten" links, but to cope with the fact that a market evaluation point may be approved (outside of the DataHub) for e.g. electrical heating or a net settlement group. Therefore links need to be corrected accordingly back in time, often longer than 90 calendar days. Such events also affect the links that Energinet handles, and thus Energinet also operates with an open time frame.  

The frequency of these changes are as follows:

* 3,000 links per year by grid operators
* 1,000 links per year by Energinet
