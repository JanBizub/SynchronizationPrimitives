namespace SynchronisationPrimitives;

internal class AutoResetEventDemo
{
    private readonly AutoResetEvent _firstDone = new(false);
    private readonly AutoResetEvent _secondDone = new(false);

    public void First(Action printFirst)
    {
        printFirst();
        _firstDone.Set(); // Signal that first() is done
    }

    public void Second(Action printSecond)
    {
        _firstDone.WaitOne(); // Wait for first() to complete
        printSecond();
        _secondDone.Set(); // Signal that second() is done
    }
    public void Third(Action printThird)
    {
        _secondDone.WaitOne(); // Wait for second() to complete
        printThird();
    }
}

internal static class AutoResetEventDemoTest
{
    public static void Run()
    {
        Console.WriteLine("AutoResetEventDemoTest");

        AutoResetEventDemo foo = new();

        Action printFirst = () => Console.Write("first");
        Action printSecond = () => Console.Write("second");
        Action printThird = () => Console.Write("third");

        // Example usage with threads in any order:
        var threadA = new Thread(() => foo.First(printFirst));
        var threadB = new Thread(() => foo.Second(printSecond));
        var threadC = new Thread(() => foo.Third(printThird));

        // Start threads in any order to simulate asynchronous execution
        threadB.Start();
        threadC.Start();
        threadA.Start();

        // Wait for all threads to finish
        threadA.Join();
        threadB.Join();
        threadC.Join();
    }
}