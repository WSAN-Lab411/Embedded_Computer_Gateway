﻿/********************************************************************************
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

#define BC_COOR
//#define ACTOR_COOR

using System;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Xml;
using System.Threading;
using System.Net;

namespace Emboard
{
    public partial class Emboard : Form
    {
        //**************************************************
        private const int WIDTH = 20;
        private const int HEIGHT = 20;
        //***************************************************
        private int pixel_x;
        private int pixel_y;
        //****************************************************
        private int id;
        private string status;
        Thread send;
        //****************************************************
        private void Emboard_Load(object sender,EventArgs e)
        {
            Database myDatabase = new Database();
            send = new Thread(new ThreadStart(comPort.SendRS));
            send.Start();
            comPort.TimerInt();
            #if ACTOR_COOR
            myDatabase.setAllFalse();
            cbMalenh.Items.Clear();
            cbMalenh.Items.Add("Lay nhiet do, do am");
            cbMalenh.Items.Add("Bat van so 1");
            cbMalenh.Items.Add("Bat van so 2");
            cbMalenh.Items.Add("Bat van so 3");
            cbMalenh.Items.Add("Bat van so 4");
            cbMalenh.Items.Add("Bat van so 5");
            cbMalenh.Items.Add("Bat van so 6");
            cbMalenh.Items.Add("Bat tat ca cac van");
            cbMalenh.Items.Add("Tat van so 1");
            cbMalenh.Items.Add("Tat van so 2");
            cbMalenh.Items.Add("Tat van so 3");
            cbMalenh.Items.Add("Tat van so 4");
            cbMalenh.Items.Add("Tat van so 5");
            cbMalenh.Items.Add("Tat van so 6");
            cbMalenh.Items.Add("Tat tat ca cac van");
            #endif
            
            #if BC_COOR
            myDatabase.setFalseActor();
            myDatabase.setNetworkIpActor("B1","0000");
            cbMalenh.Items.Clear();
            cbMalenh.Items.Add("Lay nhiet do, do am");
            cbMalenh.Items.Add("Bat canh bao muc 1");
            cbMalenh.Items.Add("Bat canh bao muc 2");
            cbMalenh.Items.Add("Bat canh bao muc 3");
            cbMalenh.Items.Add("Bat canh bao muc 4");
            cbMalenh.Items.Add("Bat canh bao muc 5");
            #endif
            
            myDatabase.setFalseBC();
            myDatabase.setValOff();
            myDatabase.setTimeVan(1, 0);
            myDatabase.setTimeVan(2, 0);
            myDatabase.setTimeVan(3, 0);
            myDatabase.setTimeVan(4,0);
            myDatabase.setTimeVan(5, 0);
            comPort.Time_alarm = myDatabase.getTimeAlarm();
            comPort.Time_control = myDatabase.getTimeActor();
            comPort.mypanel = pnShow;
            comPort.mytext = txtshow;
            comPort.tb = tbShow;
            txtmac.Hide();
            pnShow.Hide();
            btexit.Enabled = true;
            try
            {
                comPort.pictureBox = pictureBox1;
                comPort.reload(comPort.pictureBox);
            }
            catch
            {
                MessageBox.Show("Khong the load ban do");
            }
          
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                Database myDatabase = new Database();
                bool check = false;
                XmlNodeList node = (myDatabase.xml).GetElementsByTagName("node");
                foreach (XmlNode nodechild in node)
                {
                    pixel_x = Int32.Parse(nodechild.Attributes["pixel_x"].Value);
                    pixel_y = Int32.Parse(nodechild.Attributes["pixel_y"].Value);
                    mac = nodechild.Attributes["mac"].Value;
                    status = nodechild.Attributes["status"].Value;
                    if (e.Button == MouseButtons.Right && e.X > pixel_x && e.X < pixel_x + WIDTH && e.Y > pixel_y && e.Y < pixel_y + HEIGHT)
                    {
                        if (mac == "00")
                        {
                            if (status == "true" || status == "True")
                            {
                                battatca.Enabled = true;
                                MNstatusA.Enabled = true;
                                tattatca.Enabled = true;
                            }
                            else
                            {
                                battatca.Enabled = false;
                                MNstatusA.Enabled = false;
                                tattatca.Enabled = false;
                            }
                            MNstatusA.Text = "Trang thai : " + nodechild.Attributes["status"].Value;
                            ctMenuActor.Show(pictureBox1, new Point(e.X, e.Y));
                        }
                        else if (mac[0] == 'B')
                        {
                            if (status == "true" || status == "True")
                            {
                                mnReset.Enabled = true;
                            }
                            else
                            {
                                mnReset.Enabled = false;
                            }
                            ctxMenuReset.Show(pictureBox1, new Point(e.X, e.Y));
                        }
                        else
                        {
                            network_ip = nodechild.Attributes["network_ip"].Value;
                            MNstatusS.Text = "Trang thai: " + nodechild.Attributes["status"].Value;
                            if (status == "true" || status == "True")
                            {
                                laydulieu.Enabled = true;
                                MNstatusS.Enabled = true;
                                menuTemp.Enabled = true;
                                menuHumi.Enabled = true;
                                menuTemp.Text = "Nhiet do: " + nodechild.Attributes["temperature"].Value + "°C";
                                menuHumi.Text = "Do am: " + nodechild.Attributes["humidity"].Value + "%";
                            }
                            else
                            {
                                menuTemp.Text = "Nhiet do:  0°C";
                                menuHumi.Text = "Do am: 0%";
                                laydulieu.Enabled = false;
                                MNstatusS.Enabled = false;
                                menuTemp.Enabled = false;
                                menuHumi.Enabled = false;
                            }
                            ctMenuSensor.Show(pictureBox1, new Point(e.X, e.Y));
                        }
                        check = true;
                        break;
                    }
                }
                if (!check)
                {
                    XmlNodeList val = (myDatabase.xml).GetElementsByTagName("val");
                    status = myDatabase.getStatusActor("00");
                    foreach (XmlNode valchild in val)
                    {
                        pixel_x = Int32.Parse(valchild.Attributes["pixel_x"].Value);
                        pixel_y = Int32.Parse(valchild.Attributes["pixel_y"].Value);
                        if (e.Button == MouseButtons.Right && e.X > pixel_x && e.X < pixel_x + WIDTH && e.Y > pixel_y && e.Y < pixel_y + HEIGHT)
                        {
                            if (status == "true" || status == "True")
                            {
                                batvan.Enabled = true;
                                tatvan.Enabled = true;
                                MNstatusV.Enabled = true;
                            }
                            else
                            {
                                batvan.Enabled = false;
                                tatvan.Enabled = false;
                                MNstatusV.Enabled = false;
                            }
                            id = Int32.Parse(valchild.Attributes["id"].Value);
                            MNstatusV.Text = "Trang thai : " + valchild.Attributes["state"].Value;
                            ctMenuVan.Show(pictureBox1, new Point(e.X, e.Y));
                            break;
                        }
                    }
                }
            }
            catch { }
        }
        private void laydulieu_Click(object sender, System.EventArgs e)
        {
            try
            {
                command = network_ip + "000$";
                commandbyte = comPort.ConvertTobyte(command);
                comPort.WriteData(commandbyte);
                comPort.DisplayData("(" + DateTime.Now + "): Gui lenh lay du lieu sensor:\r\nMa lenh: " + command, tbShow);
            }
            catch
            {
            }
        }
        private void batvan_Click(object sender, EventArgs e)
        {
            try
            {
                Database myDatabase = new Database();
                network_ip = myDatabase.getNetworkIpActor("00");
                switch (id)
                {
                    case 1:
                        command = network_ip + "011$";
                        break;
                    case 2:
                        command = network_ip + "021$";
                        break;
                    case 3:
                        command = network_ip + "031$";
                        break;
                    case 4:
                        command = network_ip + "041$";
                        break;
                    case 5:
                        command = network_ip + "051$";
                        break;
                    case 6:
                        command = network_ip + "061$";
                        break;
                   default:
                        command = network_ip + "011$";
                        break;
                }
                comPort.DisplayData("(" + DateTime.Now + "): Gui lenh dieu khien actor\r\nMa lenh: " + command, tbShow);
                commandbyte = comPort.ConvertTobyte(command);
                comPort.WriteData(commandbyte);
            }
            catch
            {
            }
        }
        private void tatvan_Click(object sender, System.EventArgs e)
        {
            try
            {
                Database myDatabase = new Database();
                network_ip = myDatabase.getNetworkIpActor("00");
                switch (id)
                {
                    case 1:
                        command = network_ip + "010$";
                        break;
                    case 2:
                        command = network_ip + "020$";
                        break;
                    case 3:
                        command = network_ip + "030$";
                        break;
                    case 4:
                        command = network_ip + "040$";
                        break;
                    case 5:
                        command = network_ip + "050$";
                        break;
                    case 6:
                        command = network_ip + "060$";
                        break;
                     default:
                        command = network_ip + "010$";
                        break;
                }
                comPort.DisplayData("(" + DateTime.Now + "): Gui lenh dieu khien actor:\r\nMa lenh: " + command, tbShow);
                commandbyte = comPort.ConvertTobyte(command);
                comPort.WriteData(commandbyte);
            }
            catch
            {
            }
        }
        private void battatca_Click(object sender, System.EventArgs e)
        {
            try
            {
                Database myDatabase = new Database();
                network_ip = myDatabase.getNetworkIpActor("00");
                command = network_ip + "151$";
                comPort.DisplayData("(" + DateTime.Now + "): Gui lenh dieu khien actor:\r\nMa lenh: " + command, tbShow);
                commandbyte = comPort.ConvertTobyte(command);
                comPort.WriteData(commandbyte);
            }
            catch
            {
            }
        }
        private void tattatca_Click(object sender, System.EventArgs e)
        {
            try
            {
                Database myDatabase = new Database();
                network_ip = myDatabase.getNetworkIpActor("00");
                command = network_ip + "150$";
                //DisplayData(MessageType.Incoming, "Command " + command + " sent at: " + DateTime.Now + "\r\n", tbShow);
                comPort.DisplayData("(" + DateTime.Now + "): Gui lenh dieu khien actor:\r\nMa lenh: " + command, tbShow);
                commandbyte = comPort.ConvertTobyte(command);
                comPort.WriteData(commandbyte);
            }
            catch
            {
            }
        }
        private void mnReset_Click(object sender, System.EventArgs e)
        {
            try
            {
                Database mydatabase = new Database();
                network_ip = mydatabase.getNetworkIpActor("B1");
                command = network_ip + "031$";
                commandbyte = comPort.ConvertTobyte(command);
                comPort.WriteData(commandbyte);
            }
            catch
            {
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comPort.Index = tabControl1.SelectedIndex;
            if(tabControl1.SelectedIndex==1)
            {
                pnNode.Show();
                pnGeneral.Hide();
                pnthreshold.Hide();
                pnsetup.Hide();
                pnNode.Location=new Point(0,0);
                lbGeneral.Enabled = true;
                lbNode.Enabled = false;
                linkthreshold.Enabled = true;
                linksetup.Enabled = true;
                rbCreateNode.Checked = true;
                cbmac.Hide();
                txtmac.Show();
                btCreate.Text = "Create Node";

                cbSelectNode.SelectedIndex = -1;
                tbLatitude.Text = string.Empty;
                tbLongitude.Text = string.Empty;
                tbActor.Text = string.Empty;
                tbLatitude.ReadOnly = false;
                tbLongitude.ReadOnly = false;
                tbActor.ReadOnly = false;
                tbActor.Enabled = true;
                lbActor.Enabled = true;
                txtmac.Text = "";
            }
        }

        private void btshow_Click(object sender, System.EventArgs e)
        {
            comPort.mypanel.Hide();
        }
        /*
        #region DisplayData
        private void DisplayData(string msg, TextBox listBox1)
        {
            listBox1.Invoke(new EventHandler(delegate
            {
                listBox1.Font = new Font("Tahoma", 10, FontStyle.Regular);
                listBox1.Text += msg + "\r\n";
                listBox1.SelectionStart = listBox1.Text.Length;
                listBox1.ScrollToCaret();
            }));
        }
        #endregion
         * */
    }

}