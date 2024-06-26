﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Net40.TaskDemo;


public static class RuntimeHelpersEx
{
    internal static ref char GetRawStringData(this String str)
    {
        unsafe
        {
            char* first = (char*)&str;
            char firstCh = first[0];
            return ref first[0];
            /*fixed (void* first = &str)
            {
                return ref Unsafe.AsRef<char>(first);
            }*/
        }
    }
}

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
    public static void Main(string[] args)
    {

        string text = "Hello World";

        ref char first = ref text.GetRawStringData();
        char ff = text[0];

        first = '!';
        
        Console.WriteLine(text);


        /*CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken ct = cts.Token;

        Console.WriteLine($"Main->Start->ThreadId {Thread.CurrentThread.ManagedThreadId}");

        ValueTask<int> task = TaskExWhenAnyTests.RunTest(ct);

        Thread.Sleep(1000);

        Console.WriteLine($"task.Status: {task.AsTask().Status}");

        Thread.Sleep(1000);

        await TaskEx.Run(() => { CreateChanelEvent(cts); } );

        int result = await task;

        Console.WriteLine($"task.Status: {task.AsTask().Status}");

        Console.WriteLine($"result: {result}");

        Console.WriteLine($"Main->End->ThreadId {Thread.CurrentThread.ManagedThreadId}");

        Console.ReadKey(true);*/
    }

    private static void CreateChanelEvent(CancellationTokenSource cts)
    {
        Thread.Sleep(1000);
        cts.Cancel();
    }
}