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

//su dung cho lay du lieu sensor bao chay bang tay
#define USE_DATABC

using System;
using System.Windows.Forms;
using System.Xml;
using System.Threading;

namespace Emboard
{
    public partial class Emboard : Form
    {
        #region object
        libCOM comPort = new libCOM();
        #endregion
        #region Variable Command
        private string command;
        private string network_ip;
        private string mac;
        private byte[] commandbyte;
        static private string mac_actor = "";
        #endregion

        private void btConnect_Click(object sender, EventArgs e)
        {
            try
            {
                Database myDatabase = new Database();
                int checkCom = comPort.OpenPort("COM2",19200,"None","One",8);
                comPort.OpenSerialPort2();
                if (checkCom == -1)
                {
                    comPort.DisplayData("Connecting......", tbShow);
                }
                else
                {
                    btConnect.Enabled = false;
                    btSend.Enabled = true;
                    cbnode.Enabled = true;
                    cbMalenh.Enabled = true;
                    btDisconnect.Enabled = true;
                    comPort.DisplayData("(" + DateTime.Now + "): Da mo cong COM", tbShow);
                }
                comPort.ThreadRequest();
                comPort.ThreadSend();
            }
            catch
            {
            }
        }

        private void btSend_Click(object sender, EventArgs e)
        {
            try
            {
                Database myDatabase = new Database();
                int timenow = DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second;
                if (cbMalenh.SelectedIndex == 0)
                {
                    mac = cbnode.Text.Substring(7, 2);
                    comPort.timeDapung.Remove(mac);
                    comPort.timeDapung.Add(mac, timenow);
                    if (mac[0] == '0')
                    {
                        network_ip = myDatabase.getNetworkIpSensor(mac);
                    }
                    #if USE_DATABC
                    else
                    {
                        network_ip = myDatabase.getNetworkIpSensorBC(mac);
                    }
                    #endif
                    command = network_ip + "000$";
                    comPort.DisplayData("(" + DateTime.Now + "): Gui lenh lay du lieu sensor (" + mac+ "):\r\n Ma lenh : " + command, tbShow);
                }
                else
                {
                    //mac = cbnode.Text.Substring(6, 2);
                    network_ip = myDatabase.getNetworkIpActor(mac_actor);
                    comPort.timeDapung.Remove(mac_actor);
                    comPort.timeDapung.Add(mac_actor, timenow);
                    switch (cbMalenh.SelectedIndex)
                    {
                        case 1:
                            command = network_ip + "011$";
                            if (mac_actor == "00")
                            {
                                comPort.DisplayData("(" + DateTime.Now + "):Gui lenh bat van so 1\r\nMa lenh:" + command, tbShow);
                            }
                            else
                            {
                                comPort.DisplayData("(" + DateTime.Now + "): Gui lenh bat canh bao muc 1\r\nMa lenh:" + command, tbShow);
                            }
                            break;
                        case 2:
                            command = network_ip + "021$";
                            if (mac_actor == "00")
                            {
                                comPort.DisplayData("(" + DateTime.Now + "):Gui lenh bat van so 2\r\nMa lenh:" + command, tbShow);
                            }
                            else
                            {
                                comPort.DisplayData("(" + DateTime.Now + "): Gui lenh bat canh bao muc 2\r\nMa lenh:" + command, tbShow);
                            }
                            break;
                        case 3:
                            command = network_ip + "031$";
                            if (mac_actor == "00")
                            {
                                comPort.DisplayData("(" + DateTime.Now + "):Gui lenh bat van so 3\r\nMa lenh:" + command, tbShow);
                            }
                            else
                            {
                                comPort.DisplayData("(" + DateTime.Now + "): Gui lenh bat canh bao muc 3\r\nMa lenh:" + command, tbShow);
                            }
                            break;
                        case 4:
                            command = network_ip + "041$";
                            if (mac_actor == "00")
                            {
                                comPort.DisplayData("(" + DateTime.Now + "):Gui lenh bat van so 4\r\nMa lenh:" + command, tbShow);
                            }
                            else
                            {
                                comPort.DisplayData("(" + DateTime.Now + "): Gui lenh bat canh bao muc 4\r\nMa lenh:" + command, tbShow);
                            }
                            break;
                        case 5:
                            command = network_ip + "051$";
                            if (mac_actor == "00")
                            {
                                comPort.DisplayData("(" + DateTime.Now + "):Gui lenh bat van so 5\r\nMa lenh:" + command, tbShow);
                            }
                            else
                            {
                                comPort.DisplayData("(" + DateTime.Now + "): Gui lenh bat canh bao muc 5\r\nMa lenh:" + command, tbShow);
                            }
                            break;
                        case 6:
                            command = network_ip + "061$";
                            comPort.DisplayData("(" + DateTime.Now + "):Gui lenh bat van so 6\r\nMa lenh:" + command, tbShow);
                            
                            break;
                        case 7:
                            command = network_ip + "151$";
                            comPort.DisplayData("(" + DateTime.Now + "):Bat tat ca cac van\r\nMa lenh:" + command, tbShow);
                            break;
                        case 8:
                            command = network_ip + "010$";
                            comPort.DisplayData("(" + DateTime.Now + "):Gui lenh tat van so 1\r\nMa lenh:" + command, tbShow);
                            break;
                        case 9:
                            command = network_ip + "020$";
                            comPort.DisplayData("(" + DateTime.Now + "):Gui lenh tat van so 2\r\nMa lenh:" + command, tbShow);
                            break;
                        case 10:
                            command = network_ip + "030$";
                            comPort.DisplayData("(" + DateTime.Now + "):Gui lenh tat van so 3\r\nMa lenh:" + command, tbShow);
                            break;
                        case 11:
                            command = network_ip + "040$";
                            comPort.DisplayData("(" + DateTime.Now + "):Gui lenh tat van so 4\r\nMa lenh:" + command, tbShow);
                            break;
                        case 12:
                            command = network_ip + "050$";
                            comPort.DisplayData("(" + DateTime.Now + "):Gui lenh tat van so 5\r\nMa lenh:" + command, tbShow);
                            break;
                        case 13:
                            command = network_ip + "060$";
                            comPort.DisplayData("(" + DateTime.Now + "):Gui lenh tat van so 6\r\nMa lenh:" + command, tbShow);
                            
                            break;
                        case 14:
                            command = network_ip + "150$";
                            comPort.DisplayData("(" + DateTime.Now + "):Gui lenh tat tat ca cac van\r\nMa lenh:" + command, tbShow);
                            
                            break;
                    }
                    
                }
                
               
                cbMalenh.SelectedIndex = -1;
                cbnode.Items.Clear();
                cbnode.Text = "";
                if (command.Length == 8)
                {
                    commandbyte = comPort.ConvertTobyte(command);
                    comPort.WriteData(commandbyte);
                }
            }
            catch 
            {
                MessageBox.Show("Ban chua chon du thong tin o Commnad hoac Node", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            }
        }

        private void btDisconnect_Click(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    comPort.comPort.DiscardInBuffer();
                    comPort.comPort.DiscardNull = true;
                }
                catch { }
                comPort.COMClose();
                comPort.comSMS.Close();
                btConnect.Enabled = true;
                btDisconnect.Enabled = false;
                cbMalenh.Enabled = false;
                cbnode.Enabled = false;
                btSend.Enabled = false;
                comPort.threadRequest.Abort();
                comPort.threadSend.Abort();
                send.Abort();
            }
            catch { }
        }


        private void cbMalenh_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                int index_malenh = cbMalenh.SelectedIndex;
                if (index_malenh == 0)
                {
                    cbnode.Items.Clear();
                    Database my_Database = new Database();
                    //Hien thi danh sach sensor khu vuon lan
                    XmlNodeList nodeSensor = ((XmlElement)my_Database.sensor).GetElementsByTagName("node");
                    foreach (XmlNode node in nodeSensor)
                    {
                        if (node.Attributes["status"].Value == "true" || node.Attributes["status"].Value == "True")
                        {
                            string str = "Sensor " + node.Attributes["mac"].Value;
                            cbnode.Items.Add(str);
                        }
                    }
                    //Hien thi sensor khu bao chay
                    #if USE_DATABC
                    XmlNodeList nodeSensor_BC = ((XmlElement)my_Database.sensor_bc).GetElementsByTagName("node");
                    foreach (XmlNode node_BC in nodeSensor_BC)
                    {
                        if (node_BC.Attributes["status"].Value == "true" || node_BC.Attributes["status"].Value == "True")
                        {
                            string str = "Sensor " + node_BC.Attributes["mac"].Value;
                            cbnode.Items.Add(str);
                        }
                    }
                    #endif
                }
                else
                {
                    cbnode.Items.Clear();
                    Database my_Database = new Database();
                    XmlNodeList nodeActor = ((XmlElement)my_Database.actor).GetElementsByTagName("node");
                    foreach (XmlNode node in nodeActor)
                    {
                        if (node.Attributes["status"].Value == "true" || node.Attributes["status"].Value == "True")
                        {
                            if (cbMalenh.SelectedIndex < 6)
                            {
                                string str = "";
                                if (node.Attributes["mac"].Value == "B1")
                                {
                                    str = "Actor bao chay";
                                    mac_actor = "B1";
                                }
                                else
                                {
                                    str = "Actor bom tuoi";
                                    mac_actor = "00"; 
                                }
                                cbnode.Items.Add(str);
                            }
                            else 
                            {
                                string str = "Actor bom tuoi";
                                cbnode.Items.Add(str);
                                mac_actor = "00";
                                break;
                            }
                        }
                    }
                }
            }
            catch { }
        }
        private void btexit_Click(object sender, EventArgs e)
        {
                try
                {
                    comPort.threadRequest.Abort();
                    comPort.threadSend.Abort();
                    comPort.comPort.DiscardInBuffer();
                    comPort.comPort.DiscardNull = true;
                }
                catch { }
                finally
                {
                    this.Close();
                }
        }
        private void Emboard_Closed(object sender, System.EventArgs e)
        {
            try
            {
                try
                {
                    comPort.comPort.DiscardInBuffer();
                    comPort.comPort.DiscardNull = true;
                }
                catch { }
                comPort.threadRequest.Abort();
                comPort.threadSend.Abort();
                send.Abort();
            }
            catch { }
        }
    }
}
