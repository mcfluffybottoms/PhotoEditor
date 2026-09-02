namespace PhotoEditor.Models;

public class RunTask<T>
{
    public Guid Uuid { get; set; }
    public RunTaskStatus Status { get; set; }
    public T? Result { get; set; }
}