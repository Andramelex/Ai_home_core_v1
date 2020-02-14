using System;
using System.Collections;

using System.Drawing;
using System.IO;

using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Ai_home_core_v1
{
    class Coket
    {
        public string adresIP;
        public static int Sport = 2121; // порт для приема входящих запросов
                                        // порт дебага    // static int Sport = 2122;                         // получаем адреса для запуска сокета
                                        // IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("192.168.1.35"), Sport); // получаем адреса для запуска сокета
        public Socket listenSocket;

        public IPEndPoint clientep;
        // public static string datasoketToGraf = "";
        Socket handler;
        string data = null;
        public string BuffSocetText;
        public string BufftoLogBox;
        public string SendingData;
        public string WhoconectToSocket;
        public string[] WhoconectToSocketName = new string[10];
       public ArrayList list = new ArrayList();

        public void OpenSocet()
        {
            Thread myThread = new Thread(new ThreadStart(StartSocet));
            myThread.Start(); // запускаем поток
        }
        public void StartSocet() { 

        IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(adresIP), Sport);
            // создаем сокет
            listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                // связываем сокет с локальной точкой, по которой будем принимать данные
                listenSocket.Bind(ipPoint);


                // начинаем прослушивание
                listenSocket.Listen(5);
                BuffSocetText = ("Сервер запущен. Ожидание подключений...");
                Console.WriteLine("Сервер запущен. Ожидание подключений...");

                while (true)
                {
                    handler = listenSocket.Accept();

                    // получаем сообщение
                    // StringBuilder builder = new StringBuilder();

                    // Мы дождались клиента, пытающегося с нами соединиться

                    byte[] bytes = new byte[3024];
                    int bytesRec = handler.Receive(bytes);

                    data += Encoding.UTF8.GetString(bytes, 0, bytesRec);
                    clientep = (IPEndPoint)handler.RemoteEndPoint;
                    BuffSocetText = (DateTime.Now.ToLongTimeString()) +" от "+ clientep.Address + Environment.NewLine + "Пришло на сокет: " + data; //+ Environment.NewLine; = Это перенос текста на новую строку
                    int zz = 0;
                    int gh = 0;
                    ArrayList Dubl = new ArrayList();
                    string nameof="";
                    list.Add("" + clientep.Address);
                    foreach (string nameX in list)
                    {
                        zz = 0;
                        gh = 0;

                        for (int zo = 0; zo < Dubl.Count; zo++)
                        {
                           // MessageBox.Show("Что сравниваем " + nameX + " и "+ Dubl[zo]);
                            if (nameX.ToString() == Dubl[zo].ToString())
                            {
                                gh = 1;
                            }
                        }
                       // MessageBox.Show("гх равен " + gh);
                        if (gh == 0)
                        { 
                           for (int i = 0; i < list.Count; i++)
                           {

                                    if (nameX == list[i].ToString() )
                                    {
                                        zz++;

                                    }

                           }
                            nameof += (nameX + " " + zz + " разa "+"\n");
                          //  MessageBox.Show("чем сейчас равно нйемоф " + nameof);
                        }
                    
                        
                        Dubl.Add(nameX);
                       // MessageBox.Show("чем сейчас равно список дубла " + Dubl.Count);
                    }
                   
                    Dubl.Clear();
                    // list.Add(nameof);
                    // WhoconectToSocket = "Был подключен :" + clientep.Address;
                    WhoconectToSocket = "Был подключен :"+ nameof;
                    //datasoketToGraf = data;
                    RazborSocetEnter();
                    data = "";
                    nameof = "";
                  //  MessageBox.Show("нйемоф после удаления " + nameof);
                    // OtvetSocket();

                    // закрываем сокет
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                    // break;
                }
            }
            catch (Exception ex)
            {
                BufftoLogBox = ((DateTime.Now.ToLocalTime()) + " ошибка работы сокета: ");//+ ex.Message
                Console.WriteLine(ex.Message);
                // тут будет сохранять паразитный запрос
                string ErroSocet = @"D:\SomeDir\ErroSocet.txt";
                try
                {
                    using (StreamWriter sw2 = new StreamWriter(ErroSocet, true, System.Text.Encoding.Default))
                    {
                        sw2.WriteLine((DateTime.Now.ToString()) + " =" + ex.Message + " = " + data + " Он сломал сокет :" + clientep.Address);
                    }
                    data = "";
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                    listenSocket.Close();
                    OpenSocet();
                }
                catch (Exception ex2)
                {
                    BufftoLogBox = ((DateTime.Now.ToLongTimeString()) + " Ошибка сохранения логов по хакерам :( ");//+ ex2.Message

                    //MessageBox.Show("Ошибка сохранения логов по хакерам :( " + ex2.Message);
                }
            }

        }
       
        public void StopHack()
        {
            
            BufftoLogBox = (DateTime.Now.ToLongTimeString()) + " Закрытие сокета, перезагрузка соединения от :" + clientep.Address + Environment.NewLine + "Пришло на сокет: " + data;
            string ErroSocet = @"D:\SomeDir\ErroSocet.txt";
            try
            {
                using (StreamWriter sw2 = new StreamWriter(ErroSocet, true, System.Text.Encoding.Default))
                {
                    sw2.WriteLine((DateTime.Now.ToString()) + " =" + data + " хакер сидит тут :" + clientep.Address);
                }
                
            }
            catch (Exception ex2)
            {
                BufftoLogBox = ((DateTime.Now.ToLongTimeString()) + " Ошибка сохранения логов по хакерам :( ");//+ ex2.Message

                //MessageBox.Show("Ошибка сохранения логов по хакерам :( " + ex2.Message);
            }
            data = "";
            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
            listenSocket.Close();
            OpenSocet();
        }
        public void RazborSocetEnter()
        {
            //string tesOtvet = "20;12;13;14;15;16";
            data = data.ToString();
            Console.WriteLine("На сокет упало: " + data);
            if (data.Length < 4)
            {
                // MessageBox.Show("Соекет отдал малую команду, будем передавать в ком", "Внимание");
                ////////////////////////  OtvetSocketInt = 1;
                ConnectSoketvsComport();


            }
            else
            {
                StopHack();
            }
        }
        public string SendingCom() // вызываемая функция которая содержит в себе команду для компорт (ранее в  нее передали из сокета)
        {
            string boxgo = SendingData;
            SendingData = null;


            return boxgo;

        }

        public void SendtoCom(string Mes) // функция что собирает команды для передачи в компорт
        {
            SendingData = Mes;
        }

        //функция что разбирает коды полученные по сокету
        private void ConnectSoketvsComport()
        {
            Console.WriteLine("мы получили из сокета: " + data);
            // MessageBox.Show("Мы запустили отправку"+data, "Внимание");
            switch (data)
            {
                case "12":
                    SendtoCom("2");
                    // MessageBox.Show("ушло в сокет по команде 2" , "Внимание");
                    break;
                case "13":
                    SendtoCom("3");
                    //MessageBox.Show("ушло в сокет по команде 3", "Внимание");
                    break;
                case "14":
                    // byte[] msg = Encoding.UTF8.GetBytes(reply);
                    byte[] sendbytToSocket = Encoding.UTF8.GetBytes("1;95.00;22.00;0;561;0;24.00;19.00");
                    handler.Send(sendbytToSocket);
                    //handler.S
                    //SendtoCom("3");
                    // MessageBox.Show("ушло в сокет по команде 14" + sendbytToSocket, "Внимание");
                    break;

                case "15": // отправляем фото с камеры
                    {
                        //MessageBox.Show("ушло в сокет по команде 15", "Внимание");

                        Console.WriteLine("готовимся отправить картинку");
                        // There is a text file test.txt located in the root directory.
                        string fileName = @"D:\wamp2\www\3\images\cam\web1.jpg";

                        // Send file fileName to remote device
                        Console.WriteLine("Sending {0} to the host.", fileName);

                        // bitmap = new BitmapImage(uri);
                        Bitmap buf = new Bitmap(fileName);
                        Console.WriteLine("загрузили битмап");
                        //imgCam.Source = bitmap;
                        Console.WriteLine("сделали какой-то сорс битмап");
                        // ImageConverter _imageConverter = new ImageConverter();
                        // byte[] xByte = (byte[])_imageConverter.ConvertTo(bitmap, typeof(byte[]));
                        ImageConverter converter = new ImageConverter();
                        byte[] byteArray = (byte[])converter.ConvertTo(buf, typeof(byte[]));
                        int sirz = byteArray.Length;
                        string reply = sirz.ToString();

                        Console.WriteLine("вроде как сделали массив" + reply);
                        byte[] sendSize = Encoding.UTF8.GetBytes(reply);
                        handler.Send(sendSize);
                        System.Threading.Thread.Sleep(30);
                        handler.Send(byteArray);

                        // listenSocket.Send(byteArray);
                        Console.WriteLine("отправили картинку ?" + byteArray.Length);
                        //handler.Shutdown(SocketShutdown.Both);
                        //handler.Close();
                    }
                    break;
                case "16": // свет на кухне
                    SendtoCom("lightkitchen");
                    break;
                case "17": // выключаем свет везде
                    SendtoCom("lightoffall");
                    break;
                case "18": // свет в спальне
                    SendtoCom("lightSpalnya");
                    break;

                case "24":
                    SendtoCom("4");
                    break;

                case "25":
                    SendtoCom("5");
                    break;

                case "26":
                    SendtoCom("6");
                    //MessageBox.Show("ушло в сокет по команде 6", "Внимание");
                    break;
                case "27":
                    SendtoCom("7");
                    //MessageBox.Show("ушло в сокет по команде 7 (Radio)", "Внимание");
                    break;
                case "28":
                    SendtoCom("8");
                    //MessageBox.Show("ушло в сокет по команде 8", "Внимание");
                    break;
                case "32":
                    SendtoCom("LedOn"); // свет в коридоре
                    // MessageBox.Show("ушло в сокет по команде Led on", "Внимание");
                    break;
                case "33":
                    SendtoCom("LedOff"); // свет в коридоре ( старая команда)
                    // MessageBox.Show("ушло в сокет по команде Led off", "Внимание");
                    break;
                default:
                    Console.WriteLine("Вы нажали неизвестную букву");
                    break;
            }
           ;
        }
    }
}
