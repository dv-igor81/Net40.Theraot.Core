using Net60.TaskDemo;

internal class Program
{
    // 0) TaskStatus.Created - Задача инициализирована, но ещё не запланирована;
    // 1) TaskStatus.WaitingForActivation - Задача ожидает активации и внутреннего планирования инфраструктурой платформы .Net Framework;
    // 2) TaskStatus.WaitingToRun - Задача запланирована на выполнение, но ещё не начала выполняться;
    // 3) TaskStatus.Running - Задача выполняется, но ещё не завершилась;
    // 5) TaskStatus.RanToCompletion - задача успешно завершена;
    // 6) TaskStatus.Canceled - Задача приняла отмену, создав исключение;
    // 7) TaskStatus.Faulted - Задача завершилась из-за необработанного исключения.
    // =============================================================================================================
    // 0) TaskCreationOptions.None - Указывает, что следует использовать поведение по умолчанию;
    // 1) TaskCreationOptions.PreferFairness - Рекомендации для TaskScheduler для планирования задачь максимально прямым способом...;
    // 2) TaskCreationOptions.LongRunning - Указывает, что задача будет длительной подробной операцией. Предоставляет сведения...;
    // 3) TaskCreationOptions.AttachedToParent - Указывает, что задача присоеденена к родительской задаче в иерархии задач.
    public static async Task Main(string[] args)
    {
        CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken ct = cts.Token;
            
        Console.WriteLine($"Main.Start.ThreadId {Thread.CurrentThread.ManagedThreadId}");
            
        ValueTask<int> task = TaskExWhenAnyTests.RunTest(ct);
            
        Thread.Sleep(1000);
            
        Console.WriteLine($"task.Status: {task.AsTask().Status}");
            
        Thread.Sleep(1000);

        // ReSharper disable once MethodSupportsCancellation
        await Task.Run(() => { CreateChanelEvent(cts); } );
            
        int result = await task.ConfigureAwait(false);
            
        Console.WriteLine($"task.Status: {task.AsTask().Status}");
            
        Console.WriteLine($"result: {result}");
            
        Console.WriteLine($"Main.End.ThreadId {Thread.CurrentThread.ManagedThreadId}");

        Console.ReadKey(true);
    }

    private static void CreateChanelEvent(CancellationTokenSource cts)
    {
        Thread.Sleep(1000);
        cts.Cancel();
    }
}