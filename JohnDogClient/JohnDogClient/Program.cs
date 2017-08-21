using System;
using System.IO;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Drawing;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Diagnostics;
using System.Timers;
using System.Text.RegularExpressions;
using Console = Colorful.Console;

namespace JohnDogClient
{
    class JohnDogClient
    {
        public static Dictionary<int, Item> Items = new Dictionary<int, Item>();
        public static string[] MessageHistory = new string[10];
        public static int messageNumb = 0;
        private static string username = "";
        private static string password = "";
        private static bool loginregister = true;
        private static string serverrspns = "";
        private static string version = "0.0.1";
        public static string messageAction = "";
        private static TcpClient client = new TcpClient();

        private static void StartMessageHandler()
        {
            Thread messageThread = new Thread(new ThreadStart(HandleData));
            messageThread.Start();
        }

        public static void Main()
        {

            try
            {
                Swords.SetSwords();
                Shields.SetShields();
                SayNoNewLine("Client", "Welcome to John Dog!");
                Say("Client", "Just going to perform a version check with the server.");
                client.Connect("127.0.0.1", 8000);
                // use the ipaddress as in the server program
                // Port 8000 for game server

                if (!DoVersionCheck(client, version))
                {
                    Say("Client", "Client outdated! Server replied: " + serverrspns);
                    Console.WriteLine("Press any key to exit John Dog.", Color.Yellow);
                    Console.ReadKey();
                    return;
                }
                bool msg = false;
                Thread.Sleep(450);
                Say("Client", "Connected!");
                Thread.Sleep(1950);
                while (loginregister)
                {
                    Console.Clear();
                    SayNoNewLine("Client", "Do you want to login, or register?\n");
                    string input = Console.ReadLine();
                    switch (input.ToLower())
                    {
                        case "nigga":
                        case "log":
                        case "login":
                            Console.Clear();
                            SayNoNewLine("Client", "Username: ");
                            username = Console.ReadLine();
                            SayNoNewLine("Client", "Password: ");
                            password = Console.ReadLine();
                            string response = AttemptLogin(client, username, password);
                            if (response == "SUCCESS")
                            {
                                Console.Clear();
                                SayNoNewLine("Client", "Login successful!");
                                Console.ReadKey();
                                msg = true;
                                loginregister = false;
                            }
                            else if (response == "INUSE")
                            {
                                Console.Clear();
                                SayNoNewLine("Client", "Account is currently in use!");
                                Console.ReadKey();
                            }
                            else if (response == "INVALID CREDENTIALS")
                            {
                                Console.Clear();
                                SayNoNewLine("Client", "Username or password was incorrect!");
                                Console.ReadKey();
                            }
                            break;
                        case "register":
                        case "reg":
                        case "new":
                        case "create":
                            Console.Clear();
                            SayNoNewLine("Client", "Username: ");
                            username = Console.ReadLine();
                            SayNoNewLine("Client", "Password: ");
                            password = Console.ReadLine();
                            if (AttemptRegistration(client, username, password))
                            {
                                Console.Clear();
                                Say("Client", "Success registering!");
                                Console.ReadKey();
                            }
                            else
                            {
                                Console.Clear();
                                Say("Client", "Username or password was incorrect!");
                                Console.ReadKey();
                            }
                            break;
                        default:
                            Console.Clear();
                            SayNoNewLine("Client", "Please enter a valid command.");
                            Console.ReadKey();
                            break;
                    }

                }
                Say("Client", "Starting message handler...\n");
                Thread.Sleep(500);
                StartMessageHandler();
                while (msg)
                {
                    bool anyMessages = false;
                    Console.Clear();
                    for (int i = 0; i < MessageHistory.Length - 1; i++)
                    {
                        if (MessageHistory[i] == null) Thread.Sleep(0);
                        else
                        {
                            anyMessages = true;
                            Console.WriteLine(MessageHistory[i], Color.Turquoise);
                        }
                    }
                    if (anyMessages) Console.Write("\n");
                    SayNoNewLine("Client", "Type 'message <name> <msg>' to message another player.\n");
                    string meme = Console.ReadLine();
                    SendData(client, meme, false);
                }
            }

            catch (Exception e)
            {
                Say("Client", "Argh! The server's offline!");
                Console.WriteLine("\nError: " + e, Color.Red);
                Console.WriteLine("\nPress any key to exit John Dog.", Color.Yellow);
                Console.ReadKey();
            }
        }

        public static void SayNoNewLine(string name, string text)
        {
            Console.Write("<" + name + "> ", Color.Orange);
            Console.Write(text, Color.White);
        }

        public static void Say(string name, string text)
        {
            Console.Write("\n<" + name + "> ", Color.Orange);
            Console.Write(text, Color.White);
        }

        public static void HandleData()
        {
            Thread.Sleep(500);
            bool receivingData = true;
            SayNoNewLine("Data Handler", "Data handler is running!\n");
            while (receivingData)
            {
                ReceiveData(client);
            }
        }

        public static void Drop(int slot)
        {

        }

        public static string[] ConvertToCMDs(string entry)
        {
            string[] cmds = Regex.Split(entry, "\\s+");
            return cmds;
        }

        public static void SendInventoryListRequest(TcpClient client, string lsd)
        {
            string[] msg = ConvertToCMDs(lsd);
            string Message = String.Empty;
            for (int i = 2; i < msg.Length; i++) Message += " " + msg[i];
            SendData(client, "INVENTORY LIST", true);
        }

        public static void SendMessage(TcpClient client, string lsd)
        {
            string[] msg = ConvertToCMDs(lsd);
            string Message = String.Empty;
            for (int i = 2; i < msg.Length; i++) Message += " " + msg[i];
            SendData(client, "MESSAGE " + msg[1] + Message, true);
        }

        public static void AddToMessageHistory(string message)
        {
            // Increase message counter by 1
            messageNumb++;

            int msgToRemove = messageNumb - 1;

            // Add the actual message to message history
            // messageNumb subtract 1 because the array starts at 0, not 1
            MessageHistory[messageNumb - 1] = message;

            // Add 8 second timer
            System.Timers.Timer timer = new System.Timers.Timer(8000);

            // Set the value to null

        }

        public static int FindArrayNumbFromString(string[] tosearch, string tofind)
        {
            for (int i = 0; i < tosearch.Length - 1; i++)
            {
                if (tosearch[i] == tofind)
                    return i;
            }
            return 0;
        }

        public static void HandleGiftCode(string data)
        {
            // Convert data to an array separated by spaces
            string[] CodeArray = ConvertToCMDs(data);

            // [0] is 'REDEEM:SUCCESS or REDEEM:FAILURE'
            // [1] is the code
            // [2] is the gold amount
            // [3] is the xp amount

            Thread.Sleep(50);
            if (CodeArray[0] == "REDEEM:FAILURE")
            {
                Console.Clear();
                SayNoNewLine("Gift Code Manager", "Invalid gift code: " + CodeArray[1]);
                Console.ReadKey();
            }
            else if (CodeArray[0] == "REDEEM:NOCODE")
            {
                Console.Clear();
                SayNoNewLine("Gift Code Manager", "Enter a gift code!");
                Console.ReadKey();
            }
            else
            {
                string code = CodeArray[1];

                // Add to Message History
                Console.Clear();
                SayNoNewLine("Gift Code Manager", "Gift code redeem success!");
                Say("Gift Code Manager", "Gift code: " + code);
                Say("Gift Code Manager", "Contents: Gold - " + CodeArray[2] + ", XP - " + CodeArray[3]);
                Console.ReadKey();
            }
        }

        public static void HandleMessage(string data)
        {
            // Convert data to an array separated by spaces
            string[] MessageArray = ConvertToCMDs(data);

            // [0] is 'MESSAGE'
            // [1] is the username of the user which sent it
            // [2] and onwards is the message itself

            string user = MessageArray[1];
            string message = String.Empty;

            // Turn the message in to a single string
            for (int i = 2; i < MessageArray.Length; i++) message += " " + MessageArray[i];

            // Add to Message History
            AddToMessageHistory("<" + user + ">" + message);
        }

        public static string ReceiveData(TcpClient client)
        {
            byte[] bb = new byte[100];
            int k = 0;
            Stream stm = client.GetStream();
            bb = new byte[100];
            k = stm.Read(bb, 0, 100);
            string parser = "";
            bool inv = false;
            bool gcode = false;
            bool msg = false;
            for (int i = 0; i < k; i++)
            {
                parser += Convert.ToChar(bb[i]);
                if (parser == "REDEEM") gcode = true;
                if (parser == "MESSAGE ") msg = true;
                if (parser == "INVENTORY:LIST") inv = true;
            }
            if (gcode) HandleGiftCode(parser);
            else if (msg) HandleMessage(parser);
            else if (inv) ParseInventory(parser);
            return parser;
        }

        public static void ParseInventory(string data)
        {
            // Convert data to an array separated by spaces
            string[] Inventory = ConvertToCMDs(data);

            // [0] is 'INVENTORY:LIST'
            // [1] is the username of the user which sent it
            // [2] and onwards is the message itself

            int[] inventory = new int[15];


            // Convert inventory to int array
            for (int i = 1; i < 12; i++)
            {
                int meme = 0;
                if (Int32.TryParse(Inventory[i], out meme))
                {
                    inventory[i] = meme;
                }
            }


            // List inventory
            for (int i = 0; i < 13; i++)
            {
                if (i == 0)
                {
                    if (inventory[i] == -1) Say("Slot " + i + " (Weapon Slot)", "Empty");
                    else Say("Slot " + i + " (Weapon Slot)", Items[inventory[i]].Name);
                }
                else if (i == 1)
                {
                    if (inventory[i] == -1) Say("Slot " + i + " (Ability Slot)", "Empty");
                    else Say("Slot " + i + " (Ability Slot)", Items[inventory[i]].Name);
                }
                else if (i == 2)
                {
                    if (inventory[i] == -1) Say("Slot " + i + " (Armor Slot)", "Empty");
                    else Say("Slot " + i + " (Armor Slot)", Items[inventory[i]].Name);
                }
                else if (i == 3)
                {
                    if (inventory[i] == -1) Say("Slot " + i + " (Ring Slot)", "Empty");
                    else Say("Slot " + i + " (Ring Slot)", Items[inventory[i]].Name);
                }
                else
                {
                    if (inventory[i] == -1) Say("Slot " + i, "Empty");
                    else Say("Slot " + i, Items[inventory[i]].Name);
                }
            }
        }

        public static void SendData(TcpClient client, string data, bool fromSendMessage)
        {
            if (fromSendMessage) Thread.Sleep(0);
            else
            {
                string[] dataToArray = ConvertToCMDs(data);
                if (dataToArray[0] == "MESSAGE") SendMessage(client, data);
                else if (dataToArray[0] == "view" && dataToArray[1] == "inventory") SendInventoryListRequest(client, data);
            }
            Stream stm = client.GetStream();
            UTF8Encoding asen = new UTF8Encoding();
            byte[] ba = asen.GetBytes(data);
            stm.Write(ba, 0, ba.Length);
        }

        public static string AttemptLogin(TcpClient client, string user, string pass)
        {
            string data = "";
            SendData(client, "LOGIN REQUEST " + user + " " + pass, false);
            data = ReceiveData(client);
            if (data == "SUCCESS:LOGIN") return "SUCCESS";
            else if (data == "FAILURE:INUSE")
            {
                return "INUSE";
            }
            else return "INVALID CREDENTIALS";
        }

        public static bool AttemptRegistration(TcpClient client, string user, string pass)
        {
            string data = "";
            SendData(client, "REGISTER REQUEST " + user + " " + pass, false);
            data = ReceiveData(client);
            if (data == "SUCCESS:REGISTER") return true;
            else return false;
        }

        public static bool DoVersionCheck(TcpClient client, string version)
        {
            string data = "";
            SendData(client, "VERSION CHECK " + version, false);
            data = ReceiveData(client);
            if (data == "VERSION IS " + version) return true;
            else
            {
                client.Close();
                serverrspns = data;
                return false;
            }
        }
    }
}
