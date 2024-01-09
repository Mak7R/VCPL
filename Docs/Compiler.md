<link rel="stylesheet" href="styles.css">

# EN
<h3>About the compiler and its basic implementation Compiler_IIDL</h3>
<p>
The compiler must implement the ICompiler interface. That is, it must have a CompileMain method that accepts an object of type CompileStack (a stack for data during compilation), string code (all the code in the form of a string), string convertorName (the name of the compiler for the main file) and string name (the name of the main function) (will be displayed in the error stack when DebugEnviroment is selected).
</p>

<p>
Compiler_IIDL base compiler.
This compiler first imports all dependencies it finds and dependencies for all dependencies. The imported code will be inserted instead of the #import directive. Next, all C# libraries added with the #include directive will be included. After that, the code first compiles all the directives. And then all the usual code will be compiled.
</p>

# UA
<h3>Про компілятор та його базову реалізацію Compiler_IIDL</h3>
<p>
Компілятор повинен реалізовувати інтерфейс ICompilator. Тобто повинен мати метод CompilateMain, що приймає обєкт типу CompileStack(стек для даних під час компіляції), string code(весь код у вигляді строки), string convertorName (назва компілятору для головного файлу) і string name (назва головної функції)(буде відображатися у стеку помилок при обраному середовищі DebugEnviroment).
</p>

<p>
Базовий компілятор Compilator_IIDL.
Цей компілятор спершу імпортує всі залежності, що знайде та залежності для всіх залежностей. Імпортований код буде вставлений замість директиви #import. Далі відбудеться включення всіх бібліотек на C# що додані директивою #include. Після цього у коді спершу скомпілюються всі директиви. А далі вже будть компілюватися весь звичайний код.
</p>
