using System;

using System.IO;
using System.IO.Ports;

using System.Threading;


namespace Ai_home_core_v1
{
    class Comport
    {
        public SerialPort port = new SerialPort();
        public string[] ports;
        string[] splitData;
        public static string[] writePath = new string[13];
        public static string[] writePath2 = new string[13];
        public int StatusComPort = 0;
        public string returnMessage;
        private static System.Timers.Timer aTimer;
        public string BufftoLogBox;
        public string BuffTextBox;
        public string[] SendPull = new string[10];
        public string[] OtvetPull;
        public float SolarVoltage ;
        public float SolarCurent;
        int online = 0;
        public int buzzy = 0;

        public void InitalisationComPortD()

        {
            for (int i = 0; i < SendPull.Length; i++)
            {
                SendPull[i] = "0";
            }
            for (int i = 0; i < writePath.Length; i++)
            {
                writePath[i] = @"D:\SomeDir\data_" + i + ".txt";
            }
            for (int i = 0; i < writePath.Length; i++)
            {
                writePath2[i] = @"D:\SomeDir\data_soc_" + i + ".txt";
            }

            writePath[12] = @"D:\SomeDir\data_time.txt";

            ports = SerialPort.GetPortNames();
            
            aTimer = new System.Timers.Timer();
            aTimer.Interval = 500;

            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += OnTimedEvent;

         
            // Start the timer
          aTimer.Enabled = true;

        }
        public void AddSendPull(string Data)
        //
        // Сводка:
        //     Отображать элемент.
        {
            for (int i = 0; i < SendPull.Length; i++)
            {
                if (SendPull[i] == "0")
                {
                    SendPull[i] = Data;
                  //  Console.WriteLine("Добавили на отправку в очередь " + Data);
                    return;
                }
            }

        }
        public int LengthPullSend()
        {
            int summa = 0;
            for (int i = 0; i < SendPull.Length; i++)
            {
                string inspect = SendPull[i];
                if (inspect != "0")
                {
                    summa ++;
                }
             }
            return summa;
            // Console.WriteLine("проверяем очередь отправки -" + summa);
        }
        public int LengthPullOtvet()
        {
            int summa = 0;
            if (OtvetPull != null)
            {
                for (int i = 0; i < OtvetPull.Length; i++)
                {
                    string inspect = OtvetPull[i];
                    if (inspect != null)
                    {
                        summa++;
                    }
                }
            }
            return summa;
            // Console.WriteLine("проверяем очередь отправки -" + summa);
        }

        public void StartComPort() // Метод для соедниения с сом-портом и Ардуино
        {
            string joined = string.Join(",", ports);
            try
            {
                // настройки порта
                port.PortName = ports[MainWindow.num];
                port.BaudRate = 115200;
              // port.BaudRate = 9600;
                port.DataBits = 8;
                port.Parity = System.IO.Ports.Parity.None;
                port.StopBits = System.IO.Ports.StopBits.One;
                port.ReadTimeout = 100;
                port.WriteTimeout = 100;
                port.Open();
                StatusComPort = 1;
                Console.WriteLine("октрыли порт");
                          
            } catch (Exception e)
            {
                Console.WriteLine("ERROR: невозможно открыть Com порт:" + e.ToString());
                BufftoLogBox = "невозможно открыть COM порт";
                return;
            }
           
        }

        public void CloseComPort()
        {
            port.Close();
            StatusComPort = 0;

        }

        private void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e) // таймер каждую секунду проверку ком порта
        {
            online = 0;
            if (StatusComPort == 1) { 
            AddSendPull("Status");
                if (LengthPullSend() > 0 && buzzy != 1)
                {

                     Thread myThread = new Thread(new ThreadStart(SendComport2));
                      myThread.Start(); // запускаем поток
                }
            }

        }

        public void ReadComPort() // функция авто чтения компорт что пинается каждый ра 
        {

            if (StatusComPort == 1)
            {
                if (port.BytesToRead > 2)
                {
                    returnMessage = port.ReadLine();
                 
                    OtvetPull = returnMessage.Split('#');
                    

                    RazborDateCOM(OtvetPull);
                    OtvetPull = null;
                }

            }

        }
        public void RazborDateCOM(string[] Datawork)
        {
            string datatext=null;
            
            for (int z = 0; z < (Datawork.Length-1); z++)
            {
                splitData = Datawork[z].Split(';');
               // Console.WriteLine("try chuse-" + splitData[0]);


                if (splitData[0] != "777" && splitData[0] != "0")
                {
                            datatext += (DateTime.Now.ToLongTimeString() + " Получено Com port: " + Datawork[z]);
  
                }
                   
                switch (splitData[0])
                {
                    
                    case "1":
                        {
                            //  Console.WriteLine("try save");

                          //  MessageBox.Show("Напряжение было : " + splitData[11]);
                            SolarVoltage = float.Parse(splitData[10], System.Globalization.CultureInfo.InvariantCulture);
                            SolarCurent = float.Parse(splitData[11], System.Globalization.CultureInfo.InvariantCulture);
                         //  MessageBox.Show("Напряжение стало : " + SolarCurent);


                            for (int im = 1; im < splitData.Length; im++)

                                using (StreamWriter sw = new StreamWriter(writePath[im], true, System.Text.Encoding.Default))
                                {
                                    sw.WriteLine(splitData[im] +"-"+ (DateTime.Now.ToLongTimeString())+ ";");
                                    //MessageBox.Show("сохраняем данные: " + i,
                                    //"Внимание");
                                }


                            using (StreamWriter sw = new StreamWriter(writePath[12], true, System.Text.Encoding.Default))
                            {
                                sw.WriteLine((DateTime.Now.ToLongTimeString()) + ";");

                            }
                            break;
                        }
                    case "12":
                        break;
                  case "11":
                        {
                            for (int i = 1; i < splitData.Length; i++)
                            {

                                using (StreamWriter sw = new StreamWriter(writePath2[i], true, System.Text.Encoding.Default))
                                {
                                  //  sw.WriteLine(splitData[i] + ";");
                                    sw.WriteLine(splitData[i] + "-" + (DateTime.Now.ToLongTimeString()) + ";");
                                }
                               
                            }
                            break;
                        }
                    case "777":
                        {
                            // BuffTextBox = (DateTime.Now.ToLongTimeString() + " Получено Com port: " + ");
                            online = 1;
                        }
                        break;
                    case "0":
                        break;
            }
            }
            BuffTextBox = datatext;

         //   Console.WriteLine("Данные обработаны :" + splitData[0]);
            
        }
        public int Onlines()
        {
            if (online == 1)
                return 1;
            else
            {
                return 0;
            }
        }

        public void SendComport2()
        {
            buzzy = 1;
          //  int timer = LengthPullSend();
           // Console.WriteLine("Длина пула :" + timer);
            for (int i =0; i < SendPull.Length; i++)
            {
                if (SendPull[i] != "0")
                {
                 
                   // Console.WriteLine("Отправлии в ком2 :" + SendPull[i]);
                  
                  ChekAvalibelAnswerCom(SendPull[i], i);
                
                    SendPull[i] = "0";
                 
                }
               
            }
            buzzy = 0;

        }
    
        public void ChekAvalibelAnswerCom(string mesage, int numberData)
        {
           
            port.Write(mesage);

            for (int z = 0; z < 20; z++)
           
            {
                if (port.BytesToRead > 2)
                {
                 //   Console.WriteLine("Попыток для ответа ="+z+"*400 мс"+ " в потоке "+ numberData);
                 break;
                }
              Thread.Sleep(100);
               // Console.WriteLine("Колличество подходов=" + z + " в потоке " + numberData);
               // z++;
            }
            ReadComPort();
        }

    }
   
}
