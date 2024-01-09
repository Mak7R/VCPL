<link rel="stylesheet" href="styles.css">


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