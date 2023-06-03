using System.Collections.Generic;

namespace GlobalRealization;

public delegate void ElementaryFunction(ref ProgramStack stack, Reference? ReturnValue, List<ProgramObject>? args);