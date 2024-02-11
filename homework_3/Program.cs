namespace TaskSample
{
    internal class Program
    {
        static long Sum(int a, int b)
        {
            return (long)a+b;
        }
        static void Main(string[] args)
        {

            var summaryTask = new Task<long>(()=>Sum(2,2));

            summaryTask.Start();

            var calcTask = new Task<long>(()=>Sum(2,3));

            var sum = summaryTask.Result;

            var taskArray = new Task[]
            {
                new Task(()=>Console.WriteLine("Task1")),
                new Task(()=>Console.WriteLine("Task2")),
                new Task(()=>Console.WriteLine("Task3")),
            };
            foreach(var t  in taskArray)
            {
                t.Start();
            }
            Task.WaitAll(taskArray);


            var outerTask = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Hello from outer task");
                var innerTask = Task.Factory.StartNew(() =>
                {
                    Console.WriteLine("Hello from inner task");
                    Thread.Sleep(1000);
                    Console.WriteLine("Inner task finished");

                }, TaskCreationOptions.AttachedToParent);
                Console.WriteLine("Outer task finished");
            });

            outerTask.Wait();

            // 1-st way of create & run
            var task = new Task(()
                => Console.WriteLine("hello"));
            task.RunSynchronously();

            task.Wait();
            Console.WriteLine("world");

            // 2nd way - can be used in 2 homework !!!  - use static method
            var task2 = Task.Factory.StartNew(() =>
                Console.WriteLine("Hello world"));
            task2.Wait();

            // 3rd way - also uses ststic method as well as 2nd
            var t3 = Task.Run(()=> Console.WriteLine("Hello world last time"));
            t3.Wait();

            // summary: different order of hello...


        }
    }
}