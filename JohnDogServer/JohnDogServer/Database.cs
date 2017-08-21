using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;

namespace JohnDogServer
{
    public partial class Database : IDisposable
    {
        private static string _host, _databaseName, _user, _password;
        private readonly MySqlConnection _con;
        public MySqlConnection Connection { get { return _con; } }

        public Database(string host, string database, string user, string password)
        {
            _host = host;
            _databaseName = database;
            _user = user;
            _password = password;

            _con = new MySqlConnection(
                String.Format("Server={0};Database={1};uid={2};password={3};convert zero datetime=True;",
                    host, database ?? "johndog", user ?? "root", password ?? ""));
            _con.Open();
        }

        public Database()
        {
            _con = new MySqlConnection(
                String.Format("Server={0};Database={1};uid={2};password={3};convert zero datetime=True;",
                    _host, _databaseName, _user, _password));
            _con.Open();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_con.State == System.Data.ConnectionState.Open)
                {
                    _con.Close();
                    _con.Dispose();
                }
            }
            //GC.SuppressFinalize(this);//Updated
        }

        public MySqlCommand CreateQuery()
        {
            return _con.CreateCommand();
        }

        public static int DateTimeToUnixTimestamp(DateTime dateTime)
        {
            return (int)(dateTime - new DateTime(1970, 1, 1).ToLocalTime()).TotalSeconds;
        }

        public bool Register(string username, string password)
        {
            try
            {
                MySqlCommand cmd = CreateQuery();
                cmd.CommandText =
                    "INSERT INTO accounts(id, username, password) VALUES (null, '" + username + "', '" + password + "');";
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@password", password);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void UpdateLastSeen(string username)
        {
            string currentDate = DateTime.UtcNow.ToString("yyyy-MM-dd:HH-mm-ss");
            MySqlCommand cmd = CreateQuery();
            cmd.CommandText = "UPDATE accounts SET lastSeen=@lastSeen WHERE username=@username;";
            cmd.Parameters.AddWithValue("@lastSeen", currentDate);
            cmd.Parameters.AddWithValue("@username", username);
            cmd.ExecuteScalar();
        }

       

        public string GetEquipment(string username)
        {
            JohnDog.Say("Database Manager", "Getting equipment for " + username);
            string id = FindIDFromName(username);
            string charname = FindCharNameFromID(id);
            MySqlCommand cmd = CreateQuery();
            cmd.CommandText =
                "SELECT equipment FROM inventories WHERE accId=@accId AND charname=@charname;";
            cmd.Parameters.AddWithValue("@accId", Convert.ToInt32(id));
            cmd.Parameters.AddWithValue("@charname", charname);
            string meme = (string)cmd.ExecuteScalar();
            return meme;
        }

        public void SetEquipment(string username, string equips)
        {
            JohnDog.Say("Database Manager", "Getting inventory for " + username);
            string id = FindIDFromName(username);
            string charname = FindCharNameFromID(id);
            MySqlCommand cmd = CreateQuery();
            cmd.CommandText =
                "UPDATE inventories SET equipment=@equips WHERE accId=@accId AND charname=@charname;";
            cmd.Parameters.AddWithValue("@equips", equips);
            cmd.Parameters.AddWithValue("@accId", id);
            cmd.Parameters.AddWithValue("@charname", charname);
            cmd.ExecuteScalar();
        }

        public string GetInventory (string username)
        {
            JohnDog.Say("Database Manager", "Getting inventory for " + username);
            string id = FindIDFromName(username);
            string charname = FindCharNameFromID(id);
            MySqlCommand cmd = CreateQuery();
            cmd.CommandText =
                "SELECT inventory FROM inventories WHERE accId=@accId AND charname=@charname;";
            cmd.Parameters.AddWithValue("@accId", Convert.ToInt32(id));
            cmd.Parameters.AddWithValue("@charname", charname);
            string meme = (string)cmd.ExecuteScalar();
            return meme;
        }

        public void SetInventory(string username, string inventory)
        {
            JohnDog.Say("Database Manager", "Getting inventory for " + username);
            string id = FindIDFromName(username);
            string charname = FindCharNameFromID(id);
            MySqlCommand cmd = CreateQuery();
            cmd.CommandText =
                "UPDATE inventories SET inventory=@inv WHERE accId=@accId AND charname=@charname;";
            cmd.Parameters.AddWithValue("@inv", inventory);
            cmd.Parameters.AddWithValue("@accId", id);
            cmd.Parameters.AddWithValue("@charname", charname);
            cmd.ExecuteScalar();
        }

        public string FindIDFromName (string username)
        {
            MySqlCommand cmd = CreateQuery();
            cmd.CommandText =
                "SELECT * FROM accounts WHERE username=@username;";
            cmd.Parameters.AddWithValue("@username", username);
            using (MySqlDataReader rdr = cmd.ExecuteReader())
            {
                if (!rdr.HasRows) return null;
                rdr.Read();
                return Convert.ToString(rdr.GetInt32("id"));
            }
        }

        public string FindCharNameFromID(string id)
        {
            MySqlCommand cmd = CreateQuery();
            cmd.CommandText =
                "SELECT * FROM inventories WHERE accId=@accId;";
            cmd.Parameters.AddWithValue("@accId", id);
            using (MySqlDataReader rdr = cmd.ExecuteReader())
            {
                if (!rdr.HasRows) return null;
                rdr.Read();
                return rdr.GetString("charname");
            }
        }

        public bool CheckAccountInUse(string username)
        {
            MySqlCommand cmd = CreateQuery();
            cmd.CommandText = "SELECT inUse FROM accounts WHERE username=@username;";
            cmd.Parameters.AddWithValue("@username", username);
            using (MySqlDataReader rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    int accInUse = rdr.GetInt32("inUse");
                    if (accInUse == 1) return true;
                    else return false;
                }
            }
            return true;
        }

        public void LockAccount(string username)
        {
            if (username == null) return;
            MySqlCommand cmd = CreateQuery();
            cmd.CommandText = "UPDATE accounts SET inUse=1 WHERE username=@username;";
            cmd.Parameters.AddWithValue("@username", username);
            cmd.ExecuteScalar();
        }

        public void SetCodeRedeemed(string gcode, string username)
        {
            if (username == null || gcode == null) return;
            MySqlCommand cmd = CreateQuery();
            cmd.CommandText = "UPDATE giftcodes SET redeemed=1, redeemedBy=@username WHERE code=@gcode;";
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@gcode", gcode);
            cmd.ExecuteScalar();
        }

        public bool CheckCodeExists (string gcode)
        {
            MySqlCommand cmd = CreateQuery();
            cmd.CommandText =
                "SELECT * FROM giftcodes WHERE code=@code;";
            cmd.Parameters.AddWithValue("@code", gcode);
            using (MySqlDataReader rdr = cmd.ExecuteReader())
            {
                if (!rdr.HasRows) return false;
                rdr.Read();
                JohnDog.Say("Database", "SEARCHING FOR CODE!");
                if (rdr.GetString("code") == gcode)
                {
                    JohnDog.Say("Database", "Code found.");
                    return true;
                }
                else
                {
                    JohnDog.Say("Database", "No code found!");
                    return false;
                }
            }
        }

        public string GetGiftCodeContents (string gcode)
        {
            int gold = 0;
            int xp = 0;
            MySqlCommand cmd = CreateQuery();
            cmd.CommandText =
                "SELECT * FROM giftcodes WHERE code=@code;";
            cmd.Parameters.AddWithValue("@code", gcode);
            using (MySqlDataReader rdr = cmd.ExecuteReader())
            {
                rdr.Read();
                if (rdr.GetInt32("gold") > 0 && rdr.GetInt32("xp") > 0)
                {
                    gold = rdr.GetInt32("gold");
                    xp = rdr.GetInt32("xp");
                    return gold + " " + xp;
                }
                else return "null";
            }
        }

        public void UnlockAccount(string username)
        {
            if (username == null) return;
            MySqlCommand cmd = CreateQuery();
            cmd.CommandText = "UPDATE accounts SET inUse=0 WHERE username=@username;";
            cmd.Parameters.AddWithValue("@username", username);
            cmd.ExecuteScalar();
        }

        public bool Login(string username, string password)
        {
            MySqlCommand cmd = CreateQuery();
            cmd.CommandText =
                "SELECT * FROM accounts WHERE username=@username AND password=@password;";
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@password", password);
            using (MySqlDataReader rdr = cmd.ExecuteReader())
            {
                if (!rdr.HasRows) return false;
                rdr.Read();
                if (rdr.GetString("username") == username && rdr.GetString("password") == password) return true;
                else return false;
            }
        }
    }
}
