using Newtonsoft.Json;
using NSPlang;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

//rewrite socketCode with https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.socketasynceventargs?view=netcore-2.1#remarks

namespace AutismePlangServer
{
    internal class WebSocketServer
    {
        public static readonly HttpClient client = new HttpClient();
        public static Boolean CheckRunning = false;
        public static int NumberofScreens;
        public static NSPlang.JsonRes plang;

        //public static string IniFileName = "Config.ini";
        //public static FileIniDataParser IniParser;
        public static DateTime plangDate = DateTime.Now;

        public static HttpResponseMessage response;
        public static IDictionary<string, int> ScreensClients = new Dictionary<string, int>();
        public static string ServerIPAdress = "127.0.0.1";
        private static byte[] _buffer = new byte[2048];
        private static IDictionary<string, Socket> clients = new Dictionary<string, Socket>();

        //      Action<String> ServerIpLabelSetter;
        public static bool poweron = false;

        public WebSocketServer()
        {
        }

        public static void CheckClientsStillConnected()
        {
            for (int i = 1; i < clients.Count; i++)
            {
                try { 
                    bool p1 = clients.ElementAt(i).Value.Poll(1000, SelectMode.SelectWrite);
                    bool p2 = (clients.ElementAt(i).Value.Available == 0);
                    if (p1 && p2)
                    {
                        //
                    }
                    else
                    {
                        clients.ElementAt(i).Value.Shutdown(SocketShutdown.Both);
                        clients.ElementAt(i).Value.Close();
                        clients.ElementAt(i).Value.Dispose();
                        string myKey = clients.FirstOrDefault(x => x.Value == clients.ElementAt(i).Value).Key;
                        clients.Remove(myKey);
                    }
                } catch (Exception ex)
                { Console.WriteLine("Error: " + ex.Message); }
            }
        }

        public async static void CheckForExternalUpdate(Boolean dontpush = false)
        {
            if (CheckRunning) return;

            if (DateTime.Now.DayOfYear > 355) return;
            if (DateTime.Now.DayOfYear < 7) return;

            if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday || DateTime.Now.DayOfWeek == DayOfWeek.Sunday) return;

            //PowerOn
            /*  if (!poweron)
              {*/
            if (((DateTime.Now.Hour >= 7 && DateTime.Now.Minute >= 25) || DateTime.Now.Hour >= 8) && DateTime.Now.Hour < 17)
            {
                //Console.WriteLine("Powering on");
                poweron = await ToggleScreens(true);
                //poweron = true;
            }
            //   }
            //PowerOff
            /*     if (poweron)
                 {*/
            if ((DateTime.Now.Hour >= 17) && (DateTime.Now.Minute >= 25 || (DateTime.Now.Hour >= 18)))
            {
                //Console.WriteLine("Powering off");
                if (DateTime.Now.DayOfYear == 115)
                {
                    if (DateTime.Now.Hour >= 22) { await ToggleScreens(false); }
                } else poweron = await ToggleScreens(false);
                //poweron = false;
            }
            //   }

            //if ((DateTime.Now.Hour >= 9) && (DateTime.Now.Minute >= 45 || (DateTime.Now.Hour >= 10)) && (plangDate <= DateTime.Now))
            if ((DateTime.Now.Hour >= 15) && (DateTime.Now.Minute >= 45 || (DateTime.Now.Hour >= 16)) && (plangDate <= DateTime.Now))
            {
                await GetNextWorkDay();
                Console.WriteLine("Date changed!!!");
            }
            //Console.Write("Check for external update: ");
            var values = new Dictionary<string, string>
            {
                { "serverip", ServerIPAdress }
            };
            var content = new FormUrlEncodedContent(values);
            try
            {
                CheckRunning = true;
                response = await client.PostAsync("http://intern.autisme.lu/remote/checkForUpdate.ajax.php", content);
                String responseString = await response.Content.ReadAsStringAsync();

                Response resp = JsonConvert.DeserializeObject<Response>(responseString);
                if (resp.Message == "1")
                {
                    if (dontpush)
                    {
                        Console.WriteLine("Update found, but no pushing");
                        await ReadPlangFromServer();
                    }
                    else
                    {
                        Console.WriteLine("Update found, pushing");
                        await ReadPlangFromServerAndPush();
                    }
                }
                else CheckRunning = false; return; //Console.WriteLine("no update needed");
            }
            catch (Exception e)
            {
                Console.WriteLine("CheckForExternalUpdate Error: " + e.Message);
                CheckRunning = false;
            }
        }

        public async static Task<DateTime> GetNextWorkDay()
        {
            var values = new Dictionary<string, string>();
            var content = new FormUrlEncodedContent(values);
            response = await client.PostAsync("http://intern.autisme.lu/remote/getNextDay.ajax.php", content);
            String responseString = await response.Content.ReadAsStringAsync();
            if (responseString.ToString() != null)
            {
                var _content = JsonConvert.DeserializeObject<Content>(responseString);
                plangDate = (DateTime)_content.Date;
                await ReadPlangFromServerAndPush();
                return (DateTime)_content.Date;
            }
            else
            {
                plangDate = DateTime.Now;
                return DateTime.Now;
            }
        }

        public static void ReadPlang()
        {
            String jsonstring = "{\"Date\":\"2018-09-03\",\"Services\":[{\"name\":\"infomedia\",\"c\":3,\"auerzait\":\"13:15 - 14:15\",\"encadrantsMoies\":[{\"id\":159,\"numm\":\"Jill\"},{\"id\":101,\"numm\":\"Paulchen\"}],\"usagersMoies\":[{\"id\":159,\"numm\":\"Jill\"},{\"id\":159,\"numm\":\"Jill\"},{\"id\":159,\"numm\":\"Jill\"},{\"id\":159,\"numm\":\"Jill\"},{\"id\":159,\"numm\":\"Jill\"},{\"id\":101,\"numm\":\"Paulchen\"}],\"encadrantsMettes\":[{\"id\":159,\"numm\":\"Jill\"},{\"id\":101,\"numm\":\"Paulchen\"}],\"usagersMettes\":[{\"id\":159,\"numm\":\"Jill\"},{\"id\":159,\"numm\":\"Jill\"},{\"id\":159,\"numm\":\"Jill\"},{\"id\":159,\"numm\":\"Jill\"},{\"id\":101,\"numm\":\"Paulchen\"}]},{\"c\":3,\"name\":\"backoffice\",\"auerzait\":\"12:00 - 13:00\",\"encadrantsMoies\":[{\"id\":159,\"numm\":\"Jill\"},{\"id\":101,\"numm\":\"Paulchen\"}],\"usagersMoies\":[{\"id\":159,\"numm\":\"Jill\"},{\"id\":159,\"numm\":\"Jill\"},{\"id\":159,\"numm\":\"Jill\"},{\"id\":159,\"numm\":\"Jill\"},{\"id\":159,\"numm\":\"Jill\"},{\"id\":101,\"numm\":\"Paulchen\"}],\"encadrantsMettes\":[{\"id\":159,\"numm\":\"Jill\"},{\"id\":101,\"numm\":\"Paulchen\"}],\"usagersMettes\":[{\"id\":159,\"numm\":\"Jill\"},{\"id\":159,\"numm\":\"Jill\"},{\"id\":159,\"numm\":\"Jill\"},{\"id\":159,\"numm\":\"Jill\"},{\"id\":101,\"numm\":\"Paulchen\"}]},{\"name\":\"gaart\",\"c\":4,\"auerzait\":\"12:00 - 13:00\",\"encadrantsMoies\":[{\"id\":159,\"numm\":\"Jill\"},{\"id\":101,\"numm\":\"Paulchen\"}],\"usagersMoies\":[{\"id\":159,\"numm\":\"Jill\"},{\"id\":159,\"numm\":\"Jill\"},{\"id\":159,\"numm\":\"Jill\"},{\"id\":159,\"numm\":\"Jill\"},{\"id\":159,\"numm\":\"Jill\"},{\"id\":101,\"numm\":\"Paulchen\"}],\"encadrantsMettes\":[{\"id\":159,\"numm\":\"Jill\"},{\"id\":101,\"numm\":\"Paulchen\"}],\"usagersMettes\":[{\"id\":159,\"numm\":\"Jill\"},{\"id\":159,\"numm\":\"Jill\"},{\"id\":159,\"numm\":\"Jill\"},{\"id\":159,\"numm\":\"Jill\"},{\"id\":101,\"numm\":\"Paulchen\"}]},{\"name\":\"waescherei\",\"c\":1,\"auerzait\":\"12:00 - 13:00\",\"encadrantsMoies\":[{\"id\":159,\"numm\":\"Jill\"}],\"usagersMoies\":[{\"id\":159,\"numm\":\"Jill\"},{\"id\":159,\"numm\":\"Jill\"},{\"id\":159,\"numm\":\"Jill\"}],\"encadrantsMettes\":[{\"id\":159,\"numm\":\"Jill\"}],\"usagersMettes\":[{\"id\":159,\"numm\":\"Jill\"},{\"id\":159,\"numm\":\"Jill\"}]},{\"name\":\"Krank\",\"c\":5,\"Moies\":[{\"id\":159,\"numm\":\"Jill\"},{\"id\":159,\"numm\":\"Jill\"}],\"Mettes\":[{\"id\":159,\"numm\":\"Jill\"},{\"id\":159,\"numm\":\"Jill\"}],\"Dag\":[{\"id\":159,\"numm\":\"Jill\"},{\"id\":159,\"numm\":\"Jill\"}]},{\"name\":\"Conge\",\"c\":5,\"Moies\":[{\"id\":159,\"numm\":\"Jill\"},{\"id\":159,\"numm\":\"Jill\"}],\"Mettes\":[{\"id\":159,\"numm\":\"Jill\"},{\"id\":159,\"numm\":\"Jill\"}],\"Dag\":[{\"id\":159,\"numm\":\"Jill\"},{\"id\":159,\"numm\":\"Jill\"}]}]}";
            plang = NSPlang.JsonRes.FromJson(jsonstring);
            Console.WriteLine("Json imported");
        }

        public async static Task ReadPlangFromServer()
        {
            var values = new Dictionary<string, string>();
            DateTime dateTime = plangDate;
            string tmpdate = dateTime.ToString("yyyy-MM-dd");
            values.Add("date", tmpdate);
            var content = new FormUrlEncodedContent(values);
            response = await client.PostAsync("http://intern.autisme.lu/remote/getFullPlang.ajax.php", content);
            String responseString = await response.Content.ReadAsStringAsync();
            if (responseString.ToString() != null)
            { plang = JsonConvert.DeserializeObject<JsonRes>(responseString); }
            //Console.WriteLine(plang.ToString());
            CheckRunning = false;
        }

        public async static Task ReadPlangFromServerAndPush(Boolean Push = true)
        {
            try
            {
                var r = ReadPlangFromServer();
                await r;
                if (Push) { SendPlang(); }
            }
            catch (Exception e)
            {
                Console.WriteLine("ReadPlangFromServerAndPush Error: " + e.Message);
                CheckRunning = false;
            }
        }

        public static void SendPlang()
        {
            for (int i = 1; i < clients.Count; i++) { SendToClient(clients.ElementAt(i).Key.ToString()); }
        }

        public static void SendToClient(String clientname, Boolean full = false)
        {
            CheckClientsStillConnected();

            bool check = clients.TryGetValue(clientname, out Socket currentsock);

            String text = "Du bass den Screen :D:D:D";

            Console.WriteLine("Sending " + text + " to client...");

            if (full)
            {
                JsonRes plangt = new JsonRes();
                plangt = plang;
                plangt.Type = "Fullplang";
                String thejson = NSPlang.Serialize.ToJson(plangt);
                if (check) Send_data(thejson, currentsock);
                return;
            }

            JsonRes plangtemp = new JsonRes
            {
                Type = "Plang",
                Content = new Content()
            };
            plangtemp.Content.Services = new List<Service>();
            plangtemp.Content.Date = plangDate;
            //1: CDJ(SAJ) - Dreckerei - Pabeier - Gaart - Bitzerei - Wäscherei - Kichen - Service Technique
            //2: Konfiserie - BackOffice - InfoMedia - SFP - KeFa
            //3: Conge - Krank - Fromatioun/Maart
            bool found = ScreensClients.TryGetValue(clientname, out int screennumber);
            if (found)
            {
                for (int i = 0; i < plang.Content.Services.Count; i++)
                {
                    switch (screennumber)
                    {
                        case 1:
                            if (plang.Content.Services[i].Id == 11) plangtemp.Content.Services.Add(plang.Content.Services[i]); //CDJ
                            if (plang.Content.Services[i].Id == 12) plangtemp.Content.Services.Add(plang.Content.Services[i]); //Dreckerei
                            if (plang.Content.Services[i].Id == 13) plangtemp.Content.Services.Add(plang.Content.Services[i]); //Pabeier
                            if (plang.Content.Services[i].Id == 14) plangtemp.Content.Services.Add(plang.Content.Services[i]); //Gaart
                            if (plang.Content.Services[i].Id == 15) plangtemp.Content.Services.Add(plang.Content.Services[i]); //Bitzerei
                            if (plang.Content.Services[i].Id == 16) plangtemp.Content.Services.Add(plang.Content.Services[i]); //Wäscherei
                                                                                                                               // if (plang.Content.Services[i].id == 17) plangtemp.Content.Services.Add(plang.Content.Services[i]); //Kichen
                            if (plang.Content.Services[i].Id == 17) plangtemp.Content.Services.Add(plang.Content.Services[i]); //Service Technique
                            if (plang.Content.Services[i].Id == 18) plangtemp.Content.Services.Add(plang.Content.Services[i]); //Service Technique
                            break;

                        case 2:
                            if (plang.Content.Services[i].Id == 21) plangtemp.Content.Services.Add(plang.Content.Services[i]); //Kichen
                            if (plang.Content.Services[i].Id == 22) plangtemp.Content.Services.Add(plang.Content.Services[i]); //Konfiserie
                            if (plang.Content.Services[i].Id == 23) plangtemp.Content.Services.Add(plang.Content.Services[i]); //Backoffice
                            if (plang.Content.Services[i].Id == 24) plangtemp.Content.Services.Add(plang.Content.Services[i]); //InfoMedia
                            if (plang.Content.Services[i].Id == 25) plangtemp.Content.Services.Add(plang.Content.Services[i]); //SFP
                            if (plang.Content.Services[i].Id == 26) plangtemp.Content.Services.Add(plang.Content.Services[i]); //Kefa
                            if (plang.Content.Services[i].Id == 34) plangtemp.Content.Services.Add(plang.Content.Services[i]); // DOKU
                            // if (plang.Content.Services[i].id == 27) plangtemp.Content.Services.Add(plang.Content.Services[i]); //EXTRA CONTENT
                            break;

                        case 3:
                            if (plang.Content.Services[i].Id == 31) plangtemp.Content.Services.Add(plang.Content.Services[i]); //Congé
                            if (plang.Content.Services[i].Id == 32) plangtemp.Content.Services.Add(plang.Content.Services[i]); //Krank
                            if (plang.Content.Services[i].Id == 33) plangtemp.Content.Services.Add(plang.Content.Services[i]); //Formatioun
                            if (plang.Content.Services[i].Id == 35) plangtemp.Content.Services.Add(plang.Content.Services[i]); //Maart
                            break;
                    }
                }
                /*
                if (screennumber == 2)
                {
                    //inject extra screen
                    Service service = new Service
                    {
                        C = 2,
                        Id = 27,
                        Name = "extra"
                    };
                    plangtemp.Content.Services.Add(service);
                }
                */
            }

            String testjson = NSPlang.Serialize.ToJson(plangtemp);

            //            String testjson = WSPlang.Serialize.ToJson(plang);
            //            if (check) send_data(testjson, currentsock);
            if (check) Send_data(testjson, currentsock);
        }

        public static void SendToClientUpdate(String clientname, int userid, int serviceid, Personal userdata, String fullday, Boolean full = false)
        {
            CheckClientsStillConnected();
            bool check = clients.TryGetValue(clientname, out Socket currentsock);
            JsonRes plangtemp = new JsonRes
            {
                Type = "single_update",
                Response = new Response()
            };
            plangtemp.Response.Concernuserid = userid;
            plangtemp.Response.Settoserviceid = serviceid;
            plangtemp.Response.Userdata = userdata;
            plangtemp.Response.Info = fullday;

            String testjson = NSPlang.Serialize.ToJson(plangtemp);
            if (check) Send_data(testjson, currentsock);
        }

        public async static void SetServerActive()
        {
#if DEBUG
            Console.WriteLine("#### DEBUG MODE: NOT SETTING SERVER ACTIVE ######");
#else
            var values = new Dictionary<string, string>
            {
                { "serverip", ServerIPAdress }
            };
            var content = new FormUrlEncodedContent(values);
            response = await client.PostAsync("http://intern.autisme.lu/remote/setServerActive.ajax.php", content);
            String responseString = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseString);
#endif
        }

        public async static void StartUp()
        {
            Console.WriteLine("Startup");
            var r = ReadPlangFromServer();
            await r;
            if (r.IsCompleted)
            {
                NumberofScreens = 3;
                frmMain.Self.UpdateCountDisplay(NumberofScreens);
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        Console.WriteLine(ip.ToString());
                        ServerIPAdress = ip.ToString();
                    }
                }

                Console.WriteLine(ServerIPAdress);

                Socket socket = Open_main_socket();

                frmMain.Self.ServerIP.Text = "Server IP: " + ServerIPAdress;

                SetServerActive();
            }
        }

        public static void UpdateClient(String clientname, String Tag, String Toggler)
        {
            /*
            ICollection<string> keys = (clients.Keys);
            IList user_list = new ArrayList();
            user_list = keys.ToList();
            */
            bool check = clients.TryGetValue(clientname, out Socket currentsock);
            if (check)
            {
                if (Toggler == "ON") Send_data("PUTONLINE:" + Tag, currentsock);
                else Send_data("PUTOFLINE:" + Tag, currentsock);
                // PUTONLINE | PUTOFLINE
            }
        }

        public async static void UpdateSingleUser(int userid, int serviceid, string fullday)
        {
            var values = new Dictionary<string, string>();
            DateTime dateTime = plangDate;
            string tmpdate = dateTime.ToString("yyyy-MM-dd");
            values.Add("date", tmpdate);
            values.Add("concernuserid", userid.ToString());
            values.Add("settoserviceid", serviceid.ToString());
            values.Add("fullday", fullday.ToString());
            var content = new FormUrlEncodedContent(values);

            response = await client.PostAsync("http://intern.autisme.lu/remote/updateSingleUser.ajax.php", content);
            if (response.ToString() != null)
            {
                Console.WriteLine("UpdateSingleUser done");
                String responseString = await response.Content.ReadAsStringAsync();
                var resendjson = JsonConvert.DeserializeObject<Response>(responseString);
                if (resendjson.ToString() != null)
                {
                    var res = ReadPlangFromServerAndPush(false);
                    await res;
                    SendSingleUpdate(userid, serviceid, resendjson.Userdata, resendjson.Info);
                }
            }
        }

        private static void Acceptuser(List<string> headers, Socket sock)
        {
            ICollection<string> keys = (clients.Keys);
            IList user_list = new ArrayList();
            user_list = keys.ToList();

            if (user_list.Contains(headers[0]))
            {
                Console.Write("User Already Connected\n");
                bool check = clients.TryGetValue(headers[0], out Socket currentsock);
                Close_frame(currentsock);
                clients.Remove(headers[0]);
                string handshake;
                string Key = headers[4].Trim() + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
                SHA1 sha = new SHA1CryptoServiceProvider();
                clients.Add(headers[0], sock);
                byte[] hash = sha.ComputeHash(Encoding.ASCII.GetBytes(Key));
                handshake = Convert.ToBase64String(hash);
                string AcceptKey = "HTTP/1.1 101 Switching Protocols\r\nUpgrade: websocket\r\nConnection: Upgrade\r\nSec-WebSocket-Protocol: " + headers[5].Trim() + "\r\nSec-WebSocket-Accept: " + handshake.Trim() + "\r\n\r\n";
                Send_handshake(AcceptKey, sock);
                Message_listener(sock);
            }
            else
            {
                string handshake;
                string Key = headers[4].Trim() + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
                SHA1 sha = new SHA1CryptoServiceProvider();
                clients.Add(headers[0], sock);
                frmMain.Self.CleanupItems();
                for (int i = 1; i < clients.Count; i++)
                { frmMain.Self.AddItemToList(clients.ElementAt(i).Key.ToString()); }
                byte[] hash = sha.ComputeHash(Encoding.ASCII.GetBytes(Key));
                handshake = Convert.ToBase64String(hash);
                string AcceptKey = "HTTP/1.1 101 Switching Protocols\r\nUpgrade: websocket\r\nConnection: Upgrade\r\nSec-WebSocket-Protocol: " + headers[5].Trim() + "\r\nSec-WebSocket-Accept: " + handshake.Trim() + "\r\n\r\n";
                Send_handshake(AcceptKey, sock);
                Message_listener(sock);
            }
        }

        private static void Begin_handshake(IAsyncResult AR)
        {
            Console.WriteLine("Shaking Hands");
            Socket sock = ((Socket)AR.AsyncState);
            //int Data = sock.EndReceive(AR);
            try {
                int Data = sock.EndReceive(AR);

                byte[] databyte = new byte[Data];
                Array.Copy(_buffer, databyte, Data);

                String text = Encoding.ASCII.GetString(databyte);
                List<string> headers = Retriveheaders(text);
                Acceptuser(headers, sock);
            } catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        private static void BroadcastCurrentMode(String sender, string info)
        {
            JsonRes jsonres = new JsonRes
            {
                Type = "info_screen_mode",
                Response = new Response()
            };
            jsonres.Response.Sender = sender;
            jsonres.Response.Info = info;
            jsonres.Response.Message = "info_screen_mode";
            SendJsonToClients(jsonres);
        }

        private static void BroadcastJson(JsonRes jsonRes)
        {
            for (int i = 1; i < clients.Count; i++)
            { SendJsonToClient(jsonRes, clients.ElementAt(i).Key); }
        }

        private static void Close_frame(Socket sock)
        {
            Byte[] frame = new Byte[2];
            frame[0] = 136;
            frame[1] = 0;
            try { sock.Send(frame); }
            catch (SocketException e) { Console.Write("Error Occured" + e.Message); }
        }

        public async static Task<Boolean> ToggleScreens(Boolean OnOff)
        {
            //ACTION -> HOME | Wait 1250ms | DOWN | Wait 200ms | CONFIRM (controlledbyjs)

            //actionarr 0=HOME 1=DOWN 2=CONFIRM 3=PowerTv
            string[] actionarr = { "AAAAAQAAAAEAAABgAw==", "AAAAAQAAAAEAAAB1Aw==", "AAAAAQAAAAEAAABlAw==", "AAAAAQAAAAEAAAAVAw==" };
            //string[] ips = { "192.168.1.71", "192.168.1.72", "192.168.1.73" };
            //string[] ips = { "192.168.1.71", "192.168.1.72", "192.168.1.73", "192.168.11.71", "192.168.11.72", "192.168.11.73" };
            string[] ips = {  "192.168.11.71", "192.168.11.72", "192.168.11.73" };

            bool[] res = { false, false, false };
            int _c = 0;
            foreach (string ip in ips)
            {
                bool currentlyrunning = await CheckScreenRunning(ip);
                res[_c] = currentlyrunning;
                _c++;

                if (currentlyrunning != OnOff)
                {
                    Console.WriteLine("Powering " + ip + " -> " + OnOff.ToString());
                    /* IRCC Toggle Power */
                    var httpClient = new HttpClient();
                    var httpContent = new HttpRequestMessage
                    {
                        RequestUri = new Uri("http://" + ip + "/sony/IRCC"),
                        Method = HttpMethod.Post,
                        Headers =
                        {
                            { HttpRequestHeader.ContentType.ToString(), "text/xml; charset=\"utf-8\";" },
                            { "X-Auth-PSK", "1337" },
                            { "soapaction", "\"urn:schemas-sony-com:service:IRCC:1#X_SendIRCC\"" }
                        },
                        Content = new StringContent("<?xml version=\"1.0\"?><s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\" s:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\"><s:Body><u:X_SendIRCC xmlns:u=\"urn:schemas-sony-com:service:IRCC:1\"><IRCCCode>\"" + actionarr[3] + "\"</IRCCCode></u:X_SendIRCC></s:Body></s:Envelope> ", Encoding.UTF8, "text/xml")
                    };
                    try
                    {
                        HttpResponseMessage _response = await httpClient.SendAsync(httpContent);

                        Console.WriteLine(ip + " # toggle power: " + _response.IsSuccessStatusCode);
                        if (OnOff)
                        {
                            var empty = await RunDisplayAppAndroid(ip);
                        }
                    }
                    catch (Exception ex)
                    { Console.WriteLine("Error: " + ex.Message); }
                }
            }
            if ((res[0] == res[1]) && (res[1] == res[2]))
            { return res[0]; }
            else { return !OnOff; }
        }

        public async static Task<Boolean> RunDisplayAppAndroid(string ip)
        {
            var content = new SonySystem
            {
                Result = new List<SonyResult> { },
                Id = 20,
                Method = "setActiveApp",
                Version = "1.0",
                Params = new List<SonyResult> {
                    new SonyResult()
                    {
                        Uri = "com.sony.dtv.lu.autisme.displayclient.lu.autisme.displayclient.MainActivity",
                        Data = ""
                    }
                }
            };
            try { 
                var stringPayload = await Task.Run(() => JsonConvert.SerializeObject(content));

                var httpClient = new HttpClient();
                var httpContent = new HttpRequestMessage
                {
                    RequestUri = new Uri("http://" + ip + "/sony/appControl"),
                    Method = HttpMethod.Post,
                    Headers =
                    {
                        { HttpRequestHeader.ContentType.ToString(), "application/json" },
                        { "X-Auth-PSK", "1337" }
                    },
                    Content = new StringContent(stringPayload)
                };

                HttpResponseMessage _response = await httpClient.SendAsync(httpContent);
            }
            catch (Exception ex)
            { Console.WriteLine("Error: " + ex.Message); }
            return true;
        }

        public async static Task<Boolean> CheckScreenRunning(string ip)
        {
            var content = new SonySystem
            {
                Id = 20,
                Method = "getPowerStatus",
                Version = "1.0",
                Params = new List<SonyResult> { new SonyResult() }
            };
            var stringPayload = await Task.Run(() => JsonConvert.SerializeObject(content));

            var httpClient = new HttpClient();
            var httpContent = new HttpRequestMessage
            {
                RequestUri = new Uri("http://" + ip + "/sony/system"),
                Method = HttpMethod.Post,
                Headers =
                {
                    { HttpRequestHeader.ContentType.ToString(), "application/json" },
                    { "X-Auth-PSK", "1337" }
                },
                Content = new StringContent(stringPayload)
            };
            try { 
                HttpResponseMessage _response = await httpClient.SendAsync(httpContent);
                if (_response.Content != null)
                {
                    var responseContent = await _response.Content.ReadAsStringAsync();
                    var _res = JsonConvert.DeserializeObject<SonySystem>(responseContent);
                    //Console.WriteLine(_res.Result.First<SonyResult>().Status);
                    if (_res.Result.First<SonyResult>().Status == "active")
                    {
                        //Console.WriteLine
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            } catch (Exception ex)
            { Console.WriteLine("Error: " + ex.Message);
                return false;
            }
        }

        private static void Datadecode(byte[] rawdata)
        {
  
            var fin = rawdata[0] & 0x81;
            bool res = fin != 129;
            var Lenght = rawdata[1] & 127;
            byte b = rawdata[1];
            int totalLength = 0;
            int keyIndex = 0;
            int dataLength = 0;
            if (Lenght <= 125)
            {
                keyIndex = 2;
                totalLength = Lenght + 6;
                dataLength = Lenght;
            }
            if (Lenght == 126)
            {
                dataLength = BitConverter.ToUInt16(new byte[] { rawdata[3], rawdata[2] }, 0);
                keyIndex = 4;
                totalLength = dataLength + 8;
            }
            if (Lenght == 127)
            {
                dataLength = (int)BitConverter.ToInt64(new byte[] { rawdata[9], rawdata[8], rawdata[7], rawdata[6], rawdata[5], rawdata[4], rawdata[3], rawdata[2] }, 0);
                keyIndex = 10;
                totalLength = dataLength + 14;
            }
            byte[] key = new byte[] { rawdata[keyIndex], rawdata[keyIndex + 1], rawdata[keyIndex + 2], rawdata[keyIndex + 3] };
            int dataIndex = keyIndex + 4;
            int count = 0;
            for (int i = dataIndex; i < totalLength; i++)
            {
                rawdata[i] = (byte)(rawdata[i] ^ key[count % 4]);
                count++;
            }
            string message = Encoding.ASCII.GetString(rawdata, dataIndex, dataLength);
            /*Console.WriteLine("Message Recieved:" + message);*/
            try
            {
                Response resp = JsonConvert.DeserializeObject<Response>(message);
                //Console.WriteLine(resp.ToString());
                if (resp.Message == "plang_init") { SendToClient(resp.Sender); }
                if (resp.Message == "plang_handler") { SendToClient(resp.Sender, true); }
                if (resp.Message == "req_update_user") { UpdateSingleUser(resp.Concernuserid, resp.Settoserviceid, resp.Info); }
                if (resp.Message == "req_connected_screens") { ReturnConnectedScreens(resp.Sender); }
                if (resp.Message == "info_mymode_screen") { BroadcastCurrentMode(resp.Sender, resp.Info); }
                //if (resp.Message == "ytplayer") { ytHandler }
                if (resp.Message == "req_switch_mode") { SwitchModeOfClient(resp.Settoserviceid, resp.Sender); }
                if (resp.Message == "req_user_check") { SetOnOrOff(resp); }
                if (resp.Message == "req_insert_new") { InsertNewUser(resp); }
                if (resp.Message == "req_ytplayer_play")
                {
                    JsonRes jsonres = new JsonRes { Type = "ytplayer_play", Response = new Response() };
                    jsonres.Response.Message = resp.Info;
                    BroadcastJson(jsonres);
                }
            }
            catch (Exception e) { Console.WriteLine(e.Message); }
        }

        private async static void InsertNewUser(Response res)
        {
            var values = new Dictionary<string, string>();
            DateTime dateTime = plangDate;
            string tmpdate = dateTime.ToString("yyyy-MM-dd");
            values.Add("name", res.Userdata.Numm);
            values.Add("surname", res.Userdata.Virnumm);
            values.Add("photo", res.Userdata.Photo);
            values.Add("date", tmpdate);
            values.Add("serviceid", res.Userdata.Id.ToString());
            values.Add("enc", res.Userdata.Isencadrant.ToString());
            var content = new FormUrlEncodedContent(values);

            response = await client.PostAsync("http://intern.autisme.lu/remote/insertNewUser.ajax.php", content);

            if (response.ToString() != null)
            {
                Console.WriteLine(response.ToString());
                //Console.WriteLine("UpdateSingleUser done");
                String responseString = await response.Content.ReadAsStringAsync();
                var resendjson = JsonConvert.DeserializeObject<Response>(responseString);
                if (resendjson.ToString() != null)
                {
                    var resi = ReadPlangFromServerAndPush(false);
                    await resi;
                    SendSingleUpdate((int)resendjson.Userdata.Id, resendjson.Settoserviceid, resendjson.Userdata, "1");
                }
            }
        }

        private static void SetOnOrOff(Response response)
        {
        }

        private static Byte[] EncodeMessageToSend(String message)
        {
            Byte[] response;
            Byte[] bytesRaw = Encoding.UTF8.GetBytes(message);
            Byte[] frame = new Byte[10];
            Int32 indexStartRawData = -1;
            Int32 length = bytesRaw.Length;
            frame[0] = 129;
            if (length <= 125)
            {
                frame[1] = (Byte)length;
                indexStartRawData = 2;
            }
            else if (length >= 126 && length <= 65535)
            {
                frame[1] = 126;
                frame[2] = (Byte)((length >> 8) & 255);
                frame[3] = (Byte)(length & 255);
                indexStartRawData = 4;
            }
            else
            {
                frame[1] = 127;
                frame[2] = (Byte)((length >> 56) & 255);
                frame[3] = (Byte)((length >> 48) & 255);
                frame[4] = (Byte)((length >> 40) & 255);
                frame[5] = (Byte)((length >> 32) & 255);
                frame[6] = (Byte)((length >> 24) & 255);
                frame[7] = (Byte)((length >> 16) & 255);
                frame[8] = (Byte)((length >> 8) & 255);
                frame[9] = (Byte)(length & 255);
                indexStartRawData = 10;
            }
            response = new Byte[indexStartRawData + length];
            Int32 i, reponseIdx = 0;
            for (i = 0; i < indexStartRawData; i++)
            {
                response[reponseIdx] = frame[i];
                reponseIdx++;
            }
            for (i = 0; i < length; i++)
            {
                response[reponseIdx] = bytesRaw[i];
                reponseIdx++;
            }
            return response;
        }

        private static void Listen_new(Socket sock)
        {
            try
            {
                Console.WriteLine("Listening Incoming Connnection...");
                sock.Listen(5);
                sock.BeginAccept(new AsyncCallback(Start_accept_new), sock);
            }
            catch (SocketException e) { Console.WriteLine("Socket Error Occured (Listen New):" + e.Message); }
        }

        private static void Message_listener(Socket insock)
        {
            start(insock);
            void start(Socket sock)
            {
                byte[] m_buffer = new byte[10000];
                try
                {
                    sock.BeginReceive(m_buffer, 0, m_buffer.Length, SocketFlags.None, ar =>
                    {
                        try { int dat = sock.EndReceive(ar); processdata(m_buffer, dat, ar); }
                        catch (Exception se) { Console.WriteLine("Error: " + se.Message); }
                    }, sock);
                }
                catch (Exception e) { Console.WriteLine("Error Occured Begin Receive: " + e.Message); }
            }

            void processdata(byte[] r_buffer, int size, IAsyncResult AR)
            {
                try { 
                    Socket sockp = ((Socket)AR.AsyncState);
                    switch (r_buffer[0] & 81)
                    {
                        case 1:
                            Datadecode(r_buffer);
                            start(sockp);
                            break;

                        case 0:
                            if (size == 6)
                            {
                                string myKey = clients.FirstOrDefault(x => x.Value == sockp).Key;
                                if (myKey != null)
                                {
                                    Close_frame(sockp);
                                    sockp.Shutdown(SocketShutdown.Both);
                                    sockp.Close();
                                    sockp.Dispose();
                                    try { clients.Remove(myKey); }
                                    catch (Exception e)
                                    { Console.WriteLine(e.Message); }
                                }
                                else
                                {
                                    sockp.Shutdown(SocketShutdown.Both);
                                    sockp.Close();
                                    sockp.Dispose();
                                }
                            }
                            else
                            {
                                sockp.Shutdown(SocketShutdown.Both);
                                sockp.Close();
                                sockp.Dispose();
                                string myKey = clients.FirstOrDefault(x => x.Value == sockp).Key;
                                if (myKey != null) clients.Remove(myKey);

                                frmMain.Self.CleanupItems();

                                for (int i = 1; i < clients.Count; i++)
                                { frmMain.Self.AddItemToList(clients.ElementAt(i).Key.ToString()); }
                            }
                            break;

                        default:
                            Console.WriteLine("Default Switch");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
        }

        private static Socket Open_main_socket()
        {
            try
            {
                IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse(ServerIPAdress), 11000);
                Console.WriteLine("Opening Main Socket...");
                Socket main_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                main_socket.Bind(endpoint);
                Listen_new(main_socket);
                clients.Add("main_socket", main_socket);
                return main_socket;
            }
            catch (SocketException e)
            {
                Console.WriteLine("Socket Error Occured (Main Socket):" + e.Message);
                return null;
            }
        }

        private static void RequestCurrentMode()
        {
            JsonRes jsonres = new JsonRes
            {
                Type = "req_current_mode"
            };
            BroadcastJson(jsonres);
        }

        private static List<string> Retriveheaders(string Data)
        {
            /*Console.Write("Retriveing Headers\n");*/
            List<string> headers = new List<string>
            {
                Regex.Match(Data, "(?<=GET /)(.*)(?= HTTP)").ToString(),
                Regex.Match(Data, "(?<=Host: )(.*)[?=\r\n*]").ToString(),
                Regex.Match(Data, "(?<=Upgrade: )(.*)[?=\r\n]").ToString(),
                Regex.Match(Data, "(?<=Connection: )(.*)[?=\r\n]").ToString(),
                Regex.Match(Data, "(?<=Sec-WebSocket-Key: )(.*)[?=\r\n]").ToString(),
                Regex.Match(Data, "(?<=Sec-WebSocket-Protocol: )(.*)[?=\r\n]").ToString(),
                Regex.Match(Data, "(?<=Sec-WebSocket-Version: )(.*)[?=\r\n]").ToString(),
                Regex.Match(Data, "(?<=Origin: )(.*)\r\n").ToString()
            };
            return headers;
        }

        private static void ReturnConnectedScreens(string sender)
        {
            JsonRes jsonres = new JsonRes
            {
                Type = "connected_screens",
                Content = new Content()
            };
            jsonres.Content.Services = new List<Service>();

            foreach (KeyValuePair<string, int> sc in ScreensClients)
            {
                Service service = new Service
                {
                    Name = sc.Key,
                    Id = sc.Value
                };
                jsonres.Content.Services.Add(service);
            }
            SendJsonToClient(jsonres, sender);
            RequestCurrentMode();
        }

        private static void Send_data(String data, Socket socket)
        {
            byte[] r_data = EncodeMessageToSend(data);
            try
            {
                /*Console.Write("Sending Handshake:\n");*/
                socket.BeginSend(r_data, 0, r_data.Length, SocketFlags.None, new AsyncCallback(Sent), socket);
            }
            catch (SocketException e) { Console.WriteLine("Socket Error Occured When sending data 1: " + e.Message); }
        }

        private static void Send_handshake(String data, Socket socket)
        {
            byte[] send_key = Encoding.UTF8.GetBytes(data);
            try
            {
                //Console.Write("Sending Handshake:\n");
                socket.BeginSend(send_key, 0, send_key.Length, SocketFlags.None, new AsyncCallback(Sent), socket);
            }
            catch (SocketException e) { Console.WriteLine("Socket Error Occured When sending data 1: " + e.Message); }
        }

        private static Boolean SendJsonToClient(JsonRes jsonRes, String clientid)
        {
            bool check = clients.TryGetValue(clientid, out Socket currentsock);
            if (!check) return false;
            else
            {
                String serJson = NSPlang.Serialize.ToJson(jsonRes);
                Send_data(serJson, currentsock);
                return true;
            }
        }

        private static void SendJsonToClients(JsonRes jsonRes)
        {
            for (int i = 1; i < clients.Count; i++)
            {
                Boolean found = false;

                for (int j = 0; j < ScreensClients.Count; j++)
                {
                    string abc = ScreensClients.ElementAt(j).Key.ToString();
                    string def = clients.ElementAt(i).Key.ToString();
                    if (ScreensClients.ElementAt(j).Key.ToString() == clients.ElementAt(i).Key.ToString()) { found = true; }
                }

                if (!found)
                {
                    //Must be a Client / not a Screen
                    //Console.WriteLine(clients.ElementAt(i).Key);
                    SendJsonToClient(jsonRes, clients.ElementAt(i).Key);
                }
            }
        }

        private static void SendSingleUpdate(int userid, int serviceid, Personal userdata, String fullday)
        {
            for (int i = 1; i < clients.Count; i++)
            {
                SendToClientUpdate(clients.ElementAt(i).Key.ToString(), userid, serviceid, userdata, fullday, false);
            }
        }

        private static void Sent(IAsyncResult AR)
        {
            Socket sock = (Socket)AR.AsyncState;
            try { sock.EndSend(AR); }
            catch (SocketException e)
            { Console.WriteLine("Socket Error Occured When sending data 2: " + e.Message); }
        }

        private static void Start_accept_new(IAsyncResult AR)
        {
            try
            {
                Console.WriteLine("Accepting New Connection....");
                Socket sock = ((Socket)AR.AsyncState).EndAccept(AR);
                Listen_new((Socket)AR.AsyncState);
                sock.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(Begin_handshake), sock);
            }
            catch (SocketException e) { Console.WriteLine("Socket Error Occured(Start Accept New):" + e.Message); }
        }

        private static void SwitchModeOfClient(int clientnumber, string mode)
        {
            string clientname = "";
            foreach (KeyValuePair<string, int> sc in ScreensClients)
            {
                if (sc.Value == clientnumber)
                {
                    clientname = sc.Key;
                }
            }

            bool check = clients.TryGetValue(clientname, out Socket currentsock);
            if (check)
            {
                JsonRes tclass = new JsonRes
                {
                    Type = "switch_mode",
                    Response = new Response()
                };
                tclass.Response.Message = mode;
                string tjson = NSPlang.Serialize.ToJson(tclass);
                if (check) Send_data(tjson, currentsock);
            }
        }
    }
}