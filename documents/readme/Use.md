<!-- (dl (section-meta Use)) -->
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
var (sqlQuery, parameters) = builder.BuildQuery(query, validColumns, "[AdventureWorks].[Product]");
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
