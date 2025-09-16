<!-- (dl (section-meta Summary)) -->

The ORM enhancement you didn't know you needed.

### The problem

You have a table that is both wide and deep that you need to provide a custom view into. You want to selectively choose both what columns are returned as well as the column order returned.

This is where traditional DotNet ORMs fail you. They have to have preconfigured types with preconfigured columns and column order. To get around this, every mainstream ORM provides the capability to pass SQL. However building this SQL opens you up to SQL injection attacks.

### The Solution

Safe Dynamic SQL Builder, allows you to preconfigure the shape of the table, without predefining the shape of the output. It does this by generating custom SQL based off of a query object that can be received from a client.

#### Safe From SQL Injection

Safe Dynamic SQL Builder will **_never_** use client strings in building custom queries. The query object is only a specification for the query to be built.
