namespace Net60.TaskDemo
{
    public static class TaskExWhenAnyTests
    {
        public static async ValueTask<int> RunTest(CancellationToken ct)
        {
            Task<int> taskReceive = ReceiveAsync(ct);
            Task<int> taskChanel = ChanelAsync(ct);
            Task<int> taskResult = await Task.WhenAny(taskReceive, taskChanel);
            return await taskResult;
        }
        
        private static Task<int> ChanelAsync(CancellationToken ct)
        {
            SimpleStartHelper ssh = new SimpleStartHelper(ct);
            Task<int> task = Task.Run(ssh.Execute);
            //Task<int> task = Task<int>.Factory.FromAsync(BeginAsync, EndAsync, ct, "Hello Igor!");
            return task;
        }
        
        private static Task<int> ReceiveAsync(CancellationToken ct)
        {
            Task<int> task = Task<int>.Factory.FromAsync(BeginAsync, EndAsync, ct, "Hello Igor!");
            return task;
        }

        private class SimpleStartHelper
        {
            private readonly CancellationToken _ct;
            public SimpleStartHelper(CancellationToken ct)
            {
                _ct = ct;
                _ct.Register(TokenChanel);
            }
            
            public int Execute()
            {
                Console.WriteLine($"Execute:Start->ThreadId {Thread.CurrentThread.ManagedThreadId}");
                while (true)
                {
                    if (_ct.IsCancellationRequested)
                    {
                        break;
                    }
                    Thread.Sleep(100);
                }
                Thread.Sleep(1000);
                Console.WriteLine($"Execute:End->ThreadId {Thread.CurrentThread.ManagedThreadId}");
                return 777;
            }
            
            private void TokenChanel()
            {
                Console.WriteLine($"TokenChanel->ThreadId {Thread.CurrentThread.ManagedThreadId}");
            }
        }

        private static Func<CancellationToken, AsyncCallback, object, IAsyncResult> BeginAsync = _beginAsync;

        private static Func<IAsyncResult, int> EndAsync = _endAsync;
        
        private class ThreadStartHelper
        {
            private readonly CancellationToken _ct;
            private readonly AsyncCallback _asyncCallback;
            private readonly AsyncResultImpl _result;
            private bool _creatResultFlag;
            
            public ThreadStartHelper(
                CancellationToken ct,
                AsyncCallback asyncCallback)
            {
                _ct = ct;
                _asyncCallback = asyncCallback;
                _result = new AsyncResultImpl();
                _creatResultFlag = false;
                //_ct.Register(TokenChanel);
            }
            
            public void Execute()
            {
                Console.WriteLine($"Execute:Start->ThreadId {Thread.CurrentThread.ManagedThreadId}");
                while (true)
                {
                    if (_ct.IsCancellationRequested)
                    {
                        break;
                    }
                    Thread.Sleep(100);
                }
                CreatResult();
                Thread.Sleep(1000);
                Console.WriteLine($"Execute:End->ThreadId {Thread.CurrentThread.ManagedThreadId}");
            }

            private void TokenChanel()
            {
                Console.WriteLine($"TokenChanel->ThreadId {Thread.CurrentThread.ManagedThreadId}");
                CreatResult();
            }

            private void CreatResult()
            {
                lock (this)
                {
                    if (_creatResultFlag == false)
                    {
                        _creatResultFlag = true;
                        _result.SetIsCompleted();
                        _asyncCallback(_result);
                        Console.WriteLine($"CreatResult->ThreadId {Thread.CurrentThread.ManagedThreadId}");
                    }
                }
            }

            public IAsyncResult Result => _result; 
            
            private class AsyncResultImpl : IAsyncResult
            {
                private bool _isCompleted;

                public void SetIsCompleted()
                {
                    _isCompleted = true;
                }

                public bool IsCompleted => _isCompleted; //{ get; }
                public WaitHandle AsyncWaitHandle { get; }
                public object AsyncState { get; }
                public bool CompletedSynchronously { get; }
            }
        }

        private static IAsyncResult _beginAsync(CancellationToken ct, AsyncCallback asyncCallback, object state)
        {
            ThreadStartHelper threadStartHelper = new ThreadStartHelper(ct, asyncCallback);
            Thread thread = new Thread(threadStartHelper.Execute)
            {
                IsBackground = true
            };
            thread.Start();
            return threadStartHelper.Result;
        }
        
        
        private static int _endAsync(IAsyncResult asyncResult)
        {
            Console.WriteLine($"EndAsync->ThreadId {Thread.CurrentThread.ManagedThreadId}");
            return 555;
        }
    }
}