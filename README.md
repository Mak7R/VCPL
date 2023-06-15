# VCPL
<h3>EN</h3>
<p>
VCPL is a Virtually Compiled Programming Language.
In the process of virtual compilation, the text code is converted into a set (array)
instructions, and function names are converted to addresses (indexes in the data array).
</p>
<p>
At the moment, the language is procedural. The user can write instructions and combine them into functions.
Each function has its own data context
(Local variables are not accessible from other points in the program,
and global ones can be changed from any function)
(Requires checks and corrections).
</p>
<p>
To expand the capabilities of the VCPL language, you can write a class library in C#.
The library must contain the MethodContainer class, which must contain the GetAll method.
You can also add a GetNecessaryLibs method,
which will pass the list of libraries needed to run.
(Planned to replace with automatic recognition of required libraries)
</p>


<h3>UA</h3>
<p>
VCPL - це віртуально компільована мова програмування. 
В процесі віртуальної компіляції текстовий код перетворюється в набір(масив)
інструкцій, а назви функцій перетворються на адреси (індекси в масиві данних).
</p>
<p>
На даний момент мова є процедурною. Користувач може писати інструкції та об'єднувати їх до функцій.
Кожна функція має свій контекст даних 
(Локальні змінні не доступні з інших точок програми, 
а глобальні можуть бути змінені з будь-якої функції)
(Потребує перевірок та виправлень).
</p>
<p>
Для розщирення можливостей мови VCPL можна написати бібліотеку класів на C#. 
Бібліотека має містити клас MethodContainer в якому обов'язково містити метод GetAll.
Також можна додати метод GetNecessaryLibs, 
який передасть список необхідних для запуску бібліотек. 
(Заплановано замінити на автоматичне розпізнавання необхідних бібліотек)
</p>