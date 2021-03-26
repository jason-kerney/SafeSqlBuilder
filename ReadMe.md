
<!-- GENERATED DOCUMENT! DO NOT EDIT! -->
# Safe-SQL-Builder #


## Table Of Contents ##

- [Section 1: Summary](#user-content-summary)
- [Section 2: Use](#user-content-use)
- [Section 3: Thanks to all Contributors](#user-content-thanks-to-all-contributors)
- [Section 4: Enhancements up for discussion](#user-content-enhancements-up-for-discussion)

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
    "ProductID",
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

    

## Thanks to all Contributors ##

## Contributors ✨

Thanks goes to these wonderful people ([emoji key](https://allcontributors.org/docs/en/emoji-key)):

<!-- ALL-CONTRIBUTORS-LIST:START - Do not remove or modify this section -->
<!-- prettier-ignore-start -->
<!-- markdownlint-disable -->
<table>
  <tr>
    <td align="center"><a href="https://github.com/edf-re"><img src="https://avatars.githubusercontent.com/u/13739273?v=4?s=100" width="100px;" alt=""/><br /><sub><b>EDF Renewables</b></sub></a><br /><a href="#financial-edf-re" title="Financial">💵</a></td>
    <td align="center"><a href="http://www.chrisstead.net/"><img src="https://avatars.githubusercontent.com/u/4184510?v=4?s=100" width="100px;" alt=""/><br /><sub><b>Chris Stead</b></sub></a><br /><a href="#ideas-cmstead" title="Ideas, Planning, & Feedback">🤔</a></td>
    <td align="center"><a href="https://github.com/patrickhigh"><img src="https://avatars.githubusercontent.com/u/45110206?v=4?s=100" width="100px;" alt=""/><br /><sub><b>patrickhigh</b></sub></a><br /><a href="https://github.com/jason-kerney/SafeSqlBuilder/commits?author=patrickhigh" title="Code">💻</a> <a href="#ideas-patrickhigh" title="Ideas, Planning, & Feedback">🤔</a></td>
    <td align="center"><a href="https://github.com/seventumbles"><img src="https://avatars.githubusercontent.com/u/1326703?v=4?s=100" width="100px;" alt=""/><br /><sub><b>Mike Lazar</b></sub></a><br /><a href="https://github.com/jason-kerney/SafeSqlBuilder/commits?author=seventumbles" title="Code">💻</a></td>
  </tr>
</table>

<!-- markdownlint-restore -->
<!-- prettier-ignore-end -->

<!-- ALL-CONTRIBUTORS-LIST:END -->

This project follows the [all-contributors](https://github.com/all-contributors/all-contributors) specification. Contributions of any kind welcome!
    

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
    