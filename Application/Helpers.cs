using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application;

public static class Helpers
{
    public static void PrintTimestamp(string message)
    {
        string timeWithMS = DateTime.Now.ToString("HH:mm:ss.fffff");
        Console.WriteLine($"\n{timeWithMS}\t{message}");
    }
}
