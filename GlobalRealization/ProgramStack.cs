namespace GlobalRealization;

public class ProgramStack : List<Variable>
{
    public void ChangeVariable(string name, object? newValue)
    {
        for (int i = 0; i < this.Count; i++)
        {
            if (this[i].Name == name)
            {
                this[i].Value = newValue;
                return;
            }
        }

        throw new Exception("The variable was not found");
    }
    public Variable? FindVariable(string name)
    {
        foreach (var variable in this)
        {
            if (variable.Name == name)
            {
                return variable;
            }
        }

        return null;
    }

    public int FindIndex(string name)
    {
        for (int i = 0; i < this.Count; i++)
        {
            if (this[i].Name == name)
            {
                return i;
            }
        }

        return -1;
    }
}

// static realisation of stack
// first should to realize static constants which will saved in real c# stack
