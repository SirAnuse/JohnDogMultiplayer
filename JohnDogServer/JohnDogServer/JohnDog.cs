using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Threading;
using Console = Colorful.Console;

namespace JohnDogServer
{
    class JohnDog
    {
        public static void SayNoNewLine (string name, string text)
        {
            Console.Write("<" + name + "> ", Color.Orange);
            Console.Write(text, Color.White);
        }
        public static void Say (string name, string text)
        {
            Console.Write("\n<" + name + "> ", Color.Orange);
            Console.Write(text, Color.White);
        }
    }
}
