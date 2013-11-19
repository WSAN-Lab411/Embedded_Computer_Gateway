/********************************************************************************
 * Tên dự án        : Emboard
 * Nhóm thực hiện   : Sigate
 * Phòng nghiên cứu : Hệ nhúng nối mạng
 * Trường           : Đại Học Bách Khoa Hà Nội
 * Mô tả chung      : 1. Chương trình thu thập dữ liệu nhiệt độ, độ ẩm từ các sensor 
 *                    2. Ra quyết định điều khiển đến các actor phục vụ chăm sóc lan và cảnh báo cháy rừng
 *                    3. Chuyển tiếp dữ liệu về Web server để quản lý và theo dõi qua internet
 * IDE              : Microsoft Visual Studio 2008
 * Target Platform  : Window CE Device
 * *****************************************************************************/
///<sumary>
///Tên file        : libcom.cs
///Tên class       : libcom
///Mô tả chung     : 1. Đọc dữ liệu từ cổng COM, bóc tách dữ liệu
///                  2. Chuyển tiếp dữ liệu lên webserver
///                  3. Tự động điều khiển actor và gửi tin nhắn cảnh báo
///</sumary>

///<sumary>
/// Định nghĩa các chỉ thị tiền xử lý
/// USE_CANBANGTAI : Khi muốn sử dụng chức năng điều phối thời gian bật các van
/// NO_CANBANGTAI  : Không sử dụng chức năng cân bằng tải
/// BC_COOR        : Khi đặt actor báo cháy là coordinator
/// ACTOR_COOR     : Khi đặt actor tưới cây là coordinator
///</sumary>
#define USE_CANBANGTAI
//#define NO_CANBANGTAI         
#define BC_COOR  
//#define ACTOR_COOR

using System;
using System.IO.Ports;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Xml;
using System.Collections;
using System.Threading;

namespace Emboard
{
     class libCOM : libDraw
     {

         public TextBox tb;
         public Panel mypanel;
         public TextBox mytext;

         #region Bien thoi gian tuoi cay
         /// <value>
         /// Thuộc tính này tạo 1 timer để tính thời gian tự động điều khiển actor
         /// </value>
         private System.Windows.Forms.Timer timerTuoiCay = new System.Windows.Forms.Timer();

         /// <value>
         /// Các thuộc tính này tự động tăng khi timer quét để tính thời gian tắt van
         /// </value>
         private int time_count_val1 = 0;
         private int time_count_val2 = 0;
         private int time_count_val3 = 0;
         private int time_count_val4 = 0;
         private int time_count_val5 = 0;
         private int time_count_val6 = 0;

         /// <value>
         /// Thuộc tính này là thời gian tự động tắt van (Thời gian lấy từ cơ sở dữ liêu)
         /// </value>
         private static int time_control = 0;
         public int Time_control {
             set { time_control = value; }
             get { return time_control; }
         }

         /// <value>
         /// Thuộc tính này là thời gian tự động update trạng thái đồng hồ báo cháy (Lấy từ CSDL)
         /// </value>
         private static int time_alarm = 0;
         public int Time_alarm
         {
             set { time_alarm = value; }
             get { return time_alarm; }
         }

        #endregion

        #region Trang thai Van
         /// <value>
         /// Thuộc tính này trạng thái bật hay tắt của các van
         /// Bật : true
         /// Tắt : false
         /// </value>
        private bool onofVal1 = false;
        private bool onofVal2 = false;
        private bool onofVal3 = false;
        private bool onofVal4 = false; 
        private bool onofVal5 = false;
        private bool onofVal6 = false;
        #endregion

        #region COM
         /// <value>
         /// Thuộc tính này cổng COM gửi dự liệu xuống module SMS
         /// </value>
        public SerialPort comSMS = new SerialPort();

         /// <value>
         /// Thuộc tính này cổng COM truyền nhận dữ liệu xuông router emboard
         /// </value>
        public SerialPort comPort = new SerialPort();

        /// <value>
        /// Thuộc tính này dữ liệu nhận về từ cổng COM
        /// </value>
        private string comRead = null;

        /// <value>
        /// Thuộc tính này thời gian xóa dữ liệu trên textbox hiển thị thông tin
        /// </value>
        private int count = 0;

        /// <value>
        /// Thuộc tính này chỉ số chọn tab trọng giao diện
        /// 0 : tab connect
        /// 1 : tab setting
        /// 2 : tab map
        /// </value>
        private int index = 0;
        public int Index
        {
            set { index = value; }
            get { return index; }
        }
        #endregion

        #region sensor

         /// <value>
         /// Thuộc tính này nhiệt độ sensor tính được từ dữ liệu nhận từ cổng COM
         /// </value>
        private float temperature = 0;
        public float Temperature
        {
            set { temperature = value; }
            get { return temperature; }
        }

        /// <value>
        /// Thuộc tính này độ ẩm sensor tính được từ dữ liệu nhận từ cổng COM
        /// </value>
        private float humidity = 0;
        public float Humidity
        {
            set { humidity = value; }
            get { return humidity; }
        }

        /// <value>
        /// Thuộc tính này năng lượng tính được từ  dữ liệu nhận từ cổng COM
        /// </value>
        private float energy = 0;
        public float Energy {
            set { energy = value; }
            get { return energy; }
        }
        #endregion

        #region VariBaochay
         /// <value>
         /// Thuộc tính này thời gian update trạng thái đồng hồ báo cháy (tự động tăng theo time)
         /// </value>
        #if BC_COOR
        private int count_BC = 0;
        #endif
        #endregion

        #region time bat bom
        #if USE_CANBANGTAI
        /// <value>
        /// Thuoc tính này thơi gian đã bật van (lấy từ CSDL)
        /// </value>
        private int timeval1 = 0;
        private int timeval2 = 0;
        private int timeval3 = 0;
        private int timeval4 = 0;
        private int timeval5 = 0;
        #endif
        #endregion

        #region thread
         /// <value>
         /// Thuộc tính này luồng lấy dữ liệu từ web server
         /// </value>
        public Thread threadRequest;
         /// <value>
         /// Thuộc tính này luồng gửi dũ liệu lên web server
         /// </value>
        public Thread threadSend;

         /// <value>
         /// Thuộc tính này hàng đợi dữ liệu gửi lên web
         /// </value>
        Queue stringRead = new Queue();

        public Hashtable timeDapung = new Hashtable(); 
         /// <value>
         /// Thuộc tính này dữ liệu nhận từ web
         /// </value>
        private string comReadWeb = null;

        #endregion
        #region variable Ping
        private static int[] timePing = new int[30];
        private static bool[] checkPing = new bool[30];
        private static int[] countTime = new int[30];
        #endregion
        public libCOM()
        {
            comPort.ReceivedBytesThreshold = 1;
            comPort.DataReceived += new SerialDataReceivedEventHandler(comPort_DataReceived);
        }
        #region OpenPort
        public int OpenPort(string portName, int baudRate, string parity, string stopbits, int databits)
        {
            try
            {
                if (comPort.IsOpen == true) 
                    comPort.Close();
                comPort.PortName = portName;
                comPort.BaudRate = baudRate;
                comPort.Parity = (Parity)Enum.Parse(typeof(Parity), parity, true);
                comPort.StopBits = (StopBits)Enum.Parse(typeof(StopBits), stopbits, true);
                comPort.DataBits = databits;
                comPort.Open();
                return 1;
            }
            catch
            {
                return -1;
            }
        }
        #endregion

        #region WriteData
        public void WriteData(byte[] com)
        {
            try
            {
                if (comPort.IsOpen == true)
                {
                    comPort.Write(com, 0, com.Length);
                }
            }
            catch
            {
                DisplayData("Khong the gui du lieu" + DateTime.Now + "\r\n", tb);
            }
        }
        #endregion
       
        private void comPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                if (comPort.IsOpen == true)
                {
                    comRead = comPort.ReadLine();
                    //comRead = comPort.ReadExisting();
                 }
                if(comRead[0] != '#' && comRead[0] != 'R')
                    DisplayData(comRead, tb);
                if ((comRead.Length >= 6) && (comRead[0] == '#'))
                {
                    Converter(comRead);
                    stringRead.Enqueue(comRead);
                }
            }
            catch
            {
                DisplayData("\r\nKhong nhan duoc du lieu " + DateTime.Now + "\r\n", tb);
            }
        }

        #region
        public void COMClose()
        {
            try
            {
                if (comPort.IsOpen == true)
                {
                    comPort.Close();
                }
                if (comPort.IsOpen == false)
                {
                    DisplayData("(" + DateTime.Now + "):COM da dong\r\n", tb);
                }
            }
            catch
            {
                DisplayData("Khong the dong COM " + DateTime.Now + "\r\n", tb);
            }
        }
        #endregion
        
        /******************************************
         * Giai ma ban tin tra ve tu cong COM
         * Tham so dau vao: Chuoi du lieu nhan tu cong COM
         * Kieu tra ve : void
         * Dinh dang ban tin tra ve:
         * #JN:NNNN MM  Ban tin join mang
         * #AD:NNNN MM D1D2D3D4 D5D6D7D8 E1E2E3E4 Ban tin du lieu dinh ky
         * #RD:NNNN MM D1D2D3D4 D5D6D7D8 E1E2E3E4 Ban tin du lieu theo yeu cau
         * #VL:MM Ban tin thong bao ngu cua mot sensor
         * #OK:NNNN MM SS Ban tin thong bao trang thai actor
         * #SN: NNNN MM SS  Ban tin thong bao trang thai sensor
         * ***************************************/
        public void Converter(string mesg)
        {
            try
            {
                Database myDatabase = new Database();
                switch (mesg[1])
                {
                    case 'J':
                        JoinMang(mesg);
                        reload(pictureBox);
                        break;
                    case 'A':
                        // VD #AD: NNNN MM D1D2D3D4 D5D6D7D8 EEEE
                        string mac = mesg.Substring(8,2);
                        string status = "";
                        if (mac[0] == '0')
                        {
                            status = myDatabase.getStatusSensor(mac);
                        }
                        AutoData(mesg);
                        if (status != "true" && status != "True")
                        {
                            reload(pictureBox);
                        }
                        break;
                    case 'S':
                        // VD: #SN: NNNN MM SS
                        TrangThaiNode(mesg);
                        break;
                    case 'V':
                        try
                        {
                            DisplayData("(" + DateTime.Now + "): Sensor " + mesg.Substring(4, 2) + " da vao che do ngu\r\n", tb);
                            myDatabase.setActiveSensor(mesg.Substring(4, 2), false);
                            reload(pictureBox);
                        }
                        catch
                        {
                            DisplayData("Error", tb);
                        }
                        break;
                    case 'O':
                        // VD: #OK:NNNN MM SS
                        ThongTinActor(mesg);
                        reload(pictureBox);
                        break;
                    case 'R':
                        // VD #RD: NNNN MM D1D2D3D4 D5D6D7D8 EEEE
                        RequestData(mesg);
                        break;
                    case 'P':
                        PingNode(mesg);
                        break;
                }
            }
            catch
            {
                DisplayData("Khong the giai ma du lieu", tb);
            }
        }
        //Ham boc tach thong so khi join mang
        public void JoinMang(string mesg)
        {
            try
            {
                Database mydatabase = new Database();
                string check = mesg.Substring(8, 2);
                int chck = int.Parse(check, System.Globalization.NumberStyles.HexNumber);
                if (chck < 160 && chck > 0)
                {
                    DisplayData("(" + DateTime.Now + "): Thong tin gia nhap mang: \r\n Sensor " + mesg.Substring(4, 4) + " (" + check + ") " + " : \r\n Da gia nhap vao mang !!!\r\n", tb);
                    if (check[0] == '3')
                    {
                        if (mydatabase.CheckSensorBC(check) == "true")
                        {
                            mydatabase.setNetworkIpSensorBC(check, mesg.Substring(4, 4));
                            mydatabase.setStatusSensorBC(check, true);
                        }
                        else
                        {
                            mydatabase.setSensor_bc(check, mesg.Substring(4, 4), true);
                        }
                    }
                    else
                    {
                        timePing[chck] = 0;
                        if (mydatabase.CheckSensor(check) == "true")
                        {
                            mydatabase.setNetworkIpSensor(check, mesg.Substring(4, 4));
                            mydatabase.setActiveSensor(check, true);
                        }
                        else
                        {
                            mydatabase.setNodeSensor(check, mesg.Substring(4, 4), true);
                        }
                    }
                }
                if ((160 < chck && chck < 255) || chck == 0)
                {
                    DisplayData("(" + DateTime.Now + "): Thong tin trang thai Actor: \r\n Actor " + mesg.Substring(4, 4) + " (" + check + ") " + " : \r\n Van hoat dong trong mang !!!\r\n", tb);
                    if (mydatabase.CheckActor(check) == "true")
                    {
                        mydatabase.setNetworkIpActor(check, mesg.Substring(4, 4));
                        mydatabase.setStatusActor(check, true);
                    }
                    else
                    {
                        mydatabase.setNodeActor(check, mesg.Substring(4, 4), true);
                    }
                }
            }
            catch
            {
                DisplayData("Error Join mang", tb);
            }
        }
         //Hamf boc tach ban tin ping node mang
        public void PingNode(string mesg)
        {
            try
            {
                Database mydatabase = new Database();
                string check = mesg.Substring(8, 2);
                int chck = int.Parse(check, System.Globalization.NumberStyles.HexNumber);
                if (chck < 160 && chck > 0)
                {
                    DisplayData("(" + DateTime.Now + "): Thong tin ping node mang: \r\n Sensor " + mesg.Substring(4, 4) + " (" + check + ") " + " : \r\n Van hoat dong trong mang !!!\r\n", tb);
                    if (check[0] == '3')
                    {
                         mydatabase.setNetworkIpSensorBC(check, mesg.Substring(4, 4));
                         mydatabase.setStatusSensorBC(check, true);
                    }
                    else
                    {
                        timePing[chck] = 0;
                        checkPing[chck] = false;
                        countTime[chck] = 0;
                        mydatabase.setNetworkIpSensor(check, mesg.Substring(4, 4));
                        mydatabase.setActiveSensor(check, true);
                    }
                }
            }
            catch
            {
                DisplayData("Error Ping node", tb);
            }
        }
        // Ham tinh toan nhiet do, do am va nang luong cua sensor
        public void DataSensor(string mesg, ref float t, ref float h, ref float e)
        {
            try
            {
                float humi = 0;
                float temp = 0;
                float ener = 0;
                string hexd = mesg.Substring(10, 4);
                int td = int.Parse(hexd, System.Globalization.NumberStyles.HexNumber);
                hexd = mesg.Substring(14, 4);
                int rhd = int.Parse(hexd, System.Globalization.NumberStyles.HexNumber);
                hexd = mesg.Substring(18, 4);
                int rpd = int.Parse(hexd, System.Globalization.NumberStyles.HexNumber);
                float rpd1 = ((float)rpd / (float)4096);
                float rh_lind;// rh_lin:  Humidity linear 
                temp = (float)(td * 0.01 - 39.6);                  				//calc. temperature from ticks to [deg Cel]	
                rh_lind = (float)(0.0367 * rhd - 0.0000015955 * rhd * rhd - 2.0468);     	//calc. humidity from ticks to [%RH]
                humi = (float)((Temperature - 25) * (0.01 + 0.00008 * rhd) + rh_lind);   		//calc. temperature compensated humidity [%RH]
                ener = (float)(0.78 / rpd1);                                 //calc. power of zigbee
                if (humi > 100) humi = 100;       				//cut if the value is outside of
                if (humi < 0.1) humi = 0;

                t = temp;
                h = humi;
                e = ener;
            }
            catch
            { }
        }
        //Ham boc tach du lieu khi nhan du lieu dinh ky
        public void AutoData(string mesg)
        {
            try
            {
                Database mydatabase = new Database();
                string mac = mesg.Substring(8, 2);
                DataSensor(mesg, ref temperature, ref humidity, ref energy);
                DisplayData("(" + DateTime.Now + "): Du lieu dinh ky :\r\n Sensor " + mesg.Substring(4, 4) + "(" + mac + "): \r\n     Nhiet do: " + Temperature + "\r\n     Do am: " + Humidity + "\r\n     Nang luong : " + Energy + "\r\n", tb);
                string time = DateTime.Now.ToString();
                if (mac[0] == '3')
                {
                    if (mydatabase.CheckSensorBC(mac) == "true")
                    {
                        mydatabase.setNetworkIpSensorBC(mac, mesg.Substring(4, 4));
                        mydatabase.setStatusSensorBC(mac, true);
                    }
                    else
                    {
                        mydatabase.setSensor_bc(mac, mesg.Substring(4, 4), true);
                    }
                    mydatabase.SaveDataDB(Temperature, Humidity);
                }
                else
                {
                    int iIndex = int.Parse(mac,System.Globalization.NumberStyles.HexNumber);
                    timePing[iIndex] = 0;
                    if (mydatabase.CheckSensor(mac) == "true")
                    {
                        mydatabase.setNetworkIpSensor(mac, mesg.Substring(4, 4));
                        mydatabase.setActiveSensor(mac, true);
                    }
                    else
                    {
                        mydatabase.setNodeSensor(mac, mesg.Substring(4, 4), true);
                    }
                    string Val = mydatabase.getVanSensor(mac);
                    int van = Int32.Parse(Val.Substring(1,1));
                    float tempmax = mydatabase.getTempVan(van);
                    float humimax = mydatabase.getHumiVan(van);
                    bool test = ((Temperature > tempmax) && (Humidity < humimax));
                    if (test) //Dieu kien thoa man
                    {
                    if (DateTime.Now.Hour >= mydatabase.getTimeStart() && DateTime.Now.Hour < mydatabase.getTimeFinish())
                     {
                     #if USE_CANBANGTAI
                        
                         DisplayData("Thoa man dieu kien bat van (" + mydatabase.getTimeStart() + " h - " + mydatabase.getTimeFinish() + " h)", tb);
                        if(Val == "V6")
                        {
                            DisplayData("Thoa man dieu kien bat van 6 (" + mydatabase.getTimeStart() + " h - " + mydatabase.getTimeFinish() + " h)", tb);
                            AutoBatVan(Val, mesg);
                        }
                        else if (Val == "V5")
                        {
                            int tvan4 = mydatabase.getTimeVan(4);
                            int tvan5 = mydatabase.getTimeVan(5);
                            if (tvan4 > 60)
                            {
                                int tv4phut = tvan4 / 60;
                                int tv4giay = tvan4 - tv4phut * 60;
                                DisplayData("Thoi gian da bat van 4 la:" + tv4phut + " phut "+ tv4giay + " giay", tb);
                            }
                            else
                            {
                                DisplayData("Thoi gian da bat van 4 la:" + tvan4 + " giay", tb);
                            }
                            if (tvan5 > 60)
                            {
                                int tv5phut = tvan5 / 60;
                                int tv5giay = tvan5 - tv5phut * 60;
                                DisplayData("Thoi gian da bat van 5 la:" + tv5phut + " phut " + tv5giay + " giay", tb);
                            }
                            else
                            {
                                DisplayData("Thoi gian da bat van 5 la:" + tvan5 + " giay", tb);
                            }
                            if (tvan4 > tvan5)
                            {
                                DisplayData("Quyet dinh ban van 5:", tb);
                                AutoBatVan("V5", mesg);
                            }
                            else
                            {
                                DisplayData("Quyet dinh ban van 4:", tb);
                                AutoBatVan("V4", mesg);
                                Val = "V4";
                            }
                        }
                        else if (Val == "V1")
                        {
                            int tvan1 = mydatabase.getTimeVan(1);
                            int tvan2 = mydatabase.getTimeVan(2);
                            if (tvan1 > 60)
                            {
                                int tv1phut = tvan1 / 60;
                                int tv1giay = tvan1 - tv1phut * 60;
                                DisplayData("Thoi gian da bat van 1 la:" + tv1phut + " phut " + tv1giay + " giay", tb);
                            }
                            else
                            {
                                DisplayData("Thoi gian da bat van 1 la:" + tvan1 + " giay", tb);
                            }
                            if (tvan2 > 60)
                            {
                                int tv2phut = tvan2 / 60;
                                int tv2giay = tvan2 - tv2phut * 60;
                                DisplayData("Thoi gian da bat van 2 la:" + tv2phut + " phut " + tv2giay + " giay", tb);
                            }
                            else
                            {
                                DisplayData("Thoi gian da bat van 2 la:" + tvan2 + " giay", tb);
                            }
                            if (tvan1 > tvan2)
                            {
                                DisplayData("Quyet dinh ban van 2:", tb);
                                AutoBatVan("V2", mesg);
                                Val = "V2";
                            }
                            else
                            {
                                DisplayData("Quyet dinh ban van 1:", tb);
                                AutoBatVan("V1", mesg);
                            }
                        }
                        else
                        {
                            string val = Val.Substring(1,1);
                            //int van = int.Parse(val);
                            int vantruoc = van - 1;
                            int vansau = van + 1;
                            int timevan = mydatabase.getTimeVan(van);
                            int timevanTruoc = mydatabase.getTimeVan(vantruoc);
                            int timevanSau = mydatabase.getTimeVan(vansau);
                            if (timevan > 60)
                            {
                                int tvphut = timevan / 60;
                                int tvgiay = timevan - tvphut * 60;
                                DisplayData("Thoi gian da bat van "+van+" la:" + tvphut + " phut " + tvgiay + " giay", tb);
                            }
                            else
                            {
                                DisplayData("Thoi gian da bat van " + van+" la:" + timevan + " giay", tb);
                            }
                            if (timevanTruoc > 60)
                            {
                                int tvTphut = timevanTruoc / 60;
                                int tvTgiay = timevanTruoc - tvTphut * 60;
                                DisplayData("Thoi gian da bat van " + vantruoc + " la:" + tvTphut + " phut " + tvTgiay + " giay", tb);
                            }
                            else
                            {
                                DisplayData("Thoi gian da bat van "+ vantruoc +" la:" + timevanTruoc + " giay", tb);
                            }
                            if (timevanSau > 60)
                            {
                                int tvSphut = timevanSau / 60;
                                int tvSgiay = timevanSau - tvSphut * 60;
                                DisplayData("Thoi gian da bat van " + vansau + " la:" + tvSphut + " phut " + tvSgiay + " giay", tb);
                            }
                            else
                            {
                                DisplayData("Thoi gian da bat van " + vansau + " la:" + timevanSau + " giay", tb);
                            }
                            if(timevan > timevanTruoc)
                            {
                                if(timevanTruoc > timevanSau)
                                {
                                    DisplayData("Quyet dinh bat van"+vansau+":",tb);
                                    AutoBatVan("V"+vansau,mesg);
                                    Val = "V" + vansau;
                                }
                                else
                                {
                                    DisplayData("Quyet dinh bat van" + vantruoc + ":", tb);
                                    AutoBatVan("V" + vantruoc, mesg);
                                    Val = "V" + vantruoc;
                                }
                            }
                            else
                            {
                                if (timevan > timevanSau)
                                {
                                    DisplayData("Quyet dinh bat van" + vansau + ":", tb);
                                    AutoBatVan("V" + vansau, mesg);
                                    Val = "V" + vansau;
                                }
                                else
                                {
                                    DisplayData("Quyet dinh bat van" + van + ":", tb);
                                    AutoBatVan("V" + van, mesg);
                                    Val = "V" + van;
                                }
                            }
                        }
                        int timenow = DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second - 1;
                        timeDapung.Remove(Val);
                        timeDapung.Add(Val, timenow);
                     #endif
                     #if NO_CANBANGTAI
                        DisplayData("Thoa man dieu kien bat van "+id+" (" + mydatabase.getTimeStart() + " h - " + mydatabase.getTimeFinish() + " h)", tb);
                        AutoBatVan(Val, mesg);
                    #endif
                     }
                     else
                     {
                           DisplayData("Khong phai khoang thoi gian bat bom (" + mydatabase.getTimeStart() + " h - "+mydatabase.getTimeFinish()+" h)",tb);
                     }
                    }
                }
            }
            catch
            { }
        }
        //Ham boc tach du lieu theo yeu cau
        public void RequestData(string mesg)
        {
            try
            {
                Database mydatabase = new Database();
                DataSensor(mesg, ref temperature, ref humidity, ref energy);
                string _mac = mesg.Substring(8, 2);
                string time1 = DateTime.Now.ToString();
                try
                {
                    int timenow = DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second;
                    int dapung = timenow - (int)timeDapung[_mac];
                    timeDapung.Remove(_mac);
                    DisplayData("(" + DateTime.Now + "): Du lieu yeu cau (Dap ung: " + dapung + " giay): \r\n Sensor " + mesg.Substring(4, 4) + "(" + _mac + "): \r\n     Nhiet do: " + Temperature + "\r\n     Do am: " + Humidity + "\r\n     Nang luong : " + Energy + "\r\n", tb);
                }
                catch
                {
                    DisplayData("(" + DateTime.Now + "): Du lieu yeu cau: \r\n Sensor " + mesg.Substring(4, 4) + "(" + _mac + "): \r\n     Nhiet do: " + Temperature + "\r\n     Do am: " + Humidity + "\r\n     Nang luong : " + Energy + "\r\n", tb);
 
                }
                mydatabase.updateSensor(_mac, mesg.Substring(4, 4), Temperature, Humidity, Energy, time1);
                showdata(_mac, mesg.Substring(4, 4), Temperature, Humidity, Energy, mytext);
                if (_mac[0] == '3')
                {
                    mydatabase.SaveDataDB(Temperature, Humidity);
                }
                else {
                    int iIndex = Int32.Parse(_mac, System.Globalization.NumberStyles.HexNumber);
                    timePing[iIndex] = 0;
                }
            }
            catch
            {
                DisplayData("Error du lieu yeu cau", tb);
            }
        }
  
        public void AutoBatVan(string Val,string mesg)
        {
            try
            {
                if (Val == "V1")
                {
                    if (onofVal1 == false)
                    {
                        DisplayData("Gui lenh bat van 1 tu dong!",tb);
                        // Gui lenh tuoi xuong val tuong ung
                        string command = CreateCommand(true,"00","V1");
                        byte[] com = ConvertTobyte(command);
                        WriteData(com);
                        //Mo Val,bat dau dem thoi gian
                        onofVal1 = true;
                        //dtVal1 = DateTime.Now;
                        time_count_val1 = 0;
                    }
                    else
                    {
                        DisplayData("Van so 1 dang bat!", tb);
                    }
                }
                if (Val == "V2")
                {
                    if (onofVal2 == false)
                    {
                        DisplayData("Gui lenh bat van 2 tu dong!", tb);
                        string command = CreateCommand(true,"00","V2");
                        //MessageBox.Show(command);
                        byte[] com = ConvertTobyte(command);
                        WriteData(com);

                        onofVal2 = true;
                        //dtVal2 = DateTime.Now;
                        time_count_val2 = 0;
                    }
                    else
                    {
                        DisplayData("Van so 2 dang bat!", tb);
                    }
                }
                if (Val == "V3")
                {
                    if (onofVal3 == false)
                    {
                        DisplayData("Gui lenh bat van 3 tu dong!", tb);
                        string command = CreateCommand(true,"00","V3");
                        //MessageBox.Show(command);
                        byte[] com = ConvertTobyte(command);
                        WriteData(com);

                        onofVal3 = true;
                        //dtVal3 = DateTime.Now;
                        time_count_val3 = 0;
                    }
                    else
                    {
                        DisplayData("Van so 3 dang bat!", tb);
                    }
                }
                if (Val == "V4")
                {
                    if (onofVal4 == false)
                    {
                        DisplayData("Gui lenh bat van 4 tu dong!", tb);
                        string command = CreateCommand(true,"00","V4");
                        //MessageBox.Show(command);
                        byte[] com = ConvertTobyte(command);
                        WriteData(com);

                        onofVal4 = true;
                        //dtVal4 = DateTime.Now;
                        time_count_val4 = 0;
                    }
                    else
                    {
                        DisplayData("Van so 4 dang bat!", tb);
                    }
                }
                if (Val == "V5")
                {
                    if (onofVal5 == false)
                    {
                        DisplayData("Gui lenh bat van 5 tu dong!", tb);
                        string command = CreateCommand(true,"00","V5");
                        //MessageBox.Show(command);
                        byte[] com = ConvertTobyte(command);
                        WriteData(com);

                        onofVal5 = true;
                        //dtVal5 = DateTime.Now;
                        time_count_val5 = 0;
                    }
                    else
                    {
                        DisplayData("Van so 5 dang bat!", tb);
                    }
                }
                if (Val == "V6")
                {
                    if (onofVal6 == false)
                    {
                        DisplayData("Gui lenh bat van 6 tu dong!", tb);
                        // Gui lenh tuoi xuong val tuong ung
                        string command = CreateCommand(true,"00","V6");
                        byte[] com = ConvertTobyte(command);
                        WriteData(com);
                        //Mo Val,bat dau dem thoi gian
                        onofVal6 = true;
                        //dtVal1 = DateTime.Now;
                        time_count_val6 = 0;
                    }
                    else
                    {
                        DisplayData("Van so 6 dang bat!", tb);
                    }
                }
            }
            catch 
            {
                DisplayData("Khong the gui lenh bat bom", tb);
            }
        }
        public void AutoSendCanhBao()
        {
            try
            {
                Database mydatabase = new Database();
                float nhietdotb = mydatabase.SumTemp();
                float doamtb = mydatabase.SumHumi();
                    float level = doamtb / (float)20 - (float)(27 - nhietdotb) / (float)10;
                    if (level > 4)
                    {
                        string command = CreateCommand(true,"B1","V4");
                        byte[] com1 = ConvertTobyte(command);
                        WriteData(com1);
                        mydatabase.DeleteData();
                        DisplayData("(" + DateTime.Now + "): Tu dong gui canh bao muc 4\r\n", tb);
                    }
                    if ((2.5 < level) && (level < 4))
                    {
                        string command = CreateCommand(true,"B1","V3");
                        byte[] com = ConvertTobyte(command);
                        WriteData(com);    
                        mydatabase.DeleteData();
                        DisplayData("(" + DateTime.Now + "): Tu dong gui canh bao muc 3\r\n", tb);
                    }
                    if ((2 < level) && (level < 2.5))
                    {
                        string command = CreateCommand(true,"B1","V2");
                        byte[] com = ConvertTobyte(command);
                        WriteData(com);
                        mydatabase.DeleteData();
                        DisplayData("(" + DateTime.Now + "): Tu dong gui canh bao muc 2\r\n", tb);
                    }
                    if (level < 2)
                    {
                        string command = CreateCommand(true,"B1","V1");
                        byte[] com = ConvertTobyte(command);
                        WriteData(com);
                        mydatabase.DeleteData();
                        DisplayData("(" + DateTime.Now + "): Tu dong gui canh bao muc 1\r\n", tb);
                    }
                }
            catch
            {
                DisplayData("Khong the gui muc canh bao", tb);
            }
        }
        public void TrangThaiNode(string mesg)
        {
            try
            {
                Database mydatabase = new Database();
                string ss = mesg.Substring(10, 2);
                switch (ss)
                {
                    case "02":
                        DisplayData("(" + DateTime.Now + "):Canh bao chay tai node " + mesg.Substring(8, 2) +"(" + mesg.Substring(4, 4) + ") \r\n", tb);
                        //string command = CreateCommand(true, "B1", "V5");
                        //string command = "0000051$";
                        //byte[] com = ConvertTobyte(command);
                        //WriteData(com);
                        string SDT = mydatabase.getPhoneNumber();
                        string MAC = mesg.Substring(8, 2);//mydatabase.getMacSensor(mesg.Substring(4, 4));
                        AutoSendSMS(SDT, MAC);
                        int timenow = DateTime.Now.Hour*3600 + DateTime.Now.Minute*60 + DateTime.Now.Second;
                        timeDapung.Remove("V5");
                        timeDapung.Add("V5", timenow);
                        break;
                    case "03":
                        DisplayData("(" + DateTime.Now + "):Het nang luong tai node " + mesg.Substring(8, 2) + "(" + mesg.Substring(4, 4) + ") \r\n", tb);
                        break;
                    case "04":
                        DisplayData("(" + DateTime.Now + "): Phat hien xam nhap tai node " + mesg.Substring(8, 2) + "(" + mesg.Substring(4, 4) + ") \r\n", tb);
                        break;
                }
            }
            catch
            {}
        }
        public void ThongTinActor(string mesg)
        {
            try
            {
                Database mydatabase = new Database();
                string ss1 = mesg.Substring(10, 2);
                string mac_actor = mesg.Substring(8, 2);
                int tt = int.Parse(ss1, System.Globalization.NumberStyles.HexNumber);
                int vanbom = tt - 128;
                if (tt > 128 && tt < 170)
                {
                    if (vanbom == 15)
                    {
                    #if USE_CANBANGTAI
                        if (mydatabase.getStateVal(1) == "off" || mydatabase.getStateVal(1) == "Off")
                        {
                            timeval1 = DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second;
                        }
                        if (mydatabase.getStateVal(2) == "off" || mydatabase.getStateVal(2) == "Off")
                        {
                            timeval2 = DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second;
                        }
                        if (mydatabase.getStateVal(3) == "off" || mydatabase.getStateVal(3) == "Off")
                        {
                            timeval3 = DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second;
                        }
                        if (mydatabase.getStateVal(4) == "off" || mydatabase.getStateVal(4) == "Off")
                        {
                            timeval4 = DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second;
                        }
                        if (mydatabase.getStateVal(5) == "off" || mydatabase.getStateVal(5) == "Off")
                        {
                            timeval5 = DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second;
                        }
                    #endif
                        try
                        {
                            int timenow = DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second;
                            int dapung = 0;
                            dapung = timenow - (int)timeDapung[mac_actor];
                            timeDapung.Remove(mac_actor);
                            DisplayData("(" + DateTime.Now + "):Tat ca cac van da bat! \r\n(Dap ung dieu khien bang tay: " + dapung + " giay)\r\n", tb);
                        }
                        catch
                        {
                            DisplayData("(" + DateTime.Now + "):Tat ca cac van da bat!\r\n", tb);
                        }
                         mydatabase.setValOn();
                    }
                    else
                    {
                        if (mac_actor == "00")
                        {
                        #if USE_CANBANGTAI
                            switch (vanbom)
                            {
                                case 1:
                                    if (mydatabase.getStateVal(1) == "off" || mydatabase.getStateVal(1) == "Off")
                                    {
                                        timeval1 = DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second;
                                    }
                                    break;
                                case 2:
                                    if (mydatabase.getStateVal(2) == "off" || mydatabase.getStateVal(2) == "Off")
                                    {
                                        timeval2 = DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second;
                                    }
                                    break;
                                case 3:
                                    if (mydatabase.getStateVal(3) == "off" || mydatabase.getStateVal(3) == "Off")
                                    {
                                        timeval3 = DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second;
                                    }
                                    break;
                                case 4:
                                    if (mydatabase.getStateVal(4) == "off" || mydatabase.getStateVal(4) == "Off")
                                    {
                                        timeval4 = DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second;
                                    }
                                        break;
                                case 5:
                                        if (mydatabase.getStateVal(5) == "off" || mydatabase.getStateVal(5) == "Off")
                                        {
                                            timeval5 = DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second;
                                        }
                                        break;
                            }
                            #endif
                            int timenow = DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second;
                            int dapung = 0;
                            try
                            {
                                dapung = timenow - (int)timeDapung[mac_actor];
                                timeDapung.Remove(mac_actor);
                                DisplayData("(" + DateTime.Now + "): Van so " + vanbom.ToString() + " da bat!\r\n(Dap ung dieu khien bang tay: " + dapung + " giay)\r\n", tb);
                            }
                            catch
                            {
                                try
                                {
                                    dapung = timenow - (int)timeDapung["V" + vanbom.ToString()];
                                    timeDapung.Remove("V" + vanbom.ToString());
                                    DisplayData("(" + DateTime.Now + "): Van so " + vanbom.ToString() + " da bat!\r\n(Dap ung dieu khien tu dong: " + dapung + " giay)\r\n", tb);

                                }
                                catch
                                {
                                    DisplayData("(" + DateTime.Now + "): Van so " + vanbom.ToString() + " da bat!\r\n", tb);
                                }
                            }
                            
                            mydatabase.setStateVal(vanbom, "on");
                        }
                        if (mac_actor == "B1")
                        {
                            //DisplayData(MessageType.Incoming, comread, tb);
                            int timenow = DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second;
                            int dapung = 0;
                            try
                            {
                                dapung = timenow - (int)timeDapung[mac_actor];
                                timeDapung.Remove(mac_actor);
                                DisplayData("(" + DateTime.Now + "): Da bat canh bao muc " + vanbom.ToString() + "\r\n(Dap ung dieu khien bang tay: " + dapung + " giay)\r\n", tb);
                            }
                            catch
                            {
                                try
                                {
                                    dapung = timenow - (int)timeDapung["V5"];
                                    timeDapung.Remove("V5");
                                    DisplayData("(" + DateTime.Now + "): Da bat canh bao muc " + vanbom.ToString() + "\r\n(Dap ung phat hien chay: " + dapung + "giay)", tb);
                                }
                                catch 
                                {
                                    DisplayData("(" + DateTime.Now + "): Da bat canh bao muc " + vanbom.ToString() + "\r\n", tb);
                                }
                            }
                            
                        }
                    }
                    showVanOn(vanbom, mac_actor, mytext);
                }
                else
                {
                    if (tt > 64 && tt < 128)
                    {
                        int vantat = tt - 64;
                        if (vantat == 15)
                        {
                            DisplayData("(" + DateTime.Now + "): Tat ca cac van da tat tu dong!\r\n", tb);
                            mydatabase.setValOff();
                        }
                        else
                        {
                            DisplayData("(" + DateTime.Now + "): Van so " + vantat.ToString() + " da tat tu dong!\r\n", tb);
                            mydatabase.setStateVal(vantat, "off");
                        }
                    }
                    if(tt < 64)
                    {
                        if (tt == 15)
                        {
                        #if USE_CANBANGTAI
                            if (mydatabase.getStateVal(1) == "on")
                            {
                                int time1 = DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second - timeval1;
                                time1 = time1 + mydatabase.getTimeVan(1);
                                mydatabase.setTimeVan(1, time1);
                            }
                            if (mydatabase.getStateVal(2) == "on")
                            {
                                int time2 = DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second - timeval2;
                                time2 = time2 + mydatabase.getTimeVan(2);
                                mydatabase.setTimeVan(2, time2);
                            }
                            if (mydatabase.getStateVal(3) == "on")
                            {
                                int time3 = DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second - timeval3;
                                time3 = time3 + mydatabase.getTimeVan(3);
                                mydatabase.setTimeVan(3, time3);
                            }
                            if (mydatabase.getStateVal(4) == "on")
                            {
                                int time4 = DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second - timeval4;
                                time4 = time4 + mydatabase.getTimeVan(4);
                                mydatabase.setTimeVan(4, time4);
                            }
                            if (mydatabase.getStateVal(5) == "on")
                            {
                                int time5 = DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second - timeval5;
                                time5 = time5 + mydatabase.getTimeVan(5);
                                mydatabase.setTimeVan(5, time5);
                            }
                         #endif
                            int timenow = DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second;
                            int dapung = 0;
                            try
                            {
                                dapung = timenow - (int)timeDapung[mac_actor];
                                timeDapung.Remove(mac_actor);
                                DisplayData("(" + DateTime.Now + "): Tat ca cac van da tat!\r\n(Dap ung dieu khien bang tay: " + dapung + " giay)\r\n", tb);
                            }
                            catch
                            {
                                DisplayData("(" + DateTime.Now + "): Tat ca cac van da tat!\r\n", tb);
                            }
                            mydatabase.setValOff();
                        }
                        else
                        {
                         #if USE_CANBANGTAI
                            switch (tt)
                            {
                                case 1:
                                    if (mydatabase.getStateVal(1) == "on")
                                    {
                                        int time1 = DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second - timeval1;
                                        time1 = time1 + mydatabase.getTimeVan(1);
                                        mydatabase.setTimeVan(1, time1);
                                    }
                                    break;
                                case 2:
                                    if (mydatabase.getStateVal(2) == "on")
                                    {
                                        int time2 = DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second - timeval2;
                                        time2 = time2 + mydatabase.getTimeVan(2);
                                        mydatabase.setTimeVan(2, time2);
                                    }
                                    break;
                                case 3:
                                    if (mydatabase.getStateVal(3) == "on")
                                    {
                                        int time3 = DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second - timeval3;
                                        time3 = time3 + mydatabase.getTimeVan(3);
                                        mydatabase.setTimeVan(3, time3);
                                    }
                                    break;
                                case 4:
                                    if (mydatabase.getStateVal(4) == "on")
                                    {
                                        int time4 = DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second - timeval4;
                                        time4 = time4 + mydatabase.getTimeVan(4);
                                        mydatabase.setTimeVan(4, time4);
                                    }
                                    break;
                                case 5:
                                    if (mydatabase.getStateVal(5) == "on")
                                    {
                                        int time5 = DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second - timeval5;
                                        time5 = time5 + mydatabase.getTimeVan(5);
                                        mydatabase.setTimeVan(5, time5);
                                    }
                                    break;
                            }
                            #endif
                            int timenow = DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second;
                            int dapung = 0;
                            try
                            {
                                dapung = timenow - (int)timeDapung[mac_actor];
                                timeDapung.Remove(mac_actor);
                                DisplayData("(" + DateTime.Now + "): Van so " + tt.ToString() + " da tat!\r\n(Dap ung dieu khien bang tay: " + dapung + " giay)\r\n", tb);
                            }
                            catch {
                                DisplayData("(" + DateTime.Now + "): Van so " + tt.ToString() + " da tat\r\n", tb);
                            }
                            mydatabase.setStateVal(tt, "off");
                        }
                        showVanOff(tt, mytext);
                        if (tt == 1)
                        { onofVal1 = false; }
                        if (tt == 2)
                        { onofVal2 = false; }
                        if (tt == 3)
                        { onofVal3 = false; }
                        if (tt == 4)
                        { onofVal4 = false; }
                        if (tt == 5)
                        { onofVal5 = false; }
                        if (tt == 6)
                        { onofVal6 = false; }
                        if (tt == 15)
                        { onofVal1 = false; onofVal2 = false; onofVal3 = false; onofVal4 = false; onofVal5 = false; onofVal6 = false; }
                    }
                    }
            }
            catch
            {}
        }
        public byte[] ConvertTobyte(string com)
        {
                byte[] command = new byte[4];
                string nn1 = com.Substring(0, 2);
                string nn2 = com.Substring(2, 2);
                string ss = com.Substring(4, 2);
                int kytu = Convert.ToInt16(com[7]);
                int byte0 = int.Parse(nn1, System.Globalization.NumberStyles.HexNumber);
                int byte1 = int.Parse(nn2, System.Globalization.NumberStyles.HexNumber);
                int byte3 = int.Parse(ss, System.Globalization.NumberStyles.Integer);
                int kq = 0;
                if (com[6] == '0')
                {
                    kq = byte3;
                }
                if (com[6] == '1')
                {
                    kq = byte3 + 128;
                }
                command[0] = (byte)byte0;
                command[1] = (byte)byte1;
                command[2] = (byte)kq;
                command[3] = (byte)kytu;
                return command;
        }
        public string CreateCommand(bool OnorOff, string mac,string Val)
        {
            try
            {
                Database mydatabase = new Database();
                string command = "";
                command = mydatabase.getNetworkIpActor(mac);
                if (OnorOff == true) //lenh bat cac van
                {
                    switch (Val)
                    {
                        case "V1":
                            command += "011$";
                            break;
                        case "V2":
                            command += "021$";
                            break;
                        case "V3":
                            command += "031$";
                            break;
                        case "V4":
                            command += "041$";
                            break;
                        case "V5":
                            command += "051$";
                            break;
                        case "V6":
                            command += "061$";
                            break;
                    }
                }
                else        //gui lenh tat cac van
                {
                    switch (Val)
                    {
                        case "V1":
                            command += "010$";
                            break;
                        case "V2":
                            command += "020$";
                            break;
                        case "V3":
                            command += "030$";
                            break;
                        case "V4":
                            command += "040$";
                            break;
                        case "V5":
                            command += "050$";
                            break;
                        case "V6":
                            command += "060$";
                            break;
                    }
                }
                return command;
            }
            catch
            {
                return "";
            }
        }

        public void OpenSerialPort2()
        {
            try
            {
                comSMS.PortName = "COM4";
                comSMS.BaudRate = 115200;
                comPort.Parity = System.IO.Ports.Parity.None;
                comSMS.StopBits = StopBits.One;
                comSMS.DataBits = 8;
                comSMS.Handshake = Handshake.RequestToSend;
                comSMS.Open();
            }
            catch { }
        }
        private void AutoSendSMS(string sdt, string MAC)
        {
            try
            {
                if (comSMS.IsOpen)
                {
                    //comSMS.Write("AT+CMGS=" + sdt + (char)13);
                    comSMS.Write("AT+CMGS=" + sdt + "\r\n");
                    //comSMS.Write(">Phat hien chay o sensor co dia chi MAC " + mac + "\x1A");
                    comSMS.Write("Da canh bao chay o sensor co dia chi MAC " + MAC + (char)26 + (char)13);
                    DisplayData("(" + DateTime.Now + "): Gui tin nhan den so " + sdt +"\r\n", tb);
                }
                else
                {
                    OpenSerialPort2();
                    //comSMS.Write("AT+CMGF=1" + (char)13);
                    comSMS.Write("AT+CMGS=" + sdt + "\r\n");
                    //comSMS.Write("AT+CMGS=" + sdt + (char)13);
                    //comSMS.Write(">Phat hien chay o sensor co dia chi MAC " + mac + "\x1A");
                    comSMS.Write("Da canh bao chay o sensor co dia chi MAC " + MAC + (char)26 + (char)13);
                    DisplayData("(" + DateTime.Now + "): Gui tin nhan den so " + sdt + "\r\n", tb);
                }
            }
            catch
            {
                DisplayData("Khong the gui SMS \r\n", tb);
            }
        }
        public void TimerInt()
        {
            timerTuoiCay.Enabled = true;
            timerTuoiCay.Interval = 5000;
            timerTuoiCay.Tick += new System.EventHandler(TuoiCayVal1);
        }
        public void TuoiCayVal1(object sender, EventArgs e)
        {
            #if ACTOR_COOR
            for (int i = 1; i < 13; i++)
            {
                timePing[i]++;
                if (timePing[i] == 120)
                {
                    Database myDatabase = new Database();
                    byte[] bCommand = new byte[4];
                    string sHex = i.ToString("x").ToUpper();
                    sHex = "0" + sHex;
                    if (myDatabase.getStatusSensor(sHex) == "true" || myDatabase.getStatusSensor(sHex) == "True")
                    {
                        string sIp = myDatabase.getNetworkIpSensor(sHex);
                        int byte0 = int.Parse(sIp.Substring(0,2), System.Globalization.NumberStyles.HexNumber);
                        int byte1 = int.Parse(sIp.Substring(2,2), System.Globalization.NumberStyles.HexNumber);
                        bCommand[0] = (byte)byte0;
                        bCommand[1] = (byte)byte1;
                        bCommand[2] = (byte)255;
                        bCommand[3] = (byte)36;
                        DisplayData("Gui lenh ping node: "+ sHex,tb);
                        WriteData(bCommand);
                        checkPing[i] = true;
                    }
                    
                }

                if (checkPing[i] == true)
                {
                    countTime[i]++;
                    if (countTime[i] == 12)
                    {
                        Database myDatabase = new Database();
                        string sHex = i.ToString("x").ToUpper();
                        sHex = "0" + sHex;
                        myDatabase.setActiveSensor(sHex, false);
                        DisplayData("Sensor "+sHex+" khong con trong mang",tb);
                        countTime[i] = 0;
                        checkPing[i] = false;
                        reload(pictureBox);
                    }
                }
            }
            #endif
            count++;
            #if BC_COOR
            count_BC++;
            if (count_BC > Time_alarm * 12)
            {
                AutoSendCanhBao();
                count_BC = 0;
            }
            #endif
            #if ACTOR_COOR
            if (onofVal1 == true)
            {
                try
                {
                    time_count_val1++;
                    if(time_count_val1 > Time_control *12)
                    {
                        //gui lenh tat van 1
                        Database mydatabase = new Database();
                        string command = mydatabase.getNetworkIpActor("00");

                        command += "010$";
                        byte[] com = ConvertTobyte(command);
                        WriteData(com);

                        onofVal1 = false;
                    }
                }
                catch
                {
                    DisplayData("Khong gui duoc lenh tat van so 1 " + DateTime.Now, tb);
                }
            }
            if (onofVal2 == true)
            {
                try
                {
                    time_count_val2++;
                    if(time_count_val2 > Time_control *12)
                    {
                        //gui lenh tat van 2
                        Database mydatabase = new Database();
                        string command = mydatabase.getNetworkIpActor("00");

                        command += "020$";
                        byte[] com = ConvertTobyte(command);
                        WriteData(com);

                        onofVal2 = false;
                    }
                }
                catch
                {
                    DisplayData("Khong gui duoc lenh tat van so 2 " + DateTime.Now, tb);
                }
            }
            if (onofVal3 == true)
            {
                try
                {
                    time_count_val3++;
                    if(time_count_val3 > Time_control * 12)
                    {
                        //gui lenh tat van 3
                        Database mydatabase = new Database();
                        string command = mydatabase.getNetworkIpActor("00");

                        command += "030$";
                        byte[] com = ConvertTobyte(command);
                        WriteData(com);

                        onofVal3 = false;
                    }
                }
                catch
                {
                    DisplayData("Khong gui duoc lenh tat van so 3 " + DateTime.Now, tb);
                }
            }
            if (onofVal4 == true)
            {
                try
                {
                    time_count_val4++;
                    if(time_count_val4 > Time_control * 12)
                    {
                        //gui lenh tat van 4
                        Database mydatabase = new Database();
                        string command = mydatabase.getNetworkIpActor("00");

                        command += "040$";
                        byte[] com = ConvertTobyte(command);
                        WriteData(com);

                        onofVal4 = false;
                    }
                }
                catch
                {
                    DisplayData("Khong gui duoc lenh tat van so 4 " + DateTime.Now, tb);
                }
            }
            if (onofVal5 == true)
            {
                try
                {
                    time_count_val5++;
                    if(time_count_val5 > Time_control *12)
                    {
                        //gui lenh tat van 5
                        Database mydatabase = new Database();
                        string command = mydatabase.getNetworkIpActor("00");

                        command += "050$";
                        byte[] com = ConvertTobyte(command);
                        WriteData(com);

                        onofVal5 = false;
                    }
                }
                catch
                {
                    DisplayData("Khong gui duoc lenh tat van so 5 " + DateTime.Now, tb);
                }
            }
            if (onofVal6 == true)
            {
                try
                {
                    time_count_val6++;
                    if (time_count_val6 > Time_control * 12)
                    {
                        //gui lenh tat van 6
                        Database mydatabase = new Database();
                        string command = mydatabase.getNetworkIpActor("00");

                        command += "060$";
                        byte[] com = ConvertTobyte(command);
                        WriteData(com);

                        onofVal6 = false;
                    }
                }
                catch
                {
                    DisplayData("Khong gui duoc lenh tat van so 6 " + DateTime.Now, tb);
                }
            }
            #endif
        }

        #region Show Data
        public void DisplayData(string msg, TextBox listBox1)
        {
            listBox1.Invoke(new EventHandler(delegate
            {
                listBox1.Font = new Font("Tahoma", 10, FontStyle.Regular);
                if (count > 720)
                {
                    listBox1.Text = string.Empty;
                    count = 0;
                }
                listBox1.Text += msg + "\r\n";
                listBox1.SelectionStart = listBox1.Text.Length;
                listBox1.ScrollToCaret();
            }));
        }
        public void showdata(string mac, string ip, float nhietdo, float doam, float nguon, TextBox text)
        {
            text.Invoke(new EventHandler(delegate
            {
                if (index == 2)
                {
                    text.Text = "Sensor " + ip + "(" + mac + ")\r\nNhiet do : " + nhietdo + "\r\nDo am : " + doam + "\r\nNang luong : " + nguon;
                    mypanel.Show();
                }
            }));
        }
        public void showVanOn(int val, string mac, TextBox text)
        {
            text.Invoke(new EventHandler(delegate
            {
                if (index == 2)
                {
                    if (mac == "00")
                    {
                        if (val == 15)
                        {
                            text.Text = "\r\nTat ca cac van duoc bat thanh cong";
                        }
                        else
                        {
                            text.Text = "\r\nVan so " + val + " da duoc bat thanh cong";
                        }
                    }
                    else
                    {
                        text.Text = "\r\nDa bat canh bao muc " + val;
                    }
                    mypanel.Show();
                }
            }));
        }
        public void showVanOff(int val, TextBox text)
        {
            text.Invoke(new EventHandler(delegate
            {
                if (index == 2)
                {
                    if (val == 15)
                    {
                        text.Text = "\r\nTat ca cac van duoc tat thanh cong";
                    }
                    else
                    {
                        text.Text = "\r\nVan so " + val + " da duoc tat thanh cong";
                    }
                    mypanel.Show();
                }
            }));
        }
        #endregion

        #region Connect Web
        public void SendToWeb(string datasend)
        {
            try
            {
                string[] url1 = connection.Confix();
                string url = url1[2];
                if (datasend[0] == '#')
                    datasend = datasend.Substring(1, datasend.Length - 1);

                url = url + "?data=" + datasend;

                // Create web request

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                // Set value for request headers

                request.Method = "GET";

                request.ProtocolVersion = HttpVersion.Version11;

                request.AllowAutoRedirect = false;

                request.Accept = "*/*";

                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; .NET CLR 2.0.50727)";

                request.Headers.Add("Accept-Language", "en-us");

                request.KeepAlive = true;

                StreamReader responseStream = null;

                HttpWebResponse webResponse = null;

                string webResponseStream = string.Empty;

                // Get response for http web request

                webResponse = (HttpWebResponse)request.GetResponse();

                responseStream = new StreamReader(webResponse.GetResponseStream());


                // Read web response into string

                webResponseStream = responseStream.ReadToEnd();

                //close webresponse
                webResponse.Close();
                responseStream.Close();
                // show data receive
                DisplayData(webResponseStream, tb);
            }
            catch
            {
                //MessageBox.Show(e.ToString());
                //DisplayData(MessageType.Error, "Khong the gui du lieu len WebServer",tb);
            }
        }


        public string getDataServer(string url)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                // Set value for request headers

                request.Method = "GET";

                request.ProtocolVersion = HttpVersion.Version11;

                request.AllowAutoRedirect = false;

                request.Accept = "*/*";

                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; .NET CLR 2.0.50727)";

                request.Headers.Add("Accept-Language", "en-us");

                request.KeepAlive = true;

                StreamReader responseStream = null;

                HttpWebResponse webResponse = null;

                string webResponseStream = string.Empty;

                // Get response for http web request

                webResponse = (HttpWebResponse)request.GetResponse();

                responseStream = new StreamReader(webResponse.GetResponseStream());

                // Read web response into string

                webResponseStream = responseStream.ReadToEnd();

                //close WebResponse
                webResponse.Close();

                return webResponseStream;
            }
            catch
            {
                return "";
            }
        }
        public void ThreadRequest()
        {
            threadRequest = new Thread(new ThreadStart(Request));
            threadRequest.Start();
        }
        public void Request()
        {
            try
            {
                string[] url = connection.Confix();
                string uriCom = url[4];
                while (true)
                {
                    //comReadWeb = getdata_from_server(uriCom);
                    comReadWeb = getDataServer(uriCom);
                    if (comReadWeb != "")
                    {
                        if(comReadWeb[0] == '#')
                        {
                            Database myDatabase = new Database();
                            int van = int.Parse(comReadWeb.Substring(1,1));
                            float temp = float.Parse(comReadWeb.Substring(2,2));
                            float humi = float.Parse(comReadWeb.Substring(4,2));
                            myDatabase.setHumiVan(van,humi);
                            myDatabase.setTempVan(van,temp);
                            DisplayData("(" + DateTime.Now + "): Cai dat nguong tu WEB:", tb);
                            DisplayData("Van so : " + van, tb);
                            DisplayData("Nhiet do : " + temp + "°C", tb);
                            DisplayData("Do am : " + humi+ "%", tb);
                        }
                        else
                        { 
                            DisplayData("(" + DateTime.Now + "): Lenh gui tu WEB:", tb);
                            DisplayData("Ma lenh :" + comReadWeb, tb);
                            byte[] commandWeb = ConvertTobyte(comReadWeb);
                            WriteData(commandWeb);
                        }
                        comReadWeb = "";
                    }
                    Thread.Sleep(1000);
                }
            }
            catch
            {
                //DisplayData(MessageType.Error, "Khong the lay du lieu tu WEB", tb);
            }
        }
        public void Send()
        {
            try
            {
                while (true)
                {
                    if (stringRead.Count > 0)
                    {
                        SendToWeb(stringRead.Dequeue().ToString());
                    }
                    Thread.Sleep(1000);
                }
            }
            catch { }
        }
        public void ThreadSend()
        {
            threadSend = new Thread(new ThreadStart(Send));
            threadSend.Start();
        }
        public void SendRS()
        {
            try
            {
                #if BC_COOR
                    SendToWeb("BC");
                #endif
                #if ACTOR_COOR
                    SendToWeb("RS");
                #endif
            }
            catch { }
        }
        #endregion
        
     }
}
