namespace VCPL;

public delegate void ElementaryFunction(ref List<Variable> stack, object? args);
public delegate void RunDelegate(List<Variable> stack, int id); // should be deleted