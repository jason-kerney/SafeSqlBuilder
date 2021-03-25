
<!-- GENERATED DOCUMENT! DO NOT EDIT! -->
# Safe-SQL-Builder #


## Table Of Contents ##

- [Section 1: Summary](#user-content-summary)
- [Section 2: Use](#user-content-use)
- [Section 3: Enhancements up for discussion](#user-content-enhancements-up-for-discussion)

## Summary ##

The ORM enhancement you didn't know you needed.

### The problem

You have a table that is both wide and deep that you need to provide a custom view into. You want to selectively choose both what columns are returned as well as the column order returned.

This is where traditional DotNet ORMs fail you. They have to have preconfigured types with preconfigured columns and column order. To get around this, every mainstream ORM provides the capability to pass SQL. However building this SQL opens you up to SQL injection attacks.

### The Solution

Safe Dynamic SQL Builder, allows you to preconfigure the shape of the table, without predefining the shape of the output. It does this by generating custom SQL based off of a query object that can be received from a client.

#### Safe From SQL Injection

Safe Dynamic SQL Builder will **_never_** use client strings in building custom queries. The query object is only a specification for the query to be built.

    

## Use ##
The developer defines the columns that are allowed as part of the query in an object called ```ValidColumns```.

Example:
```c#
var validColumns = new ValidColumns(new []
{
    "ProductId",
    "Name",
    "ProductNumber",
    "Color",
    "StandardCost",
    "ProductLine",
});
```
----

> **WARNING:** This object is what protects you from SQL Injection. It is intended to be predefined by the programmer and should _NEVER_ contain strings from an untrusted source.

----

Once the valid columns is defined you will then allow the client to configure a Query object.

Example:
```c#
var query = new Query
{
    fields = new[]
    {
        "Name",
        "Color",
    },
    filters = new[]
    {
        new Filter
        {
            property = "Name",
            values = new object[] {"Widget-D", "Enameled Widget"}
        },
        new Filter
        {
            property = "ProductLine",
            values = new object[]
            {
                "(Blanks)",
                "Books",
                "Widgets",
            }
        },
        new Filter
        {
            property = "Color",
            values = new object[] {"#AB558A"}
        },
    },
    sort = "Name",
    direction = "asc"
};
```

You will then create an instance of the ```QueryOrchestrotor``` and pass it both the query and the valid columns. Calling ```ToString()``` on the returned ```SqlGenerator``` will give you the desired sql statement.

```c#
var builder = new QueryOrchestrator();
var (sqlQueryGenerator, parameters) = builder.BuildQuery(query, validColumns, "[AdventureWorks].[Product]");
var sql = sqlQueryGenerator.ToString();
```

The SQL generated for the query is:

```sql
SELECT [Color]
	,[Name]
FROM [AdventureWorks].[Product]
WHERE ([ProductLine] = @ProductLine_EQUAL)
	AND ([Name] = @Name_EQUAL)
ORDER BY CASE
		WHEN [ProductNumber] IS NULL
			THEN 1
		ELSE 0
		END
	,[ProductNumber]
```

All filter values are parameterized. Which brings me to the second item returned from the ```BuildQuey``` method. The parameters are returned as an ```IEnumerable<(string, object)>``` where the string is the parameter name, including the ```@``` and an object holding the value to be passed to your database query manager or ORM.

    

## Enhancements up for discussion ##

These are ideas we have had, but have not investigated yet. As such these are not promises of future features, only things we are considering.

* Value Replacement
* Multiple Sort Columns
* Not keyword
  * Not between
  * Not equal to
  * Not IN
  * Not Greater then (turns into ```<=```)
  * Not Greater then or equal to (turns into ```<```)
  * Not Less then (turns into ```>=```)
  * Not Less then or equal to (turns into ```>```)
* ```'SELECT * '``` if no fields given
* Value cast
* String Parsing of value to type (aka date string to date)
* Coalescing of values
* Escaping brackets in column names
    

<!-- GENERATED DOCUMENT! DO NOT EDIT! -->
    