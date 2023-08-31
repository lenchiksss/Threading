using System.Text;

class Program
{

    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        Random random = new Random();
        const int maxCustomers = 3;
        const int numChairs = 1;
        Semaphore waitingRoom = new Semaphore(numChairs, maxCustomers);
        Semaphore barberChair = new Semaphore(1, 1);
        Semaphore barberSleepChair = new Semaphore(0, 1);
        Semaphore seatBelt = new Semaphore(0, 1);
        bool allDone = false;

        void Barber()
        {
            while (!allDone)
            {
                Console.WriteLine("Парикмахер спить.");
                barberSleepChair.WaitOne();
                if (!allDone)
                {
                    Console.WriteLine("Парикмахер робить стрижку.");
                    Thread.Sleep(random.Next(1, 3) * 1000);
                    Console.WriteLine("Парикмахер доробив стрижку.");
                    seatBelt.Release();
                }
                else
                {
                    Console.WriteLine("Парикмахер спить.");
                }
            }
            return;
        }

        void Customer(Object number)
        {
            int Number = (int)number;
            Console.WriteLine("Клієнт {0} йде в парикмахерську", Number);
            Thread.Sleep(random.Next(1, 5) * 1000);
            Console.WriteLine("Клієнт {0} прийшов", Number);
            waitingRoom.WaitOne();
            Console.WriteLine("Клієнт {0} заходить в кімнату для очікування", Number);
            barberChair.WaitOne();
            waitingRoom.Release();
            Console.WriteLine("Клієнт {0} будить парикмахера", Number);
            barberSleepChair.Release();
            seatBelt.WaitOne();
            barberChair.Release();
            Console.WriteLine("Клієнт {0} уходить з парикмахерської", Number);
        }

        Thread barberThread = new Thread(Barber);
        barberThread.Start();

        List<Thread> customerThreads = new List<Thread>(); 

        for (int i = 1; i <= maxCustomers; i++)
        {
            Thread customerThread = new Thread(new ParameterizedThreadStart(Customer));
            customerThread.Start(i);
            customerThreads.Add(customerThread);
        }

        foreach (Thread customerThread in customerThreads)
        {
            customerThread.Join();
        }
        
        allDone = true;
        barberSleepChair.Release();
        barberThread.Join();
        Console.WriteLine("Кінець робочого дня");
    }
}