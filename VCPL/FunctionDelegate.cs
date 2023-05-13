namespace VCPL;

public delegate void ElementaryFunction(ref ProgramStack stack, List<ProgramObject>? args);

public delegate void PreProcessorDirective(object? args);
public delegate void RunDelegate(List<Variable> stack, int id); // should be deleted