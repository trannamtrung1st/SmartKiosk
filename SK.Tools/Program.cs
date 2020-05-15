using System;

namespace SK.Tools
{
    class Program
    {
        static void Main(string[] args)
        {
            var generator = new ActivationCodeGenerator();
            generator.Start();
        }
    }
}
