using System;

using System.IO;

using System.Threading;

using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

using Image = System.Windows.Controls.Image;

namespace Ai_home_core_v1
{
    
     partial class MainWindow : Window
    {
        Comport ComPortClass = new Comport();
        Coket SoketMyClass = new Coket();

        public string BuffSocetText;
        public string BufftoLogBox;
   
        public int countsolarMassive = 0;
        static  public int num = 0;
    
        private static System.Timers.Timer aTimer;
        private static System.Timers.Timer dTimer;
        int chekCam = 0;
        
        string[] splitData;
        string[] splitDataSoc;
        string returnMessage;
 
        int AvtDataCom = 0;
       
        public int DebugStatus = 0;

        public static double[] CoMass = new double[10];
        public static string[] CoMassData = new string[10];
        string BufftoLogBoxCash;
        string BuffTextBoxCash;
        string BuffSocetBoxCash;
        string BuffSocetLogboxCash;

        public MainWindow()
        {
            CoMass[0] = 5;
            CoMass[1] = 5;
            CoMass[2] = 5;
            CoMassData[0] = "0";
            CoMassData[1] = "1";
            CoMassData[2] = "";
            

            InitializeComponent();
            ComPortClass.InitalisationComPortD();
            serchCom();
            
            
             aTimer = new System.Timers.Timer();
             dTimer = new System.Timers.Timer();
             aTimer.Interval = 1000;
             dTimer.Interval = 100;

            // Hook up the Elapsed event for the timer. 
             aTimer.Elapsed += OnTimedEvent;
             dTimer.Elapsed += Timeron100;

            // Start the timer
            aTimer.Enabled = true;
            dTimer.Enabled = true;

        }
        private void COM_TOP()
           
        {
            try
            {
                int Xo = ComPortClass.LengthPullSend();

                int Xl = ComPortClass.LengthPullOtvet();
                if (Xl == null)
                {
                    Xl = 0;
                }
                // RichTextBox rtb = new RichTextBox();

                ComTOP2.Document.Blocks.Clear();

                TextRange rangeOfText1 = new TextRange(ComTOP2.Document.ContentEnd, ComTOP2.Document.ContentEnd);
                rangeOfText1.Text = ("Длина очереди: ");
                rangeOfText1.ApplyPropertyValue(TextElement.ForegroundProperty, System.Windows.Media.Brushes.Black);
                rangeOfText1.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);

                TextRange rangeOfText2 = new TextRange(ComTOP2.Document.ContentEnd, ComTOP2.Document.ContentEnd);
                rangeOfText2.Text = ("S" + Xo);
                rangeOfText2.ApplyPropertyValue(TextElement.ForegroundProperty, System.Windows.Media.Brushes.Green);
                rangeOfText2.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);

                TextRange rangeOfText3 = new TextRange(ComTOP2.Document.ContentEnd, ComTOP2.Document.ContentEnd);
                rangeOfText3.Text = ("=");
                rangeOfText3.ApplyPropertyValue(TextElement.ForegroundProperty, System.Windows.Media.Brushes.Black);
                rangeOfText3.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);


                TextRange rangeOfWord = new TextRange(ComTOP2.Document.ContentEnd, ComTOP2.Document.ContentEnd);
                rangeOfWord.Text = ("D" + Xl);
                rangeOfWord.ApplyPropertyValue(TextElement.ForegroundProperty, System.Windows.Media.Brushes.Red);
                rangeOfWord.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Regular);

                // ComTOP.Text = String.Format("Длина очереди: S{0}=D{1}", Xo, Xl);
            } catch (Exception ex)
            {
                MessageBox.Show("Ошибк отображения замера очереди");
            }

        } 
        public void TransleyterSocetTOcomport() // функция что передает данные из сокета в очередь на отпправку в компорт
        {
            string mex = SoketMyClass.SendingCom();
            if (mex != null)
            {
                ComPortClass.AddSendPull(mex);
                CoketShow.Text = SoketMyClass.WhoconectToSocket; // метод отображает кто подключился к сокету
                
            }

        }
        public void chekStatusArduino()
        {
           
            int online=ComPortClass.Onlines();
            if (online == 1)
            {
                BrStatus.Fill = (System.Windows.Media.Brush)this.TryFindResource("StOn");

            } else
            {
                BrStatus.Fill = (System.Windows.Media.Brush)this.TryFindResource("StatusOff");
            }
        }
        private void Timeron100 (object sender, ElapsedEventArgs e) // таймер на 100 мл, что будет выводит техбоксы
        {
           
          
            Application.Current.Dispatcher.Invoke(() =>
            {
                TransleyterSocetTOcomport(); // запускаем метод что тянет данные из сокета в очередь отправки компорта
                RelogTexDisplay(); // метод что забирает данные для отображения в дисплеях (лог и техбокс)

              //  Console.WriteLine("test" + DateTime.Now.ToString());
            });
           
        }
        public void RelogTexDisplay() //Функция  сихнронизации и обновления чтения данных для отображения в листнерах логов и текста
        {
            try
            {

                if (BufftoLogBoxCash != ComPortClass.BufftoLogBox)
                {
                    LogBoxListSet(ComPortClass.BufftoLogBox);
                };
                if (BuffTextBoxCash != ComPortClass.BuffTextBox && ComPortClass.BuffTextBox != null)
                {
                    Console.WriteLine(ComPortClass.BuffTextBox);
                    TextBoxListSet(ComPortClass.BuffTextBox);
                    TextBox.Text = ComPortClass.BuffTextBox;//старый метод
                };
                if (BuffSocetBoxCash != SoketMyClass.BuffSocetText)
                {
                    TextBoxListSet(SoketMyClass.BuffSocetText);

                    // TextBox.Text = ComPortClass.BuffTextBox;//старый метод
                };
                if (BuffSocetLogboxCash != SoketMyClass.BufftoLogBox)
                {
                    LogBoxListSet(SoketMyClass.BufftoLogBox);
                };

                BufftoLogBoxCash = ComPortClass.BufftoLogBox;
                BuffTextBoxCash = ComPortClass.BuffTextBox;
                BuffSocetBoxCash = SoketMyClass.BuffSocetText;
                BuffSocetLogboxCash = SoketMyClass.BufftoLogBox;
            }catch (Exception ex) { MessageBox.Show("Ошибка функции отображени буфер лога и техт лога"); }
        }


        public void TextBoxListSet(string mesage) // функция что добавляет в техтбокс полученную стринг-дату
            
        {
            int Lenth=0;
            Lenth= TextBoxList.Items.Count;
            if (Lenth > 16)
            {
                TextBoxList.Items.Clear();
            }
            
            TextBoxList.Items.Add(mesage);
            
        }
        public void LogBoxListSet(string mesage)// функция что добавляет в Логобокс полученную стринг-дату
        {
            int Lenth = 0;
            Lenth = LogBox.Items.Count;
            if (Lenth > 10)
            {
                LogBox.Items.Clear();
            }

            LogBox.Items.Add(mesage);
        }

        private void serchCom() // просматривает открытые комп-порт ы отображает их  на текстбоксе
        {
             for (int i = 0; i < ComPortClass.ports.Length; i++)
            {
                Console.WriteLine("[" + i.ToString() + "] " + ComPortClass.ports[i].ToString());


                //------------------Вот этот метод позовляет изменять Юи в главном потоке из дочерхних потоков
                Application.Current.MainWindow.Dispatcher.Invoke(() =>
                {
                    TextBoxListSet(("[" + i.ToString() + "] " + ComPortClass.ports[i].ToString()));
                  //  TextBox.Text = "[" + i.ToString() + "] " + ComPortClass.ports[i].ToString();
                });
                //-------------Вот этот метод позовляет изменять Юи в главном потоке из дочерхних потоков


            }
            if (ComPortClass.ports.Length < 1)
            {
                btCom0.Visibility = Visibility.Hidden;
                btCom1.Visibility = Visibility.Hidden;
                btCom1_Copy.Visibility = Visibility.Hidden;
                TextBox.Text = "Нет открытых ком-портов";
                TextBoxListSet("Нет открытых ком-портов");
                btnOne.IsEnabled = false;
                ButClosePort.IsEnabled = false;
            }
            //LabelData.Text = DateTime.Now.ToString("yyyy.MM.dd, HH.mm.ss");
        }

        //методо тамера что обновляет мне экран
        private void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e) // таймер на 1 секунду.
        {
                      
            Application.Current.Dispatcher.Invoke(() =>
            {
                chekStatusArduino();

                LabelData.Content = DateTime.Now.ToShortDateString() + ", " + DateTime.Now.ToLongTimeString(); // каждую секунду (по таймеру) отображаем время в лейбле
                progressBar.Value++; // каждую секунду добавляем значение прогрессбара )
                                     // double solar = ComPortClass.SolarCurent;
                SolarStatus();
                COM_TOP();


                if (progressBar.Value == 59) //прогресс бар дошел до 59 то..
                {
                    progressBar.Value = 1; // сбросили его снова к началу
                    if (chekCam == 1)
                    {
                        // autoSendZaprocSocet();
                        Barpush(); // дернули изображение с вебки
                    }
                    if (AvtDataCom == 1)
                    {
                        autoSendZaprocSocet(); // начинаем дудосить компорт дохлый методок, пора удалять
                    }
                    
                };
                
            });

        }
        public void SolarStatus()
        {
            string dataSolar = ComPortClass.SolarCurent.ToString("0.####");
            solarProgress.Value = (Convert.ToDouble(dataSolar)*1000);

            dataSolar = ComPortClass.SolarVoltage.ToString("0.####");
            solarProgressVolt2.Value = Convert.ToDouble(dataSolar);

            double sumwat = 0;
             sumwat = (solarProgress.Value / 1000) * solarProgressVolt2.Value;
            watshow.Text = sumwat.ToString("0.##");
        }

        // методо загрузки картинки камеры.
        private void Barpush()
        {
          
            BitmapImage bitmap = new BitmapImage();
            var stream = File.OpenRead(@"D:\wamp2\www\3\images\cam\web1.jpg");
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;

            bitmap.StreamSource = stream;
            bitmap.EndInit();
            stream.Close();

            Application.Current.Dispatcher.Invoke(() =>
            {
                imgCam.Source = bitmap; // функция для анимации переключения картикни
                DoubleAnimation dblAnim = new DoubleAnimation(); // создаем экземпляр класс с анимацей :)
                dblAnim.From = 0.0; //указываем откуда начинатся аниамация
                dblAnim.To = 1.0;// указываем когда заканчивает , но я так понимаю это шаг
                dblAnim.Duration = new Duration(TimeSpan.FromMilliseconds(500)); // продолжительность анимации
                imgCam.BeginAnimation(Image.OpacityProperty, dblAnim); // указываем для имджкама, использовать класс анимации 

            });

        }

        //==========================Блок кнопок
        //======================================

        //Функция кнопки что запускает соединение с комппортом
        private void btnOne_Click(object sender, RoutedEventArgs e)
        {
            ComPortClass.StartComPort(); // Метод для соедниения с сом-портом и Ардуино
            lblPortData.Content = "Соединение установлено";
            btnOne.Visibility = Visibility.Hidden;
            ButClosePort.Visibility = Visibility.Visible;
          //  BrStatus.Fill = (System.Windows.Media.Brush)this.TryFindResource("StOn");
        }
        //Функция кнопки что закрывает соединение с комппортом
        private void ButPortClose(object sender, RoutedEventArgs e)
        {
            
            ComPortClass.CloseComPort();
          
            Console.WriteLine("Закрыли порт");
            lblPortData.Content = "Соединение закрыто";
            TextBoxListSet("Закрыли Com Pot");
            TextBox.Text = "Закрыли Com порт";
            lblPortData.Content = "Ожидаем ... ";
          //  BrStatus.Fill = (System.Windows.Media.Brush)this.TryFindResource("StatusOff");
            ButClosePort.Visibility = Visibility.Hidden;
            btnOne.Visibility = Visibility.Visible;
           

        }

        //Функция кнопки что запускает слушатель сокета

        private void btnZero_Click(object sender, RoutedEventArgs e)
        {
            try {

                if (DebugStatus == 1)
                {
                    SoketMyClass.adresIP = "192.168.1.35";
                      LogBox.Items.Add(DateTime.Now.ToLocalTime() + " включен режим дебага сокета");

                }
                else { SoketMyClass.adresIP = "192.168.1.155"; }
                Console.WriteLine("ardres :" + SoketMyClass.adresIP);
                Thread myThread = new Thread(new ThreadStart(SoketMyClass.OpenSocet)); ;
                myThread.Start(); // запускаем поток
                SocetStatus.Fill = (System.Windows.Media.Brush)this.TryFindResource("StOn");
              
              
            }
            catch (Exception ex) 
            {
                MessageBox.Show("Получили ошибку открытия сокета" + ex.StackTrace);
            }
        }

        //Функция кнопки что отправляет набранное сообщение в порт компа (сом порт для Ардуино)
        public static void SendtoCom(string Csend)
        {
            // MessageBox.Show("Send to Arduino " + Csend, "Внимание");
            try
            {
               // port.Write(Csend);

            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                //Console.ReadLine(); return; 
                //    MessageBox.Show("Error in  +" + e.Message,
                //                      "вот оно че");
            }

        }

        private void BtSendOne_1(object sender, RoutedEventArgs e)
        {

            LongLogCut();
            Console.WriteLine("Отправили 2");

        }

        //  System.Windows.Forms.NotifyIcon nIcon = new System.Windows.Forms.NotifyIcon();
        private void BtSendTwo_2(object sender, RoutedEventArgs e)
        {
            //this.WindowState = System.Windows.WindowState.Minimized;
            //this.nIcon.Icon = new Icon(@"../../Cartman-General.ico");
            //this.nIcon.ShowBalloonTip(5000, "Hi", "This is a BallonTip from Windows Notification", System.Windows.Forms.ToolTipIcon.Info);
            // nIcon.ShowBalloonTip(500, "Сообщение", "Я свернулась:)", System.Windows.Forms.ToolTipIcon.Warning);

            //port.Write("3");
            //Console.WriteLine("Отправили 3");
            //LoadDateCom();
        }

        private void BtSendSix_2(object sender, RoutedEventArgs e)
        {
            ComPortClass.AddSendPull("1");
           ComPortClass.AddSendPull("11");
          
         
            Console.WriteLine("Отправили комбо из 1 и 11");
        }

        private void Button_Click(object sender, RoutedEventArgs e) //кнопка выбор 0 компорта
        {
            try
            {
                num = 0;
                btCom0.Visibility = Visibility.Hidden;
                btCom1.Visibility = Visibility.Hidden;
                btCom1_Copy.Visibility = Visibility.Hidden;
                TextBoxListSet("Установлено: " + ComPortClass.ports[0]);
                TextBox.Text = ("Установлено: " + ComPortClass.ports[0]);
            }
            catch(Exception err) {
                MessageBox.Show("ловим бага -" + err.Message + "/n" + err.StackTrace);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) //кнопка выбор 1 компорта
        {
            try
            {
                num = 1;
            btCom0.Visibility = Visibility.Hidden;
            btCom1.Visibility = Visibility.Hidden;
            btCom1_Copy.Visibility = Visibility.Hidden;
            TextBox.Text = ("Установлено: " + ComPortClass.ports[1]);
                TextBoxListSet("Установлено: " + ComPortClass.ports[1]);
            }
            catch (Exception err)
            {
                MessageBox.Show("ловим бага -" + err.Message + "/n" + err.StackTrace);
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e) //кнопка выбор 2 компорта
        {
            try
            {
                num = 2;
            btCom0.Visibility = Visibility.Hidden;
            btCom1.Visibility = Visibility.Hidden;
            btCom1_Copy.Visibility = Visibility.Hidden;
            TextBox.Text = ("Установлено: " + ComPortClass.ports[2]);
                TextBoxListSet("Установлено: " + ComPortClass.ports[2]);
            }
            catch(Exception err) {
                MessageBox.Show("ловим бага -" + err.Message + "/n" + err.StackTrace);
            }
}

        private void indiprogres(object sender, RoutedPropertyChangedEventArgs<double> e) // методо отображения прогрессбара
        {
            textBlock1.Text = progressBar.Value.ToString();
        }

        private void ClozeWindows(object sender, EventArgs e) // закрытие формы(программы)
        {

            /* MessageBox.Show("Закрываем прилоежние.",
                 "Внимание",
     MessageBoxButton.YesNo, MessageBoxImage.Question);*/

            System.Environment.Exit(1);
        }

        private void SendIn_Click(object sender, RoutedEventArgs e)
        {
           // string SendArduino = "";
           string SendArduino = InputText.Text.ToString();
            //   MessageBox.Show("Инпут текст равен :" + SendArduino, "Внимание");
            ComPortClass.AddSendPull(SendArduino);
        }



        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

            chekCam = 1;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {

            chekCam = 0;
        }
        // функция запроса от ардуины по комп порту данных, шлем ему команду и потом вызываем разбор данных (свою функцию)
        private void LoadDateCom()
        {
           
            ComPortClass.AddSendPull("1");
           
            Console.WriteLine("Отправлили в Компорт: 1");
          
        }


        public void LongLogCut()
        {
            // MessageBox.Show("Запустили обрезку","Внимание");
            for (int i = 1; i < 10; i++)
            {
                //  MessageBox.Show("Запустили готовимся читать  строку {}"+i, "Внимание");
           //     List<string> linesCut = File.ReadLines(writePath[i]).Reverse().Take(1444).ToList();
            //    linesCut.Reverse();
                //  MessageBox.Show("прочитали и обрезали строку {}" + i+ linesCut[i], "Внимание");
           //     using (StreamWriter sw = new StreamWriter(writePath[i], false, System.Text.Encoding.Default))
                {
                    //    MessageBox.Show("готовимся записать обрезанную {}" + i, "Внимание");
              //      foreach (string str in linesCut)
                    {

                //        sw.WriteLine(str);
                    }
                    //    MessageBox.Show("записали{}" + i, "Внимание");
                }
            }
            for (int i = 1; i < 9; i++)
            {
          //      List<string> linesCut = File.ReadLines(writePath2[i]).Reverse().Take(1444).ToList();
             //   linesCut.Reverse();
            //    using (StreamWriter sw = new StreamWriter(writePath2[i], false, System.Text.Encoding.Default))
                {
          //          foreach (string str in linesCut)
                  {
               //         sw.WriteLine(str);
                    }
                }

            }

        }
       
 
  
        //функция что собираетс данные с сокета для рисования графика. и пердает их на вторую форму через общий арай массив
        private void solarMassBase()
        {
            //MessageBox.Show("Пробуем собрать массив для графика солара ", "Внимание");

            if (countsolarMassive != 10)

            {
                // MessageBox.Show("Массив не меньше 10 " + splitDataSoc[6], "Внимание");

                //double sendder = double.Parse(splitDataSoc[6]);
                String datagraf = DateTime.Now.ToLongTimeString();
                double sendder = Convert.ToDouble(splitDataSoc[6].Replace(".", ","));
                // MessageBox.Show(" Записали данные в массив " + sendder, "Внимание");
                /*
                if (sendder < 0)
                {
                    MessageBox.Show("Если пришло малое значение пишем его в 0 " + sendder, "Внимание");
                    return;

                    sendder = countsolarMassive;
                  
                }
                */
                //int asrs = (int)sendder;
                for (int n = (CoMass.Length - 1); n > 0; n--)
                {
                    CoMass[n] = CoMass[(n - 1)];
                    CoMassData[n] = CoMassData[(n - 1)];
                }
                CoMass[0] = sendder;
                CoMassData[0] = datagraf;
                //  MessageBox.Show("Закидываем полученную цифру в соответсубщих масив-место " + asrs, "Внимание");
                countsolarMassive++;
            }
            else
            {
                countsolarMassive = 0;
            }
        }
        private void SolarValureProgressBarAmp(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            textsolarA.Text = solarProgress.Value.ToString();
        
        }
           

        private void SolarValureProgressBarVolt(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            textsolarV.Text = solarProgressVolt2.Value.ToString();
        }
       
        // метод отрабаывает когда чекбокс автообновления включен
        private void CheckBox_Checked1(object sender, RoutedEventArgs e)
        {
            AvtDataCom = 1;
        }

        // метод отрабаывает когда чекбокс автообновления выключен
        private void CheckBox_Unchecked1(object sender, RoutedEventArgs e)
        {
            AvtDataCom = 0;
        }

        //метод что проверяет если чекбокс автообновление данных включен, генерирует запрос в компорт === ДОХЛЫЙ НАДО УБИТЬ
        public void autoSendZaprocSocet()
        {
            if (AvtDataCom == 1 && ComPortClass.StatusComPort==1)
            {
                TextBox.Text = "Запрашиваем данные";
                TextBoxListSet("Запрашиваем данные");
                try
                {
                    //2//  SendtoCom("1");
                    // port.Write("1");
                    //2//    System.Threading.Thread.Sleep(1000);
                    //2//  returnMessage = port.ReadLine();


                    Console.WriteLine("Отправили AutoSend");
                    LoadDateCom();
                }
                catch (Exception e)
                {
                    //  Console.WriteLine("Error: " + e.Message); Console.ReadLine(); return; 
                    MessageBox.Show("Автосэндер данных из компопрта выдал ошибку: " + e.Message,
                                          "вот оно че");
                }
            }
        }

        int polarEnable = 0; // кнопка для открытия формы с графиком солнечной панели и ее переменная-счетчк открытия
        private void ShowBar(object sender, RoutedEventArgs e)
        {

            string win = "";
            foreach (Window window in Application.Current.Windows)
            {

                win += window.ToString();

                if (window.Title == "Монитор Солнечной панели")
                {
                    window.Close();
                    //wingraptab.Close();
                    polarEnable = 1;

                    //  MessageBox.Show(win);
                    break;
                };

            }
            if (polarEnable == 0)
            {
                Bar1 barTabSwhov = new Bar1();
                barTabSwhov.Owner = this;
                barTabSwhov.WindowStartupLocation = System.Windows.WindowStartupLocation.Manual;

                double Top = this.Top;
                double Left = this.Left;
                barTabSwhov.Left = this.Left + (this.Height + 200);
                barTabSwhov.Top = this.Top;
                barTabSwhov.Show();

            }
            else { polarEnable = 0; }


        }

        int wingraptableEnable = 0;  // кнопка для открытия формы с управлением домом :)  и ее переменная-счетчк открытия
        private void ShowWGTab(object sender, RoutedEventArgs e)
        {

            /*
            wingraptab.Close();
            if (wingraptableEnable == 1)
            {
                wingraptab.Close();
                wingraptab.Close();
                wingraptableEnable = 0;
            }
            */

            string win = "";
            foreach (Window window in Application.Current.Windows)
            {

                win += window.ToString();

                if (window.Title == "Умный дом")
                {
                    window.Close();
                    //wingraptab.Close();
                    wingraptableEnable = 1;

                    //  MessageBox.Show(win);
                    break;
                };

            }
            if (wingraptableEnable == 0)
            {
                WinGraphTab wingraptab = new WinGraphTab();
                wingraptab.Owner = this;
                wingraptab.WindowStartupLocation = System.Windows.WindowStartupLocation.Manual;

                double Top = this.Top;
                double Left = this.Left;
                wingraptab.Left = this.Left;
                wingraptab.Top = this.Top - (this.Width - 250);
                wingraptab.Show();

                //MessageBox.Show(wingraptableEnable.ToString());

            }
            else { wingraptableEnable = 0; }

        }

        int graptableEnable = 0;  // кнопка для открытия формы графика :)  и ее переменная-счетчк открытия
        private void ShowGraf(object sender, RoutedEventArgs e)
        {
            string labelWindows = "";
            foreach (Window windows in App.Current.Windows)
            {
                labelWindows += windows.ToString();
                if (windows.Title == "График")
                {
                    windows.Close();
                    graptableEnable = 1;
                    break;
                };
            }
            if (graptableEnable == 0)
            {
                FromGraf fromgraf = new FromGraf();
                fromgraf.Owner = this;
                fromgraf.WindowStartupLocation = System.Windows.WindowStartupLocation.Manual;
                double Top = this.Top;
                double Left = this.Left;

                fromgraf.Left = this.Left - 520;
                fromgraf.Top = Top;
                fromgraf.Show();
            }
            else { graptableEnable = 0; }
        }

        private void cliclMouse_open(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            BaseForm baseform1 = new BaseForm();
            baseform1.Owner = this;
            baseform1.Height = 800;
            baseform1.Width = 800;


            //Viewbox boxPrivew = new Viewbox();
            //boxPrivew.StretchDirection = StretchDirection.Both;
            //boxPrivew.Stretch = Stretch.Fill;
            //boxPrivew.MaxWidth = 400;
            //boxPrivew.MaxHeight = 400;
            //Image zoomeimage = new Image();
            // zoomeimage.Source = BitmapFrame.Create(new Uri(@"D:\wamp2\www\3\images\cam\web1.jpg"));
            //zoomeimage.Source = BitmapFrame.Create(new Uri(@"E:\ai_back.jpg"));

            //  Image myImage = new Image();
            //  myImage.Width = 200;


            //  // Create source
            //  BitmapImage myBitmapImage = new BitmapImage();

            //  // BitmapImage.UriSource must be in a BeginInit/EndInit block
            //  myBitmapImage.BeginInit();
            //  myBitmapImage.UriSource = new Uri(@"D:\wamp2\www\3\images\cam\web1.jpg");

            //  // To save significant application memory, set the DecodePixelWidth or  
            //  // DecodePixelHeight of the BitmapImage value of the image source to the desired 
            //  // height or width of the rendered image. If you don't do this, the application will 
            //  // cache the image as though it were rendered as its normal size rather then just 
            //  // the size that is displayed.
            //  // Note: In order to preserve aspect ratio, set DecodePixelWidth
            //  // or DecodePixelHeight but not both.
            //  myBitmapImage.DecodePixelWidth = 200;
            //  myBitmapImage.EndInit();
            //  //set image source
            //  // myImage.Source = myBitmapImage;

            //myImage.Source = myBitmapImage;

            //boxPrivew.Child = myImage;

            //baseform1 container = panel;
            //baseform1.AddChild(boxPrivew);

            //container = this;
            //container.AddChild(panel);

            // boxPrivew.Stretch = Stretch.Fill;
            //dynamicViewbox.Child = redCircle;
            // RootLayout.Children.Add(dynamicViewbox);
            baseform1.Show();

        }

        private void debug_click_chek(object sender, RoutedEventArgs e) // функция что перевод  сокет из сервера на дебаг режим
        {
            if (DebugStatus == 0)
            {
                DebugStatus = 1;
            }
            else { DebugStatus = 0; };
            Console.WriteLine("Сокет открыт для: " + DebugStatus);
        }

       
    }
}
