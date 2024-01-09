<link rel="stylesheet" href="styles.css">




# UA 
<h3>Як розробити власне середовище для запуску програми на VCPL</h3>

<ol>
    <li>
        Необхідно створити екземпляр класу, що реалізує інтерфейс <a href="/Docs/Compiler.md">ICompiler</a>. Наприклад екземпляр <a href="/Docs/Compiler.md">базового компілятору Compiler_IIDL</a>: 
        <pre class="code">ICompilator compilator = new Compilator_IIDL(environment);</pre>
    </li>
    <li>
        Далі слід створити всі необхідні для компіляції <a>компоненти</a> (може різнитися в залежності від обраного вами компілятору та компонентів), а саме згенерувати Enviroment, CompileStack та RuntimeStack зареєструвати в оточенні <a href="/Docs/SyntaxConvertor.md">синтаксичні конвертори</a>: 
        <pre class="code">ReleaseEnvironment releaseEnvironment = new ReleaseEnvironment(vcplLogger); 
releaseEnvironment.SplitCode = (string code) => { return code.Split("\r\n"); }; 
releaseEnvironment.envCodeConvertorsContainer.AddCodeConvertor("CLite", new CLiteConvertor()); 
AbstractEnvironment environment = releaseEnviroment; 
CompileStack cStack = CreateBasicStack(); 
RuntimeStack rtStack = cStack.Pack(); 
environment.RuntimeStack = rtStack;</pre>
    </li>
    <li>
        Останній крок: компіляція функції та її виклик. Рекомендується також викликати очищення кучі від сміття пісял компіляції перед викликом. У виклику функції передати порожній масив вказівників типу <a href="/Docs/CoreComponents.md">IPointer</a>.
        <pre class="code">ElementaryFunction main = compilator.CompilateMain(cStack, code, ChosenSyntax, "main"); 
GC.Collect(); 
GC.WaitForPendingFinalizers(); 
main.Invoke(Array.Empty<IPointer>()); </pre>
    </li>
</ol>



