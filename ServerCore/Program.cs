namespace ServerCore
{
    public class Test()
    {

    }
    internal class Program
    {
        static object lockObj = new object();
        static int datas = 0;
        static void TestThread1()
        {
            lock (lockObj)
            {
                for (int i = 0; i < 10; i++)
                {

                    datas++;
                    Console.WriteLine($"TestThread1 : {datas}");

                }
            }
        }
        static void TestThread2()
        {
            lock (lockObj)
            {
                for (int i = 0; i < 10; i++)
                {

                    Console.WriteLine($"TestThread2 : {datas}");

                }
            }
        }
        static void Main(string[] args)
        {
            Task t1 = new Task(TestThread1);
            Task t2 = new Task(TestThread2);
            t1.RunSynchronously();
            t2.RunSynchronously();

         


            Task.WaitAll(t1, t2);


        }

    }

}
