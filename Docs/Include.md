<link rel="stylesheet" href="styles.css">

# EN 
<h3>How to import a C# code file into VCPL</h3>

<p>
In order to import the C# library (.dll), it is necessary to write:
<pre class="code">
#include(FileName\Or\PathToFile\WithoutFormat)
</pre>

Or

<pre class="code">
#include(FileName\Or\PathToFile\WithoutFormat, NamespaceName)
</pre>

To use data from the library, write:
<pre class="code">
yournamespace.data
</pre>
</p>


<strong class="important">Clarification</strong>
<p>
<p>
When specifying a <strong class="important">NOT</strong> file, write its .dll format, and the file itself must necessarily have the .dll format. Also, the version of .NET in your library must be the same as the program you are using.
</p>
<p>
If you import a file with a full path and no namespace specified, your namespace will be the filename instead of the full path.
</p>

<p>
The library file must have the same name as the namespace in it, and the code must also contain a Library class with a public static field Items of type ICollection<(string? name, object? value)>.
</p>

<p>
The namespace that was used will also be added to the constant stack.
</p>

<p>When developing your library, import the GlobalInterface module to it. This will allow you to create functions and manage pointers to them</p>

# UA
<h3>Як імпортувати файл з кодом C# до VCPL</h3>

<p>
Для того щоб імпортувати бібліотеку С# (.dll) необіхдно написати: 
<pre class="code">
#include(FileName\Or\PathToFile\WithoutFormat)
</pre>

Або 

<pre class="code">
#include(FileName\Or\PathToFile\WithoutFormat, NamespaceName)
</pre>

Для використання данних з бібліотеки напиши:
<pre class="code">
yournamespace.data
</pre>
</p>


<strong class="important">Уточнення</strong>
<p>
<p>
При вказанні файлу <strong class="important">НЕ</strong> пишіть його формат .dll, а сам файл повинен мати обов'язково формат .dll. До того ж версія .NET у вашій бібліотеці повинна бути тою ж що і програма яку ви використовуєте.
</p>
<p>
Якщо ви імпортуєте файл з повним шляхом і без вказання простору імен вашим простором імен стане ім'я файлу а не весь шлях.
</p>

<p>
Файл бібліотеки обов'язково повинен мати назву туж саму, що і простір імен у ньому, також код повинен містити клас Library з публічним статичним полем Items типу ICollection<(string? name, object? value)>.
</p>

<p>
Простір імен, що був використаний також додастся до стеку констант.
</p>

<p>При розробці своєї бібліотеки імпоруйте до неї модуль GlobalInterface. Це дозволить вам створювати функції та керувати вказівниками в них</p>