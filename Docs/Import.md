<link rel="stylesheet" href="styles.css">

# EN
<h3>How to import a file with a different VCPL code into the main one</h3>

<p>
In order to import another file, it is necessary to write:
<pre class="code">
#import(FileName\Or\PathToFile\WithoutFormat, SyntaxName)
</pre>

Or

<pre class="code">
#import(FileName\Or\PathToFile\WithoutFormat, SyntaxName, NamespaceName)
</pre>

To use data from the library, write:
<pre class="code">
yournamespace.data
</pre>
</p>

<strong class="important">Clarification</strong>
<p>
<p>
When specifying a <strong class="important">NOT</strong> file, write its .vcpl format, and the file itself must necessarily have the .vcpl format.
</p>
<p>
When specifying the <strong class="important">NOT</strong> syntax, write it in quotation marks.
</p>
<p>
The namespace is specified only for the data that was initialized in the file. For others (data from dependencies of this file), the namespace will remain as it was.
</p>

# UA
<h3>Як імпортувати файл з іншим кодом VCPL до основного</h3>

<p>
Для того щоб імпортувати інший файл необіхдно написати: 
<pre class="code">
#import(FileName\Or\PathToFile\WithoutFormat, SyntaxName)
</pre>

Або 

<pre class="code">
#import(FileName\Or\PathToFile\WithoutFormat, SyntaxName, NamespaceName)
</pre>

Для використання данних з бібліотеки напиши:
<pre class="code">
yournamespace.data
</pre>
</p>

<strong class="important">Уточнення</strong>
<p>
<p>
При вказанні файлу <strong class="important">НЕ</strong> пишіть його формат .vcpl, а сам файл повинен мати обов'язково формат .vcpl.
</p>
<p>
При вказанні синтаксису <strong class="important">НЕ</strong> пишіть його в кавичках.
</p>
<p>
Простір імен вказується лише для даних що були ініціалізовані у файлі. Для інших (дані з залежностей цього файлу) простір імен залишиться тим, що був.
</p>