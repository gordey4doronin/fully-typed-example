[en-US :us:](https://github.com/gordey4doronin/fully-typed-example/blob/master/README.md) | ru-RU :ru:
#  На пути к полной типизации с TypeScript, Swashbuckle и AutoRest 

## Введение

В данной статье рассматривается вопрос о том, как реализовать обмен типизированными сообщениями между Back-End на основе ASP.NET Web API и Front-End, созданного с использованием TypeScript.
Это приобретает особенное значение при работе над объёмными проектами, и тем-более важно, если команда является распределенной.
Например, когда Back-End и Front-End разработчики работают из разных мест, в разных часовых поясах, и не всегда имеют возможность проконтактировать и обсудить что-либо.
В этом случае отслеживание изменений представляет кропотливую работу, которая может быть чревата множеством трудноуловимых ошибок.

Для автора статьи, как для человека, который пришел к разработке Front-End со стороны WPF и Silverlight, большой проблемой, стало отсутствие статической типизации.
Сколько раз вместо того чтобы сложить “2” и “2” складывал “2” и “Функцию возвращающую 2”, или передавал DOM объект вместо его jQuery обертки.
Появление статических анализаторов кода, таких как JSLint, несколько облегчило проблему, но настоящим прорывом, особенно в командной разработке, для нас стал TypeScript.

![TypeScript + Swagger logo](https://habrastorage.org/files/b0b/c02/9aa/b0bc029aa8f7457784c62d25b8dbb42b.png)

## Суть проблемы

TypeScript – язык, который позволяет достичь статической типизации, хотя кое-кто называет ее “иллюзией” ( https://habrahabr.ru/post/258957/, https://habrahabr.ru/post/272055/ ).
Любопытно, что критики особо выделяют работу с Back-End как типично нетипобезопасный сценарий.
Однако, суть проблемы заключается в том, что при написании Front-End приложений на JavaScript прежде, и на TypeScript в настоящее время, мы не имеем такого инструментария для работы с метаданными и авто-генерации клиентского кода, как некогда имели в WCF.

## Метаданные

Если обратиться к опыту WPF+WCF, то там в этом отношении все достаточно хорошо.
Конечно данные, вообще говоря, всегда путешествуют в нетипизированном виде, но при отправке они остаются типизированными почти до самого конца, и лишь непосредственно перед посылкой другой стороне сериализуются в строку или бинарный поток.
На другой стороне они, опять же, попадают в некоего клиента, который превращает их в типизированные.
Для того, чтобы не писать руками такого клиента и не пытаться выловить множественные ошибки и существуют метаданные.
В мире .NET в 90% случаев вообще не требуется никакой работы, ни для их генерации, ни для их обработки.
Вы просто пишите свой сервис, не забыв добавить соответствующий endpoint, и получаете автогенерируемые метаданные.
Также в один клик генерируете клиента и в результате получаете обмен типизированными сообщениями.

При разработке Single Page Application на JavaScript/TypeScript на смену WCF приходит Web API.
В своё время было несколько удивительно, почему нет никакого способа генерации метаданных для Web API из коробки (не считать же help-pages метаданными).
Видимо ответ в том, что главным получателем данных от Web API был код JavaScript, в котором типизация не имеет смысла.
Однако, у нас теперь не JavaScript а TypeScript, и желание оперировать типизированными данными вновь становится актуальным.

Очень популярным форматом метаданных сейчас является OpenAPI/Swagger.
Неудивительно, что появляются возможности генерировать метаданные и документацию в этом формате.

Далее мы продемонстрируем процесс организации типизированного взаимодействия.
Вкратце, мы выполним следующие шаги:

1. Подключим и настроим библиотеку Swashbuckle
2. Сгенерируем документацию/метаданные
3. Убедимся в удобности хранения сгенерированного файла в системе контроля версий
4. Подключим AutoRest
5. Сгенерируем клиентские модели
6. Опробуем их в деле.

## Swashbuckle
https://github.com/domaindrivendev/Swashbuckle

Для начала мы хотим сгенерировать метаданные.
Итак, предположим у нас есть Web API, а в нем — контроллер, отвечающий за работу с сотрудниками.

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

Как видно – возвращается массив типизированных объектов типа Employee.
Запустив наш проект, мы можем запросить список сотрудников:
http://localhost:1234/api/employees

Давайте теперь подключим библиотеку Swashbuckle.
В NuGet существует два пакета Swashbuckle.Core и Swashbuckle.
Разница между ними состоит в том, что первый является ядром и содержит весь код, делающий магию, a второй, в свою очередь, является лишь дополнением, которое устанавливает бутстраппер, конфигурирующий ядро.

Об этом написано в документации https://github.com/domaindrivendev/Swashbuckle#iis-hosted

Мы отдаем предпочтение установке Core и написанию конфигурационного кода самостоятельно, т.к. его потом удобнее переиспользовать.

Давайте его установим

```
PM> Install-Package Swashbuckle.Core
```

зарегистрируем с помощью WebActivatorEx

```cs
[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(FullyTypedExample.WebApi.SwaggerConfig), "RegisterGlobal")]
```

а также напишем код конфигурации

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

Здесь все довольно просто: сначала мы устанавливаем версию и заголовок нашего API.
Далее говорим, что нужно включить xml-комментарии для контроллеров и моделей.
Настраиваем порядок и группировку action внутри swagger-документа.
Отдельно хочется упомянуть опцию PrettyPrint.
Она включает форматирование JSON для swagger-документа.
Эта опция пригодится для того, чтобы в дальнейшем документацию хранить в системе контроля версий и с легкостью просматривать её изменения, используя любой diff просмотрщик.

Теперь можно запустить проект и увидеть интерфейс Swagger.
http://localhost:1234/swagger

![Swagger UI](https://habrastorage.org/files/41d/d89/f88/41dd89f886774cb3b52a5b6d4876894b.png)

Рядом можно посмотреть на сам swagger-документ в виде JSON.
http://localhost:1234/swagger/docs/v1

Теперь нам нужно сложить сгенерированную документацию в систему контроля версий.
Поскольку Swashbuckle использует под капотом майкрософтовский IApiExplorer, для того чтобы сгенерировать swagger файл обязательно придется запустить Web API (подробнее об этом тут https://github.com/domaindrivendev/Swashbuckle/issues/559).
То есть каждый раз, когда вы хотите сгенерировать документацию, вам придется запустить Web API и скопировать swagger/docs в файл вручную.
Конечно же, хочется что-то более автоматизированное.

Мы решили это с помощью запуска Web API в виде self-hosted приложения, отправки запроса не endpoint swagger-а и записи ответа в файл.
Тут как раз и пригодилось переиспользовать код конфигурации Swashbuckle. Выглядит все это примерно так:

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

Давайте теперь все это запустим:

```bash
nuget.exe restore "..\FullyTypedExample.sln"
"C:\Program Files (x86)\MSBuild\12.0\bin\MSBuild.exe" "..\FullyTypedExample.WebApi.SelfHosted\FullyTypedExample.WebApi.SelfHosted.proj" /v:minimal
"..\FullyTypedExample.WebApi.SelfHosted\bin\Debug\FullyTypedExample.WebApi.SelfHosted.exe" --swagger "swagger.json"
```

Итого мы получили swagger-документ в виде JSON файла и положили его в систему контроля версий.
Теперь Front-End разработчики из нашей распределенной команды могут с легкостью отследить изменения в endpoint-ах.
Давайте посмотрим, как это выглядит.

Допустим, мы добавили новый action для получения сотрудника по его идентификатору.

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

И заново сгенерировали swagger.json. Посмотрим, что поменялось

![Swagger docuemnt diff](https://habrastorage.org/files/e72/72c/346/e7272c346d144e9aab1f882a2a93aecd.png)

Как видите, для этого action появилась документация, которую легко можно увидеть, используя diff просмотрщик.
Благодаря опции PrettyPrint она отформатирована и легко читается.

## AutoRest
https://github.com/Azure/autorest

Итак, первая часть нашего задания выполнена — метаданные у нас есть.
Как же теперь сгенерировать клиентскую часть, т.е. типы данных (получаемых с сервера) на клиентской стороне?

Нужно сказать, что можно генерировать и сам код для запроса Web API, просто это немного сложнее и требует более трудоемкой работы по конфигурации кодогенераторов или написанию своих.
Также, многое зависит от того, какие библиотеки (будь то jQuery, SuperAgent или даже новый экспериментальный Fetch API https://developer.mozilla.org/en/docs/Web/API/Fetch_API) и подходы (Promises, Rx и др.) вы используете в своем клиентском коде.

Для кодогенерации существуют следующие варианты:

1. **Swagger Code Generator** https://github.com/swagger-api/swagger-codegen
Официальный инструмент от команды Swagger, написан на Java и требует соответствующей инфраструктуры.
Также может запускаться в Docker. Правда, генерация JavaScript и тем-более TypeScript в нем отсутствует.
Хотя если вам нужно сгенерировать код, например, на Java — это ваш выбор. Нам он не подошел по понятным причинам.

2. **Swagger JS library** https://github.com/swagger-api/swagger-js
Тоже официальный инструмент от команды Swagger.
Уже теплее. Написан на JS и генерирует JS код соответственно.
Устанавливается через npm или bower.
Инфраструктура нам подходит, но увы здесь нет той самой генерации типов.

3. **Swagger to JS & Typescript Codegen** https://github.com/wcandillon/swagger-js-codegen
Проект был опубликован несколько позже чем мы начали разрабатывать этот подход.
Возможно в ближайшем будущем это станет самым подходящим решением.

4. Написать свой ~~велосипед~~ кодогенератор.
В целом, почему бы и нет? Но для начала мы решили, что попробуем AutoRest, и если не взлетит, или не устроит нас возможностями, напишем таки свой, с блэк-джеком и… Ну вы поняли.

5. **AutoRest** https://github.com/Azure/autorest
И наконец, AutoRest от Azure команды Microsoft.
Сейчас актуальная версия — 0.15.0, и честно говоря непонятно, считается ли это у них полноценным релизом или нет, но пометки Pre, как на предыдущих, не наблюдается.
В общем, тут все просто, мы установили и сходу сгенерировали *.d.ts файлы, которые нам и были нужны.

Итак, давайте пройдем заключительный отрезок нашего пути с помощью этого инструмента.

Подключаем AutoRest через NuGet:

```
PM> Install-Package AutoRest
```

Пакет не ставится в какой-то конкретный проект, ссылка на него добавляется для всего решения.

```xml
<?xml version="1.0" encoding="utf-8"?>
<packages>
  <package id="AutoRest" version="0.15.0" />
</packages>
```

В пакете есть консольное приложение AutoRest.exe, которое, собственно, и выполняет генерацию.
Для запуска мы используем следующий скрипт

```bash
nuget.exe restore "..\FullyTypedExample.sln"
"..\packages\AutoRest.0.15.0\tools\AutoRest.exe" -Input "swagger.json" -CodeGenerator NodeJS
move "Generated\models\index.d.ts" "..\FullyTypedExample.HtmlApp\models.d.ts"
```

На вход мы подаем наш ранее сгенерированный swagger.json, а на выходе получаем models\index.d.ts — файл с моделями.
Копируем его в клиентский проект.

Теперь в TypeScript мы имеем следующее описание модели:

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

Давайте опробуем его в деле:

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

Здесь мы обращаемся к полям модели id и name.
Мы намеренно опустили реализацию запроса на сервер, т.к. она, как мы уже и говорили, может зависеть от выбранных библиотек и подходов.

Если мы попытаемся обратиться к полю age, которого не существует, наш TS код не скомпилируется.
Если в API исчезнет поле, к которому мы обращались ранее, наш код опять же не скомпилируется.
Если добавятся новые поля, мы это сразу увидим, используя все тот же diff.
Кроме того, мы автоматически получаем JSDoc документацию на основе метаданных.
В общем, все прелести статической типизации налицо.

## ResponseType

Интересно, что при необходимости для документации можно указать иной тип нежели тот, что возвращается.
Например, это может быть полезным при наличии legacy-кода, работающего с нетипизированными DataSet-ами;
либо, если вы возвращаете IHttpActionResult из контроллеров.
Не затрагивая реализацию методов, мы можем пометить их атрибутом ResponseType и разработать специальные типы

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

чтобы получить на клиентской стороне типизированные модели

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

## Проблемы

Во-первых, рост размера файла models.d.ts со временем.
Пока мы еще не занимались разбиением его на несколько подфайлов, но это несомненно нужно будет сделать.

Также может возникнуть проблема с неправильной генерацией имен полей, если используется нестандартная нотация, например, если используются подчеркивания.
Поле LAST_NAME из C# кода сгенерируется в Swagger как lasT_NAME, а в TypeScrpt — как lasTNAME.

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

Отметим, что большинство мелких проблем легко решается с помощью конфигурации и не стоит отдельного упоминания.
 
## Заключение

Данный подход позволил нам организовать обмен типизированными сообщениями.
При этом он обеспечил типизацию клиентских моделей, уменьшил вероятность расхождения клиентского и серверного кода, сделал более простым отслеживание изменений в API и моделях.
Приятным бонусом стало удобное ручное тестирование API со встроенным REST-клиентом и возможностью генерации payload на лету по схеме.
Использование данного подхода также помогло улучшить взаимодействие Back-End и Front-End разработчиков. 
