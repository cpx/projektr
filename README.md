## What is this?

A library for filtering object properties using a text-based filter syntax, e.g. for filtering the response object in a RESTful API.

It takes a filter `string` as the input and outputs a dynamically compiled projector method of type `Func<T, ExpandoObject>`, that takes an instance of `T` and returns an instance of `ExpandoObject` that only contains the properties specified in the filter.

:warning: This is a very early, proof-of-concept solution that is not particularly optimized, does not have any proper error handling, nor is it battle-tested yet. It can hopefully still be useful, but buyer beware.

### Syntax

A filter is specified as a comma-separated list of property names. E.g. the filter `Property1,Property2` returns an object containing only Property1 and Property2.

For properties of complex types, sub-filters can be specified using parens. E.g the filter `Property1,Property2(SubProperty1)` also contains an object containing only Property1 and Property2, but the value of Property2 in addition is filtered to only contain SubProperty1. These can also be nested, e.g `Property1,Property2(SubProperty1,SubProperty2(SubSubProperty1),SubProperty3)`

### Example

```csharp
var rick = new Artist
{
	FirstName = "Rick",
	LastName = "Astley",
	BirthPlace = "Newton-le-Willows, England, United Kingdom",
	BirthDate = "1966-02-06",
	TopHits =
		{
			new Track
			{
				Title = "Never Gonna Give You Up",
				ReleaseYear = 1987,
				Length = TimeSpan.Parse("0:3:31")
			}
		}
};
var projektr = ProjektrCompiler.Compile<Artist>("FirstName,LastName,TopHits(Title,Length)");
var filteredRick = projektr(rick);
// creates object like:
var filteredRick2 = new
{
	FirstName = "Rick",
	LastName = "Astley",
	TopHits =
		{
			new
			{
				Title = "Never Gonna Give You Up",
				Length = TimeSpan.Parse("0:3:31")
			}
		}
};
```

### ASP.NET Web API

Filters can be added to a Web API action using the `UseProjektr` extension method and the `ProjektrFilterAttribute`.

E.g. to add Projektr and configure the "fields" parameter on a route to be used for the input filter:

```csharp
var config = new HttpConfiguration();
config.UseProjektr();
app.UseWebApi(config);
```
```csharp
[HttpGet]
public IHttpActionResult Get([FromUri,ProjektrFilter] string fields = null)
```

The HTTP response can now be filtered using a request like `GET /?fields=Property1,Property2`.

### Unsupported use cases / ToDo

- **Validation and error handling.** Only the "happy path" scenario is implemented. There is no validation of the input filter during any of the compilation phases. Incorrect usage will most likely lead to exceptions thrown from internally.
- **Filtering on sub-classes.** Because the code generator uses the declared type of a property to determine an object's schema, if the property value's type is a sub-class of the declared type, any properties belonging to the sub-class(es) will be unknown. This will only have an effect if you specify a property filter on the sub-type (since then a new object needs to be generated, otherwise the existing object will be re-used).
- **.NET Core support**
- **Web API: Using property from a complex route argument.** E.g:
	```csharp
	[HttpPost]
	public IHttpActionResult Post([FromBody] SomeRequestBody requestBody)
	{
		// ...
	}

	public class SomeRequestBody
	{
		[ProjektrFilter]
		public string Fields { get; set;}
	}
	```


### Under the hood

The library consists of a simple tokenizer and parser to produce an AST representing the filter, and a code generator that produces a compiled expression tree that constructs an ExpandoObject for the filtered types. There is a static helper type ```ProjektrCompiler``` that handles the whole chain from filter string to compiled code, that also caches the compilation result to avoid polluting the dynamic assembly with duplicates.