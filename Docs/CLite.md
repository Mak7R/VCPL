<link rel="stylesheet" href="styles.css">

# CLite
<h3>#import</h3>
<p>
The import directive accepts 2 or 3 arguments: A file without the specified format (you can use the path, or it will be searched in the directory where the executable file is located, or in the directory where the compiler is located), syntax without quotes in its name and an optional argument namespace name </p>

<pre class="code">#import(file, syntax, namespace = file)</pre>

<p>Example:</p>
<pre class="code">#import(MyLibrary, CLite, lib)
lib.func(lib.variable)</pre>


<h3>#include</h3>
<p>
The include directive accepts 1 or 2 arguments: A file without the specified format (the path can be used, or it will be searched in the directory where the executable file is located, or in the directory where the compiler is located), and an optional argument the name of the namespace</p>

<pre class="code">#include(file, namespace = file)</pre>

<p>Example:</p>
<pre class="code">#include(MyLibrary, lib)
lib.func(lib.constant)</pre>

<h3>#init</h3>
<p>
The init directive takes 1 or 2 arguments: the name of the variable, and if you want to create a constant, you specify its value</p>

<p>Initialize variable:</p>
<pre class="code">#init(var)</pre>

<p>Constant initialization:</p>
<pre class="code">#init(var, value)</pre>

<p>Example:</p>
<pre class="code">#init(myVar)
myVar = 1
#init(myConst, 1)</pre>


<h3>#define/#end/return</h3>
<p>
The define directive takes the required argument-name of the function and any number of argument names of this function. The end directive takes only one argument, the name of the function to end. The return function may not accept any values or may accept the value of a variable whose value will be placed in the Stack to position 0, 0 (temp variable)</p>

<pre class="code">#define(func, arg1 .. argn)
#end(func)</pre>

<p>Examples:</p>
<pre class="code">#define(myFoo, arg)
WriteLine(console, arg)
#end(myFoo)</pre>

<pre class="code">#define(myFoo, arg)
arg = +(arg, 1)
return(arg)
#end(myFoo)
#init(var)
var = 43
myFoo(var)
var = temp</pre>

<p class="important">Clarification</p>
<p>If you use your own program for compilation, make sure that there is a variable in the lowest level stack and use its name instead of temp</p>

<h4><a href="/Examples/">More code examples in the Examples folder</a></h4>


# UA

<h3>#import</h3>
<p>
Директива import приймає 2 або 3 аргументи: Файл без вказаного формату (можна повиний шлях, або його будуть шукати в каталозі де знаходиться виконуваний файл, або в каталозі де знаходиться компілятор), синтаксис без лапок в його назві і необов'язковий аргумент назва простору імен</p>

<pre class="code">#import(file, syntax, namespace = file)</pre>

<p>Приклад:</p>
<pre class="code">#import(MyLibrary, CLite, lib)
lib.func(lib.variable)</pre>


<h3>#include</h3>
<p>
Директива include приймає 1 або 2 аргументи: Файл без вказаного формату (можна повиний шлях, або його будуть шукати в каталозі де знаходиться виконуваний файл, або в каталозі де знаходиться компілятор), і необов'язковий аргумент назва простору імен</p>

<pre class="code">#include(file, namespace = file)</pre>

<p>Приклад:</p>
<pre class="code">#include(MyLibrary, lib)
lib.func(lib.constant)</pre>

<h3>#init</h3>
<p>
Директива init приймає 1 або 2 аргументи: назва змінної, і якщо потрібно створити констану то вказуєте її значення</p>

<p>Ініціалізація змінної:</p>
<pre class="code">#init(var)</pre>

<p>Ініціалізація константи:</p>
<pre class="code">#init(var, value)</pre>

<p>Приклад:</p>
<pre class="code">#init(myVar)
myVar = 1
#init(myConst, 1)</pre>


<h3>#define/#end/return</h3>
<p>
Директива define приймає обов'язковий аргумнт-назву функції та будь-яку кількість назв аргументів цієї функції. Директива end приймає лише один аргумент, назву функціїї яку слід завершити. Функція return можне не приймати ніяких значень або приймати значення змінної значння якої буде покладено в Стек до позиціїї 0, 0 (змінна temp)</p>

<pre class="code">#define(func, arg1 .. argn)
#end(func)</pre>

<p>Приклади:</p>
<pre class="code">#define(myFoo, arg)
WriteLine(console, arg)
#end(myFoo)</pre>

<pre class="code">#define(myFoo, arg)
arg = +(arg, 1)
return(arg)
#end(myFoo)
#init(var)
var = 43
myFoo(var)
var = temp</pre>

<p class="important">Уточнення</p>
<p>Якщо ви використовуєте власну програму для компіляції то запевніться, що в стеку найнижчого рівня є змінна і використовуйте замість temp її назву</p>

<h4><a href="/Examples/">Більше прикладів коду в папці Examples</a></h4>