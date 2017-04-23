en-US :us: | [ru-RU :ru:](https://github.com/gordey4doronin/fully-typed-example/blob/master/README.ru-RU.md)
# On the way to full typing with TypeScript, Swashbuckle, and AutoRest

## Introduction

The article is devoted to the question of implementing exchange of typed messages between Back-End based on ASP.NET Web API and Front-End created with the use of TypeScript.
It is important for the teams working on big projects, especially for the distributed ones.
For example, when Back-End and Front-End developers work from different places and time zones often lacking a chance to communicate and discuss any issue.
Tracking changes in this case can be rigorous and causing elusive errors sometimes.

One of the main problems the author (who came to Front-End from WPF and Silverlight) faced was the absence of static typing.
Quite often instead of adding “2” and “2” he added “2” and “Callback function returning 2”, or passed DOM object instead of its jQuery wrapper.
Of course, occurrence of such static code analyzers as JSLint served as a solution for this problem.
However, TypeScript was a real breakthrough, especially for the teamwork.

![TypeScript + Swagger logo](https://habrastorage.org/files/b0b/c02/9aa/b0bc029aa8f7457784c62d25b8dbb42b.png)

## Problem Key Part

TypeScript is a programming language that allows reaching static typing, even though some people call it “Illusion” ( https://habrahabr.ru/post/258957/, https://habrahabr.ru/post/272055/ ).
It is interesting that critics describe an interaction with a Back-End as a typical not type-safe scenario.
Actually, the problem is that when writing Front-End application using JavaScript before and TypeScript now, we do not have the same tools to work with metadata and auto-generate client code, which we had in WCF for example.

## Metadata
As for WPF+WCF experience, everything works well there. Even though data always tranfsers as not typed, while sending it is typed until the very end. Only right before it is sent to the other side, it is serialized into string or binary stream.
After that, some client on the other side makes it typed.
In order not to write such a client by hand trying to fix numerous errors, metadata is used.
In the .NET world, 90% of situations do not require any additional work for either its generation or processing.
You just write your service adding a corresponding endpoint and receive auto-generated metadata.
Then in one click, you generate the client and, as a result, you get the exchange of typed messages.

Web API comes to change WCF for developing Single Page Application using JavaScript/TypeScript.
The absence of ways to generate metadata for Web API from the box (we cannot regard help-pages as metadata) was quite surprising for the first time.
Seems, the thing is that the main data recipient from Web API was JavaScript code, in which typing does not play any sense.
However, now we have TypeScript instead of JavaScript, which means operating typed data takes its role again.

Now one of the most popular metadata format is OpenAPI/Swagger. No wonder that this format gives new opportunities to generate metadata and documentation.

The article will demonstrate the process of organization of typed interoperability through these steps:

1. Add and set up Swashbuckle library
2. Generate documentation/metadata
3. Check it is comfortable to store generated file in the version control system
4. Add AutoRest
5. Generate client models
6. Try them in work

## Swashbuckle
https://github.com/domaindrivendev/Swashbuckle

First, we need to generate metadata. So, let us assume that we have Web API with the controller responsible for the work with the employees.

```cs
/// <summary>
/// Gets all employees.
/// </summary>
/// <remarks>
/// Gets the list of all employees.
/// </remarks>
/// <returns>
/// The list of employees.
/// </returns>
[Route("api/employees")]
[HttpGet]
public Employee[] GetEmployees()
{
    return new[]
        {
            new Employee { Id = 1, Name = "John Doe" },
            new Employee { Id = 2, Name = "Jane Doe" }
        };
}
```

As you see, the array of the typed objects of Employee type is returned.
Running the project, we can make a query of the employee list: http://localhost:1234/api/employees

Now it is turn to add Swashbuckle library.
In NuGet there are two packages: Swashbuckle.Core and Swashbuckle.
The difference between them is that the first is the core with the code making all magic, and the second is an addition, which installs bootstrapper to configure that core.

It is mentioned in the documentation: https://github.com/domaindrivendev/Swashbuckle#iis-hosted

We prefer to install the Core and write the configuration code ourselves, for it will be more comfortable to re-use it.

Let us install it

```
PM> Install-Package Swashbuckle.Core
```

then register with the help of WebActivatorEx

```cs
[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(FullyTypedExample.WebApi.SwaggerConfig), "RegisterGlobal")]
```

and write the configuration code

```cs
/// <summary>
/// Configures Swagger.
/// </summary>
/// <param name="config">
/// The Swagger configuration.
/// </param>
public static void ConfigureSwagger(SwaggerDocsConfig config)
{
    config.SingleApiVersion("v1", "FullyTypedExample.WebApi");
    config.IncludeXmlComments(GetXmlCommentsPathForControllers());
    config.IncludeXmlComments(GetXmlCommentsPathForModels());
    config.GroupActionsBy(apiDescription => apiDescription.ActionDescriptor.ControllerDescriptor.ControllerName);
    config.OrderActionGroupsBy(Comparer<string>.Default);
    config.PrettyPrint();
}
```

It is quite simple: first, we set the version and the API header.
Then we turn on xml-doc for the controllers and models.
After that we configure the order and action grouping inside the swagger-document.
We should also mention PrettyPrint option turning on JSON formatting for swagger-document.
This option is required for storing documentation in the version control system and easily review it using any of the diff viewer.

Now we can launch the project and see Swagger user interface:
http://localhost:1234/swagger

![Swagger UI](https://habrastorage.org/files/41d/d89/f88/41dd89f886774cb3b52a5b6d4876894b.png)

It is also possible to see the swagger-document in JSON format.
http://localhost:1234/swagger/docs/v1

Now we need to put the generated documentation into the version control system.
As soon as Swashbuckle uses Microsoft IApiExplorer under the hood, it is required to run Web API first to generate swagger file (find more on this: https://github.com/domaindrivendev/Swashbuckle/issues/559).
Therefore, every time when you want to generate the documentation you have to run Web API and copy swagger/docs into the file manually.
Of course, something more automated is preferred.

We solved this issue with the help of running Web API as a self-hosted application, sending request to swagger endpoint and writing the response into the file.
At this point, we needed to reuse the Swashbuckle configuration code. This is how it looks like:

```cs
/// <summary>
/// Generate Swagger JSON document.
/// </summary>
/// <param name="filePath">
/// The file path where to write the generated document.
/// </param>
private static void GenerateSwaggerJson(string filePath)
{
    // Start OWIN host
    using (TestServer server = TestServer.Create<WebApiHostStartup>())
    {
        HttpResponseMessage response = server.CreateRequest("/swagger/docs/v1").GetAsync().Result;

        string result = response.Content.ReadAsStringAsync().Result;
        string path = Path.GetFullPath(filePath);

        File.WriteAllText(path, result);
    }
}
```

Let’s launch it all: 

```bash
nuget.exe restore "..\FullyTypedExample.sln"
"C:\Program Files (x86)\MSBuild\12.0\bin\MSBuild.exe" "..\FullyTypedExample.WebApi.SelfHosted\FullyTypedExample.WebApi.SelfHosted.proj" /v:minimal
"..\FullyTypedExample.WebApi.SelfHosted\bin\Debug\FullyTypedExample.WebApi.SelfHosted.exe" --swagger "swagger.json"
```

As a result, we got the swagger-document in a JSON file and put it into the version control system.
Now the Front-End developers from our distributed team can easily track changes in the endpoints.
Let us see how it works.

For example, we added a new action to get the employee by the ID.

```cs
/// <summary>
/// Gets employee by id.
/// </summary>
/// <param name="employeeId">
/// The employee id.
/// </param>
/// <remarks>
/// Gets the employee by specified id.
/// </remarks>
/// <returns>
/// The <see cref="Employee"/>.
/// </returns>
[Route("api/employees/{employeeId:int}")]
public Employee GetEmployeeById(int employeeId)
{
    return this.GetEmployees().SingleOrDefault(x => x.Id == employeeId);
}
```

And then re-generated swagger.json. This is what changed:

![Swagger docuemnt diff](https://habrastorage.org/files/e72/72c/346/e7272c346d144e9aab1f882a2a93aecd.png)

As you can see, a new documentation, which is possible to view with any of the diff viewer, appeared for this action. Owing to PrettyPrint option, it is formatted and easy to read.

## AutoRest
https://github.com/Azure/autorest

So, the first task has been completed: we have metadata now.
How should we generate client code (i.e., the data types received from the server)?

It is worth to mention that it is possible to generate code for requesting Web API, although it is a bit harder and requires more efforts for code-generators configuration or writing your own ones.
Also, much depends on which libraries (jQuery, SuperAgent, or experimental Fetch API https://developer.mozilla.org/en/docs/Web/API/Fetch_API) and approaches (Promises, Rx, etc.) you use in your client code.

There are some options for code-generation:

1. **Swagger Code Generator** https://github.com/swagger-api/swagger-codegen
The official tool from the Swagger team written in Java and requiring the corresponding infrastructure.
It also can be started in Docker. However, it lacks JavaScript and especially TypeScript generation. Though, if you need to generate code, for instance in Java, it will be a good choice. As for us, we obviously could not use it.

2. **Swagger JS library** https://github.com/swagger-api/swagger-js
One more official tool from the Swagger team written in JS and generating JS code. It is installed through the nmp or bower. Infrastructure is quite suitable for us, but unfortunately it lacks type generation.

3. **Swagger to JS & Typescript Codegen** https://github.com/wcandillon/swagger-js-codegen
The project was published later after we started developing this approach. Probably it will become the most suitable solution in the future. 

4. **Write your own ~~bicycle~~ code-generator.**
Why not? Nevertheless, initially we decided to use AutoRest. If it fails, we will write our own with blackjack and … You know what we mean.

5. **AutoRest** https://github.com/Azure/autorest
Finally, it is turn of AutoRest from the Azure Microsoft team. The most recent version is 0.15.0. It is not quite clear whether it is the ready-to-use release or not, but we do not see any “Pre” note, like in previous ones.
Here it went simple, we installed and immediately generated *.d.ts files, which we actually needed.

So, now let’s pass to the final path with the help of this tool.

Add AutoRest through NuGet:

```
PM> Install-Package AutoRest
```

The package is not installed into the particular project, the reference to it is added for the whole solution.

```xml
<?xml version="1.0" encoding="utf-8"?>
<packages>
  <package id="AutoRest" version="0.15.0" />
</packages>
```

The package has the console application AutoRest.exe that does the generation. To start we use the following script: 

```bash
nuget.exe restore "..\FullyTypedExample.sln"
"..\packages\AutoRest.0.15.0\tools\AutoRest.exe" -Input "swagger.json" -CodeGenerator NodeJS
move "Generated\models\index.d.ts" "..\FullyTypedExample.HtmlApp\models.d.ts"
```

We input the generated swagger.json, and as an output, we get models\index.d.ts file with models. Then we copy it to the client project.

Now we have the following model description in TypeScript:

```typescript
/**
 * @class
 * Initializes a new instance of the Employee class.
 * @constructor
 * Represents the employee.
 * @member {number} id Gets or sets the employee identifier.
 * 
 * @member {string} name Gets or sets the employee name.
 * 
 */
export interface Employee {
    id: number;
    name: string;
}
```

Let’s try it in action:

```typescript
public makeRequest() {
    this.repository.getEmployees()
        .then((employees) => {
            // Generate html using tempalte string
            this.table.innerHTML = employees.reduce<string>((acc, x) => {
                    return `${acc}<tr><td>${x.id}</td><td>${x.name}</td></tr>`;
                }, '');
        });
}
```

Here we use the id and name model fields.
We intentionally skipped the server request implementation, as we mentioned it depends on chosen libraries and approaches.

If we try to access a non-existing age field, our TS code will not be complied.
If the API field which we have referred before disappears, the code will not be compiled again.
If some new fields are added, we will see it immediately using the already known diff tool. Moreover, we automatically get JSDoc documentation based on metadata. Well, all the benefits of static typing at work.

## ResponseType

What is interesting, there is a possibility to override returned type, if it is necessary for the documentation.
For example, it can be useful for the legacy-code working with non-typed DataSets, or if you return IHttpActionResult from the controllers.
We can mark methods by ResponseType attribute, not touching the implementation, and develop special types:

```cs
/// <summary>
/// Gets all departments.
/// </summary>
/// <remarks>
/// Gets the list of all departments.
/// </remarks>
/// <returns>
/// The list of departments.
/// </returns>
[Route("api/departments")]
[HttpGet]
[ResponseType(typeof(DepartmentsResponse))]
public DataSet GetDepartments()
{
    var dataTable = new DataTable("Departments");

    dataTable.Columns.Add("Id", typeof(int));
    dataTable.Columns.Add("Name", typeof(string));

    dataTable.Rows.Add(1, "IT");
    dataTable.Rows.Add(2, "Sales");

    var dataSet = new DataSet();
    dataSet.Tables.Add(dataTable);

    return dataSet;
}
```

in order to get the typed models on the client side

```typescript
/**
 * @class
 * Initializes a new instance of the Department class.
 * @constructor
 * Represents the department.
 * @member {number} id Gets or sets the department identifier.
 * 
 * @member {string} name Gets or sets the department name.
 * 
 */
export interface Department {
    id: number;
    name: string;
}
```

## Problems

First, models.d.ts file grows with the time.
For now, we have not tried to separate it into a few sub-files, but, surely, it will be necessary to do.

Second, there can occur the problem with the incorrect field names generation while using the non-standard notation. For example, if snake_case (i.e., underscore notation) is used.
LAST_NAME field from C# code is generated to Swagger as lasT_NAME, and to TypeScrpt as lasTNAME.

```cs
/// <summary>
/// Gets or sets the last name.
/// </summary>
[Required]
// ReSharper disable once InconsistentNaming
public string LAST_NAME { get; set; }
```

```json
"lasT_NAME": {
  "description": "Gets or sets the last name.",
  "type": "string"
}
```

```typescript
export interface Employee {
    id: number;
    name: string;
    firstName: string;
    lasTNAME: string;
}
```

By the way, most of the minor issues are easily solved with the help of the configuration and are not worth to mention.

## Conclusion

This approach let us organize the exchange of the typed messages.
Moreover, it provided typing of the client models, lowered the possible inconsistence of client and server code, and improved the source changes tracking in API and models.
One of the nice benefits of it is the comfortable manual API testing with the built in REST-client and possibility to generate payload on-the-fly using model schema.
Using this approach also helped to improve the interaction between Back-End and Front-End developers.
