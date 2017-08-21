using System;
using System.Text;
using System.Net;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Drawing;
using System.Text.RegularExpressions;
using JohnDogServer;
using Console = Colorful.Console;

public class Server
{
    public static string latestcommand = "";
    internal static Database db { get; set; }
    public static string version = "0.0.1";
    public static int clientNo = 0;
    private static bool truth = true;
    public static Dictionary<int, Socket> clients = new Dictionary<int, Socket>();
    public static Dictionary<string, Socket> clientNames = new Dictionary<string, Socket>();
    private static TcpListener server = new TcpListener(IPAddress.Any, 8000);
    private static void StartServer()
    {
        Thread listenThread = new Thread(new ThreadStart(ListenForClients));
        listenThread.Start();
    }
    public static void Main()
    {
        try
        {
            db = new Database("127.0.0.1", "johndog", "root", "");
            StartServer();
            // use local m/c IP address, and 
            // use the same in the client

            /* Initializes the Listener */
            

            /* Start Listeneting at the specified port */

            JohnDog.SayNoNewLine("Connection Manager", "The server is running at port 8000.");
            JohnDog.Say("Connection Manager", "The local IP address is: " +
                              server.LocalEndpoint);
            JohnDog.Say("Connection Manger", "Waiting for a connection...");

        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.StackTrace);
            Console.ReadKey();
        }
    }

    private static void ListenForClients()
    {
        server.Start();
        while (truth)
        {
            Socket client = server.AcceptSocket();

            clientNo++;
            clients.Add(clientNo, client);
            
            Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
            
            clientThread.Start(client);
        }
    }

    private static void HandleVersionCheck(object joe)
    {
        Thread.Sleep(25);
        int joeNo = (int)joe;
        JohnDog.Say("Client Manager", "Client is requesting version [ID: " + joeNo + "] - sent: " + "VERSION " + version);
    }

    public static string[] ConvertToCMDs(string entry)
    {
        string[] cmds = Regex.Split(entry, "\\s+");
        return cmds;
    }

    public static void Message (string username, string username1, string message)
    {
        UTF8Encoding asen = new UTF8Encoding();
        if (db.CheckAccountInUse(username) && db.CheckAccountInUse(username1))
        {
            if (username1 == username) clientNames[username].Send(asen.GetBytes("MESSAGE:SELF"));
            clientNames[username].Send(asen.GetBytes("MESSAGESENT " + username1 + " " + message));
            clientNames[username1].Send(asen.GetBytes("MESSAGE " + username + " " + message));
        }
        else
        {
            JohnDog.Say("Message Manager", "The target username is offline.");
            clientNames[username].Send(asen.GetBytes("FAILURE:MSG"));
            return;
        }
    }

    private static void HandleClientComm(object client)
    {
        string username = "";
        bool isCheckingVersion = false;
        Thread versionCheck = new Thread(new ParameterizedThreadStart(HandleVersionCheck));
        bool john = true;
        int joe = clientNo;
        Socket s = (Socket)client;
        JohnDog.Say("Client Manager", "Client received [ID: " + joe + "] - " + clients[joe].RemoteEndPoint);
        while (john)
        {
            UTF8Encoding asen = new UTF8Encoding();
            byte[] b = new byte[100];
            int k = 0;
            try { k = s.Receive(b); }
            catch
            {
                john = false;
                JohnDog.Say("Client Manager", "Client disconnected [ID: " + joe + "] - " + clients[joe].RemoteEndPoint);
                clientNames.Remove(username);
                try { db.UnlockAccount(username); }
                catch { }
                return;
            }
            string action = "";
            string thing = "";
            for (int i = 0; i < k; i++)
            {
                // Convert received data to string after printing
                thing += Convert.ToChar(b[i]);
                if (thing == "LOGIN REQUEST") action = "LOGIN REQUEST";
                else if (thing.ToUpper() == "REDEEM") action = "REDEEM";
                else if (thing == "REGISTER REQUEST") action = "REGISTER REQUEST";
                else if (thing == "VERSION CHECK") action = "VERSION CHECK";
                else if (thing.ToUpper() == "MESSAGE") action = "MESSAGE";
                else if (thing.ToUpper() == "INVENTORY ") action = "INVENTORY";
            }
            JohnDog.Say("Command Manager", "Command: " + action.ToUpper());
            latestcommand = thing;
            if (action == "LOGIN REQUEST")
            {
                string[] cmds = ConvertToCMDs(thing);
                if (db.Login(cmds[2], cmds[3]) && db.CheckAccountInUse(cmds[2]))
                {
                    JohnDog.Say("Database", "Account in use!");
                    s.Send(asen.GetBytes("FAILURE:INUSE"));
                }
                else if (db.Login(cmds[2], cmds[3]))
                {
                    clientNames.Add(cmds[2], s);
                    username = cmds[2];
                    db.LockAccount(cmds[2]);
                    s.Send(asen.GetBytes("SUCCESS:LOGIN"));
                }
                else
                    s.Send(asen.GetBytes("FAILURE:LOGIN"));
            }
            else if (action == "REGISTER REQUEST")
            {
                string[] cmds = ConvertToCMDs(thing);
                if (db.Register(cmds[2], cmds[3]))
                {
                    JohnDog.Say("Database", "Client registered user " + cmds[2] + " [ID: " + joe + "] - " + clients[joe].RemoteEndPoint);
                    s.Send(asen.GetBytes("SUCCESS:REGISTER"));
                }
                else s.Send(asen.GetBytes("FAILURE:REGISTER"));
            }
            else if (action == "MESSAGE")
            {
                string[] cmds = ConvertToCMDs(thing);
                string Messager = String.Empty;
                for (int i = 2; i < cmds.Length; i++) Messager += " " + cmds[i];
                Message(username, cmds[1], Messager);
            }
            else if (action == "VERSION CHECK")
            {
                HandleVersionCheck(joe);
                s.Send(asen.GetBytes("VERSION IS " + version));
            }
            else if (action.ToUpper() == "REDEEM")
            {
                string[] cmds = ConvertToCMDs(thing);
                string contents = String.Empty;
                try
                {
                    if (db.CheckCodeExists(cmds[1]))
                    {
                        db.SetCodeRedeemed(cmds[1], username);
                        JohnDog.Say("Database", "Successful redeem");
                        contents = db.GetGiftCodeContents(cmds[1]);
                        s.Send(asen.GetBytes("REDEEM:SUCCESS " + cmds[1] + " " + contents));
                    }
                    else
                    {
                        JohnDog.Say("Database", "Failed redeem");
                        s.Send(asen.GetBytes("REDEEM:FAILURE " + cmds[1]));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex, Color.Red);
                    s.Send(asen.GetBytes("REDEEM:NOCODE"));
                }
            }
            else if (action.ToUpper() == "INVENTORY")
            {
                string[] cmds = ConvertToCMDs(thing);
                if (cmds[1] == "LIST")
                {
                    string equips = db.GetEquipment(username);
                    string inv = db.GetInventory(username);
                    if (equips == null) JohnDog.Say("Inventory List", "equips null??");
                    if (inv == null) JohnDog.Say("Inventory List", "inv null??");
                    JohnDog.Say("Inventory List", "Sending - INVENTORY:LIST " + equips + " " + inv);
                    s.Send(asen.GetBytes("INVENTORY:LIST " + equips + " " + inv));
                }
                else if (cmds[1] == "DROP")
                {

                }
                else if (cmds[1] == "EQUIP")
                {

                }
            }
            if (isCheckingVersion) s.Send(asen.GetBytes("VERSION " + version));
            if (latestcommand == "") JohnDog.Say("Client Manager", "Client sent empty byte [ID: " + joe + "] - " + clients[joe].RemoteEndPoint);
            else JohnDog.Say("Client Manager", "Client command received [ID: " + joe + "] - " + latestcommand);
        }
    }
}
