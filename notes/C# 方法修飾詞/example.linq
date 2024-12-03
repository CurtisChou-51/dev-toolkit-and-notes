<Query Kind="Program" />

void Main()
{
    ISpeak obj = new Child();
    obj.Speak1();
    ((Parent)obj).Speak1();

    obj.Speak2();
    ((Parent)obj).Speak2(); // 編譯期類型是 Parent，執行的不是 Child 的 Speak2
}

public interface ISpeak
{
    void Speak1();
    void Speak2();
}


public class Parent : ISpeak
{
    public virtual void Speak1()
    {
        Console.WriteLine("Parent speaks 1");
    }
    
    public virtual void Speak2()
    {
        Console.WriteLine("Parent speaks 2");
    }
}

public class Child : Parent, ISpeak
{
    public override void Speak1()
    {
        Console.WriteLine("Child speaks 1");
    }
    
    public new void Speak2()
    {
        Console.WriteLine("Child speaks 2");
    }
}