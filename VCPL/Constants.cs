namespace VCPL;

public class Constants<T>
{
    private T data;
    
    public T Data
    {
        get { return data; }
    }
    
    public Constants(T data)
    {
        this.data = data;
    }
}