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

using System;
using System.Windows.Forms;
using System.Xml;

namespace Emboard
{
    public class Database
    {
        string[] file = connection.Confix();
        public XmlDocument xml = new XmlDocument();
        public XmlDocument xml_bc = new XmlDocument();
        public XmlNode sensor;
        public XmlNode actor;
        public XmlNode data_bc;
        public XmlNode sensor_bc;

        //Ham khoi tao
        public Database()
        {
            try
            {
                xml.Load(file[0]);
                sensor = xml.SelectSingleNode("//root/sensor");
                actor = xml.SelectSingleNode("//root/actor");

                xml_bc.Load(file[1]);
                sensor_bc = xml_bc.SelectSingleNode("//root/sensor");
                data_bc = xml_bc.SelectSingleNode("//root/data");
            }
            catch
            {
                //MessageBox.Show("Khong the ket noi co so du lieu");
            }
        }

        /******************************************************************************
         * Sensor khu cham soc  lan
         * ****************************************************************************/
        // Tao node sensor
        public void setNodeSensor(string mac_sensor, string network_ip_sensor, bool status_sensor)
        {
            try
            {
                int total_sensor = Convert.ToInt32(sensor.Attributes[0].Value);

                XmlNode node_sensor = xml.CreateElement("node");

                XmlAttribute mac = xml.CreateAttribute("mac");
                mac.Value = mac_sensor.ToString();
                node_sensor.Attributes.Append(mac);

                XmlAttribute network_ip = xml.CreateAttribute("network_ip");
                network_ip.Value = network_ip_sensor.ToString();
                node_sensor.Attributes.Append(network_ip); 

                XmlAttribute status = xml.CreateAttribute("status");
                status.Value = status_sensor.ToString();
                node_sensor.Attributes.Append(status);

                XmlAttribute pixel_x = xml.CreateAttribute("pixel_x");
                pixel_x.Value = "0";
                node_sensor.Attributes.Append(pixel_x);

                XmlAttribute pixel_y = xml.CreateAttribute("pixel_y");
                pixel_y.Value = "0";
                node_sensor.Attributes.Append(pixel_y);

                XmlAttribute tempreture = xml.CreateAttribute("temperature");
                tempreture.Value = "0";
                node_sensor.Attributes.Append(tempreture);

                XmlAttribute humidity = xml.CreateAttribute("humidity");
                humidity.Value = "0";
                node_sensor.Attributes.Append(humidity);

                XmlAttribute energy = xml.CreateAttribute("energy");
                energy.Value = "0";
                node_sensor.Attributes.Append(energy);

                XmlAttribute actor = xml.CreateAttribute("Val");
                if (mac_sensor.Length > 0)
                {
                    if (mac_sensor == "01" | mac_sensor == "02")
                    {
                        actor.Value = "V1";
                        node_sensor.Attributes.Append(actor);
                    }
                    else if (mac_sensor == "03" | mac_sensor == "04")
                    {
                        actor.Value = "V2";
                        node_sensor.Attributes.Append(actor);
                    }
                    else if (mac_sensor == "05" | mac_sensor == "06")
                    {
                        actor.Value = "V3";
                        node_sensor.Attributes.Append(actor);
                    }
                    if (mac_sensor == "08" | mac_sensor == "07")
                    {
                        actor.Value = "V4";
                        node_sensor.Attributes.Append(actor);
                    }
                    else if (mac_sensor == "09" | mac_sensor == "0A")
                    {
                        actor.Value = "V5";
                        node_sensor.Attributes.Append(actor);
                    }
                    else if (mac_sensor == "0B" | mac_sensor == "0C")
                    {
                        actor.Value = "V6";
                        node_sensor.Attributes.Append(actor);
                    }
                    else
                    {
                        actor.Value = "V1";
                        node_sensor.Attributes.Append(actor);
                    }
                }


                XmlAttribute time = xml.CreateAttribute("time");
                time.Value = "0";
                node_sensor.Attributes.Append(time);

                sensor.AppendChild(node_sensor);

                total_sensor++;
                sensor.Attributes[0].Value = total_sensor.ToString();

                xml.Save(file[0]);
            }
            catch { }   
        }

        //tao node sensor bang tay
        public int setSensor(string mac, int _pixel_x, int _pixel_y, string Van)
        {
            try
            {
                int total_sensor = Convert.ToInt32(sensor.Attributes[0].Value);
                XmlNode node_sensor = xml.CreateElement("node");

                XmlAttribute mac1 = xml.CreateAttribute("mac");
                mac1.Value = mac.ToString();
                node_sensor.Attributes.Append(mac1);

                XmlAttribute network_ip = xml.CreateAttribute("network_ip");
                network_ip.Value = "";
                node_sensor.Attributes.Append(network_ip);

                XmlAttribute status1 = xml.CreateAttribute("status");
                status1.Value = "false";
                node_sensor.Attributes.Append(status1);

                XmlAttribute pixel_x = xml.CreateAttribute("pixel_x");
                pixel_x.Value = _pixel_x.ToString();
                node_sensor.Attributes.Append(pixel_x);

                XmlAttribute pixel_y = xml.CreateAttribute("pixel_y");
                pixel_y.Value = _pixel_y.ToString();
                node_sensor.Attributes.Append(pixel_y);

                XmlAttribute tempreture = xml.CreateAttribute("temperature");
                tempreture.Value = "0";
                node_sensor.Attributes.Append(tempreture);

                XmlAttribute humidity = xml.CreateAttribute("humidity");
                humidity.Value = "0";
                node_sensor.Attributes.Append(humidity);

                XmlAttribute energy1 = xml.CreateAttribute("energy");
                energy1.Value = "0";
                node_sensor.Attributes.Append(energy1);

                XmlAttribute actor = xml.CreateAttribute("Val");
                actor.Value = Van.ToString();
                node_sensor.Attributes.Append(actor);

                XmlAttribute time1 = xml.CreateAttribute("time");
                time1.Value = "0";
                node_sensor.Attributes.Append(time1);

                sensor.AppendChild(node_sensor);

                total_sensor++;
                sensor.Attributes[0].Value = total_sensor.ToString();

                xml.Save(file[0]);
                return 1;
            }
            catch
            {
                return -1;
            }
        }

        // Lay dia chi mang node sensor
        public string getNetworkIpSensor(string mac)
        {
            try
            {
                XmlElement element = xml.DocumentElement;
                XmlNode myxmlnode = element.SelectSingleNode("//root/sensor/node[@mac='" + mac.ToString() + "']");
                return myxmlnode.Attributes[1].Value;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        // Lay trang thai 1 node sensor
        public string getStatusSensor(string mac)
        {
            try
            {
                XmlElement element = xml.DocumentElement;
                XmlNode myxmlnode = element.SelectSingleNode("//root/sensor/node[@mac='" + mac.ToString() + "']");
                return myxmlnode.Attributes[2].Value;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        // lay doa do pixel X node sensor
        public float getSensorPixel_x(string mac)
        {
            try
            {
                XmlElement element = xml.DocumentElement;
                XmlNode myxmlnode = element.SelectSingleNode("//root/sensor/node[@mac='" + mac.ToString() + "']");
                return float.Parse(myxmlnode.Attributes[3].Value);
            }
            catch
            {
                return (-1);
            }
        }

        // lay toa do pixel Y node sensor
        public float getSensorPixel_y(string mac)
        {
            try
            {
                XmlElement element = xml.DocumentElement;
                XmlNode myxmlnode = element.SelectSingleNode("//root/sensor/node[@mac='" + mac.ToString() + "']");
                return float.Parse(myxmlnode.Attributes[4].Value);
            }
            catch
            {
                return (-1);
            }
        }

        // Lay nhiet do hien tai node sensor
        public double getTemp(string mac)
        {
            try
            {
                XmlElement element = xml.DocumentElement;
                XmlNode myxmlnode = element.SelectSingleNode("//root/sensor/node[@mac ='" + mac.ToString() + "']");
                return Convert.ToDouble(myxmlnode.Attributes[5].Value);
            }
            catch
            {
                return (-1);
            }
        }

        // Lay do am hien tai
        public double getHumi(string mac)
        {
            try
            {
                XmlElement element = xml.DocumentElement;
                XmlNode myxmlnode = element.SelectSingleNode("//root/sensor/node[@mac ='" + mac.ToString() + "']");
                return Convert.ToDouble(myxmlnode.Attributes[6].Value);
            }
            catch
            {
                return (-1);
            }
        }

        //Lay nang luong sensor
        public double getEnergySensor(string mac)
        {
            try
            {
                XmlElement element = xml.DocumentElement;
                XmlNode myxmlnode = element.SelectSingleNode("//root/sensor/node[@mac ='" + mac.ToString() + "']");
                return Convert.ToDouble(myxmlnode.Attributes[7].Value);
            }
            catch
            {
                return (-1);
            }
        }

        //Lay van node sensor
        public string getVanSensor(string mac)
        {
            try
            {
                XmlElement element = xml.DocumentElement;
                XmlNode myxmlnode = element.SelectSingleNode("//root/sensor/node[@mac='" + mac.ToString() + "']");
                return myxmlnode.Attributes["Val"].Value;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        //set dia chi mang node sensor
        public int setNetworkIpSensor(string mac, string ip)
        {
            try
            {
                XmlElement element = xml.DocumentElement;
                XmlNode myxmlnode = element.SelectSingleNode("//root/sensor/node[@mac = '" + mac.ToString() + "']");
                myxmlnode.Attributes["network_ip"].Value = ip.ToString();
                xml.Save(file[0]);
                return 1;
            }
            catch
            {
                return -1;
            }
        }

        // Set trang thai active sensor
        public int setActiveSensor(string mac, bool status)
        {
            try
            {
                XmlElement element = xml.DocumentElement;
                XmlNode myxmlnode = element.SelectSingleNode("//root/sensor/node[@mac = '" + mac.ToString() + "']");
                myxmlnode.Attributes[2].Value = status.ToString();
                xml.Save(file[0]);
                return 1;
            }
            catch
            {
                return -1;
            }
        }

        // Set toa do X node sensor
        public int setSensorPixel_x(string mac, int x)
        {
            try
            {
                XmlElement element = xml.DocumentElement;
                XmlNode myxmlnode = element.SelectSingleNode("//root/sensor/node[@mac = '" + mac.ToString() + "']");
                myxmlnode.Attributes[3].Value = x.ToString();
                xml.Save(file[0]);
                return 1;
            }
            catch
            {
                return -1;
            }
        }

        // Set toa do Y node sensor
        public int setSensorPixel_y(string mac, int y)
        {
            try
            {
                XmlElement element = xml.DocumentElement;
                XmlNode myxmlnode = element.SelectSingleNode("//root/sensor/node[@mac = '" + mac.ToString() + "']");
                myxmlnode.Attributes[4].Value = y.ToString();
                xml.Save(file[0]);
                return 1;
            }
            catch
            {
                return -1;
            }
        }

        //Set Van cho node sensor
        public int setVanSensor(string mac, string van)
        {
            try
            {
                XmlElement element = xml.DocumentElement;
                XmlNode myxmlnode = element.SelectSingleNode("//root/sensor/node[@mac = '" + mac.ToString() + "']");
                myxmlnode.Attributes["Val"].Value = van.ToString();
                xml.Save(file[0]);
                return 1;
            }
            catch
            {
                return -1;
            }
        }

        //Lay tong so node sensor
        public int getTotalSensor()
        {
            try
            {
                return Convert.ToInt32(sensor.Attributes[0].Value);
            }
            catch
            {
                return -1;
            }
        }

        //set tong so node sensor
        public int setTotalSensor(int totalSensor)
        {
            try
            {
                sensor.Attributes[0].Value = totalSensor.ToString();
                xml.Save(file[0]);
                return 1;
            }
            catch
            {
                return -1;
            }
        }

        //Kiem tra sensor vuon lan xem co ton tai khong
        public string CheckSensor(string mac)
        {
            try
            {
                string checksensor = "false";
                XmlNodeList nodeSensor = ((XmlElement)sensor).GetElementsByTagName("node");
                foreach (XmlNode node in nodeSensor)
                {
                    if (node.Attributes[0].Value == mac.ToString())
                    {
                        checksensor = "true";
                    }
                }
                return checksensor;
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }

        /******************************************************************************
         * actor khu cham soc  lan
         * ****************************************************************************/

        //tao node actor
        public void setNodeActor(string mac_actor, string network_ip_actor, bool status_actor)
        {
            try
            {
                int total_actor = Convert.ToInt32(actor.Attributes[0].Value);
                XmlNode node_actor = xml.CreateElement("node");

                XmlAttribute mac = xml.CreateAttribute("mac");
                mac.Value = mac_actor.ToString();
                node_actor.Attributes.Append(mac);

                XmlAttribute network_ip = xml.CreateAttribute("network_ip");
                network_ip.Value = network_ip_actor.ToString();
                node_actor.Attributes.Append(network_ip);

                XmlAttribute status = xml.CreateAttribute("status");
                status.Value = status_actor.ToString();
                node_actor.Attributes.Append(status);

                XmlAttribute pixel_x = xml.CreateAttribute("pixel_x");
                XmlAttribute pixel_y = xml.CreateAttribute("pixel_y");
                switch (mac_actor)
                {
                    case "00":
                        pixel_x.Value = "343";
                        pixel_y.Value = "122";
                        break;
                    case "B1":
                        pixel_x.Value = "333";
                        pixel_y.Value = "11";
                        break;
                    default:
                        pixel_x.Value = "0";
                        pixel_y.Value = "0";
                        break;
                }
                node_actor.Attributes.Append(pixel_x);
                node_actor.Attributes.Append(pixel_y);

                XmlAttribute energy = xml.CreateAttribute("energy");
                energy.Value = "0";
                node_actor.Attributes.Append(energy);

                actor.AppendChild(node_actor);
                total_actor++;
                actor.Attributes[0].Value = total_actor.ToString();
                xml.Save(file[0]);
            }
            catch { }
        }

        //tao actor bang tay
        public int setActor(string mac, int _pixel_x, int _pixel_y)
        {
            try
            {
                int total_actor = Convert.ToInt32(actor.Attributes[0].Value);
                XmlNode node_actor = xml.CreateElement("node");

                XmlAttribute mac1 = xml.CreateAttribute("mac");
                mac1.Value = mac.ToString();
                node_actor.Attributes.Append(mac1);

                XmlAttribute network_ip = xml.CreateAttribute("network_ip");
                network_ip.Value = "";
                node_actor.Attributes.Append(network_ip);

                XmlAttribute status1 = xml.CreateAttribute("status");
                status1.Value = "false";
                node_actor.Attributes.Append(status1);

                XmlAttribute pixel_x = xml.CreateAttribute("pixel_x");
                pixel_x.Value = _pixel_x.ToString();
                node_actor.Attributes.Append(pixel_x);

                XmlAttribute pixel_y = xml.CreateAttribute("pixel_y");
                pixel_y.Value = _pixel_y.ToString();
                node_actor.Attributes.Append(pixel_y);

                XmlAttribute energy1 = xml.CreateAttribute("energy");
                energy1.Value = "0";
                node_actor.Attributes.Append(energy1); ;

                actor.AppendChild(node_actor);

                total_actor++;
                actor.Attributes[0].Value = total_actor.ToString();

                xml.Save(file[0]);
                return 1;
            }
            catch
            {
                return -1;
            }
        }

        // Lay Dia chi mang node actor
        public string getNetworkIpActor(string mac)
        {
            try
            {
                XmlElement element = xml.DocumentElement;
                XmlNode myxmlnode = element.SelectSingleNode("//root/actor/node[@mac ='" + mac.ToString() + "']");
                return myxmlnode.Attributes[1].Value;
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }

        //lay trang thai actor 
        public string getStatusActor(string mac)
        {
            try
            {
                XmlElement element = xml.DocumentElement;
                XmlNode myxmlnode = element.SelectSingleNode("//root/actor/node[@mac='" + mac.ToString() + "']");
                return myxmlnode.Attributes[2].Value;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        //lay pixel x actor
        public double getActorPixel_x(string mac)
        {
            try
            {
                XmlElement element = xml.DocumentElement;
                XmlNode myxmlnode = element.SelectSingleNode("//root/actor/node[@mac='" + mac.ToString() + "']");
                return Convert.ToDouble(myxmlnode.Attributes[3].Value);

            }
            catch
            {
                return (-1);
            }
        }

        // lay toa do pixelY actor
        public double getActorPixel_y(string mac)
        {
            try
            {
                XmlElement element = xml.DocumentElement;
                XmlNode myxmlnode = element.SelectSingleNode("//root/actor/node[@mac ='" + mac.ToString() + "']");
                return Convert.ToDouble(myxmlnode.Attributes[4].Value);
            }
            catch
            {
                return (-1);
            }
        }

        //Lay nang luong actor
        public double getEnergyActor(string mac)
        {
            try
            {
                XmlElement element = xml.DocumentElement;
                XmlNode myxmlnode = element.SelectSingleNode("//root/actor/node[@ID='" + mac.ToString() + "']");
                return Convert.ToDouble(myxmlnode.Attributes[5].Value);
            }
            catch
            {
                return (-1);
            }
        }

        //set dia chi mang node actor
        public int setNetworkIpActor(string mac, string ip)
        {
            try
            {
                XmlElement element = xml.DocumentElement;
                XmlNode myxmlnode = element.SelectSingleNode("//root/actor/node[@mac = '" + mac.ToString() + "']");
                myxmlnode.Attributes["network_ip"].Value = ip.ToString();
                xml.Save(file[0]);
                return 1;
            }
            catch
            {
                return -1;
            }
        }

        // Set trang thai status actor
        public int setStatusActor(string mac, bool status)
        {
            try
            {
                XmlElement element = xml.DocumentElement;
                XmlNode myxmlnode = element.SelectSingleNode("//root/actor/node[@mac = '" + mac.ToString() + "']");
                myxmlnode.Attributes[2].Value = status.ToString();
                xml.Save(file[0]);
                return 1;
            }
            catch
            {
                return -1;
            }
        }

        // Set toa do X node actor
        public int setActorPixel_x(string mac, float x)
        {
            try
            {
                XmlElement element = xml.DocumentElement;
                XmlNode myxmlnode = element.SelectSingleNode("//root/actor/node[@mac = '" + mac.ToString() + "']");
                myxmlnode.Attributes[3].Value = x.ToString();
                xml.Save(file[0]);
                return 1;
            }
            catch
            {
                return -1;
            }
        }

        // Set toa do Y node actor
        public int setActorPixel_y(string mac, float y)
        {
            try
            {
                XmlElement element = xml.DocumentElement;
                XmlNode myxmlnode = element.SelectSingleNode("//root/actor/node[@mac = '" + mac.ToString() + "']");
                myxmlnode.Attributes[4].Value = y.ToString();
                xml.Save(file[0]);
                return 1;
            }
            catch
            {
                return -1;
            }
        }

        //Lay tong so node actor
        public int getTotalActor()
        {
            try
            {
                return Convert.ToInt32(actor.Attributes[0].Value);
            }
            catch
            {
                return -1;
            }
        }

        // set tong so node actor
        public int setTotalActor(int totalActor)
        {
            try
            {
                actor.Attributes[0].Value = totalActor.ToString();
                xml.Save(file[0]);
                return 1;
            }
            catch
            {
                return -1;
            }
        }

        //Ham kiem tra actor xem co ton tai khong
        public string CheckActor(string mac)
        {
            try
            {
                string checkActor = "false";
                XmlNodeList nodeActor = ((XmlElement)actor).GetElementsByTagName("node");
                foreach (XmlNode node in nodeActor)
                {
                    if (node.Attributes[0].Value == mac.ToString())
                    {
                        checkActor = "true";
                    }
                }
                return checkActor;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /******************************************************************************
         * Van tuoi khu cham soc  lan
         * ****************************************************************************/

        //Lay trang thai van tuoi
        public string getStateVal(int id)
        {
            try
            {
                XmlElement element = xml.DocumentElement;
                XmlNode myxmlnode = element.SelectSingleNode("//root/van/val[@id = '" + id.ToString() + "']");
                return myxmlnode.Attributes["state"].Value;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        //Lay toa do pixel_x van tuoi theo id (so hieu van)
        public double getValPixel_x(int id)
        {
            try
            {
                XmlElement element = xml.DocumentElement;
                XmlNode myxmlnode = element.SelectSingleNode("//root/van/val[@id='" + id.ToString() + "']");
                return Convert.ToDouble(myxmlnode.Attributes["pixel_x"].Value);

            }
            catch
            {
                return (-1);
            }
        }

        //Lay toa do pixel_y van tuoi theo id (so hieu van)
        public double getValPixel_y(int id)
        {
            try
            {
                XmlElement element = xml.DocumentElement;
                XmlNode myxmlnode = element.SelectSingleNode("//root/van/val[@id='" + id.ToString() + "']");
                return Convert.ToDouble(myxmlnode.Attributes["pixel_y"].Value);

            }
            catch
            {
                return (-1);
            }
        }

        //set trang thai van tuoi
        public int setStateVal(int id, string state)
        {
            try
            {
                XmlElement element = xml.DocumentElement;
                XmlNode myxmlnode = element.SelectSingleNode("//root/van/val[@id = '" + id.ToString() + "']");
                myxmlnode.Attributes["state"].Value = state.ToString();
                xml.Save(file[0]);
                return 1;
            }
            catch
            {
                return -1;
            }
                                                                                                                                                                                                                                                                    }

        //set tat ca cac van tuoi off
        public int setValOff()
        {
            try
            {
                XmlNodeList val = xml.GetElementsByTagName("val");
                foreach (XmlNode _val in val)
                {
                    _val.Attributes["state"].Value = "off";
                }
                xml.Save(file[0]);
                return 1;
            }
            catch
            {
                return -1;
            }
        }

        //set tat ca cac van tuoi on
        public int setValOn()
        {
            try
            {
                XmlNodeList val = xml.GetElementsByTagName("val");
                foreach (XmlNode _val in val)
                {
                    _val.Attributes["state"].Value = "on";
                }
                xml.Save(file[0]);
                return 1;
            }
            catch
            {
                return -1;
            }
        }

        /******************************************************************************
         * threshold tu dong bom tuoi
         * ****************************************************************************/
        //set nguong nhiet do tung van
        public int setTempVan(int van,float temp)
        {
            try
            {
                XmlElement element = xml.DocumentElement;
                XmlNode myxmlnode = element.SelectSingleNode("//root/van/val[@id="+van+"]");
                myxmlnode.Attributes["temp"].Value = temp.ToString();
                xml.Save(file[0]);
                return 1;
            }
            catch
            {
                return -1;
            }
        }
        //set nguong do am tung van
        public int setHumiVan(int van, float humi)
        {
            try
            {
                XmlElement element = xml.DocumentElement;
                XmlNode myxmlnode = element.SelectSingleNode("//root/van/val[@id=" + van + "]");
                myxmlnode.Attributes["humi"].Value = humi.ToString();
                xml.Save(file[0]);
                return 1;
            }
            catch
            {
                return -1;
            }
        }
        //lay nguong nhiet do tung van
        public float getTempVan(int van)
        {
            try
            {
                XmlElement element = xml.DocumentElement;
                XmlNode myxmlnode = element.SelectSingleNode("//root/van/val[@id="+van+"]");
                return float.Parse(myxmlnode.Attributes["temp"].Value);
            }
            catch
            {
                return -1;
            }
        }
        //lay nguong nhiet do tung van
        public float getHumiVan(int van)
        {
            try
            {
                XmlElement element = xml.DocumentElement;
                XmlNode myxmlnode = element.SelectSingleNode("//root/van/val[@id=" + van + "]");
                return float.Parse(myxmlnode.Attributes["humi"].Value);
            }
            catch
            {
                return -1;
            }
        }
        //set nguong nhiet do
        public int setTemp(float temp)
        {
            try
            {
                XmlElement element = xml.DocumentElement;
                XmlNode myxmlnode = element.SelectSingleNode("//root/general/index");
                myxmlnode.Attributes["temperature"].Value = temp.ToString();
                xml.Save(file[0]);
                return 1;
            }
            catch
            {
                return -1;
            }
        }
        
        //set nguong do am
        public int setHumi(float humi)
        {
            try
            {
                XmlElement element = xml.DocumentElement;
                XmlNode myxmlnode = element.SelectSingleNode("//root/general/index");
                myxmlnode.Attributes["humidity"].Value = humi.ToString();
                xml.Save(file[0]);
                return 1;
            }
            catch
            {
                return -1;
            }
        }
        
        //set so dien thoai
        public int setPhoneNumber(string number)
        {
            try
            {
                XmlElement element = xml.DocumentElement;
                XmlNode myxmlnode = element.SelectSingleNode("//root/general/index");
                myxmlnode.Attributes["phonenumber"].Value = number.ToString();
                xml.Save(file[0]);
                return 1;
            }
            catch
            {
                return -1;
            }
        }

        //lay nguong nhiet do
        public float getTempMax()
        {
            try
            {
                XmlElement element = xml.DocumentElement;
                XmlNode myxmlnode = element.SelectSingleNode("//root/general/index");
                return float.Parse(myxmlnode.Attributes["temperature"].Value);
            }
            catch
            {
                return -1;
            }
        }

        //Lay nguong do am
        public float getHumiMax()
        {
            try
            {
                XmlElement element = xml.DocumentElement;
                XmlNode myxmlnode = element.SelectSingleNode("//root/general/index");
                return float.Parse(myxmlnode.Attributes["humidity"].Value);
            }
            catch
            {
                return -1;
            }
        }

        //Lay so dien thoai
        public string getPhoneNumber()
        {
            try
            {
                XmlElement element = xml.DocumentElement;
                XmlNode myxmlnode = element.SelectSingleNode("//root/general/index");
                return myxmlnode.Attributes["phonenumber"].Value;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /******************************************************************************
        * delete co so du lieu
        * ****************************************************************************/
        //xoa node sensor
        public int deleteSensor(string mac)
        {
            try
            {
                int totalSensor = getTotalSensor();
                XmlElement element = xml.DocumentElement;
                XmlNode myxmlnode = element.SelectSingleNode("//root/sensor/node[@mac = '" + mac.ToString() + "']");
                myxmlnode.ParentNode.RemoveChild(myxmlnode);
                totalSensor--;
                sensor.Attributes[0].Value = totalSensor.ToString();
                xml.Save(file[0]);
                return 1;
            }
            catch
            {
                return -1;
            }
        }

        //xoa node actor
        public int deleteActor(string mac)
        {
            try
            {
                int totalActor = getTotalActor();
                XmlElement element = xml.DocumentElement;
                XmlNode myxmlnode = element.SelectSingleNode("//root/actor/node[@mac = '" + mac.ToString() + "']");
                myxmlnode.ParentNode.RemoveChild(myxmlnode);
                totalActor--;
                actor.Attributes[0].Value = totalActor.ToString();
                xml.Save(file[0]);
                return 1;
            }
            catch
            {
                return -1;
            }
        }

        //xoa CSDL
        public void DeleteData()
        {
            data_bc.RemoveAll();
            XmlAttribute total1 = xml_bc.CreateAttribute("total");
            total1.Value = "0";
            data_bc.Attributes.Append(total1);
            xml_bc.Save(file[1]);
        }

        //set toan bo trang thai sensor, actor ve false
        public int setAllFalse()
        {
            try
            {
                XmlNodeList nodeSensor = xml.GetElementsByTagName("node");
                foreach (XmlNode node in nodeSensor)
                {
                    if (node.Attributes["mac"].Value == "00")
                    {
                        node.Attributes["status"].Value = "true";
                    }
                    else
                    {
                        node.Attributes["status"].Value = "false";
                    }
                }
                xml.Save(file[0]);
                return 1;
            }
            catch
            {
                return -1;
            }
        }
        //set toan bo ve false tru dong ho bao chay la coor
        public int setFalseActor()
        {
            try
            {
                XmlNodeList nodeSensor = xml.GetElementsByTagName("node");
                foreach (XmlNode node in nodeSensor)
                {
                    if (node.Attributes["mac"].Value == "B1")
                    {
                        node.Attributes["status"].Value = "true";
                    }
                    else
                    {
                        node.Attributes["status"].Value = "false";
                    }
                }
                xml.Save(file[0]);
                return 1;
            }
            catch
            {
                return -1;
            }
        }
        //set all false sensor bao chay
        public int setFalseBC()
        {
            try
            {
                XmlNodeList nodeSensor = ((XmlElement)sensor_bc).GetElementsByTagName("node");
                foreach (XmlNode nodeBC in nodeSensor)
                {
                    nodeBC.Attributes[2].Value = "false";
                }
                xml_bc.Save(file[1]);
                return 1;
            }
            catch
            {
                return -1;
            }
        }

        /***************************************************************************************
         Cac ham phuc vu khu vuc canh bao chay
         * **************************************************************************************/
        //set dia chi mang node sensor khu bao chay
        public int setNetworkIpSensorBC(string mac, string ip)
        {
            try
            {
                XmlElement element = xml_bc.DocumentElement;
                XmlNode myxmlnode = element.SelectSingleNode("//root/sensor/node[@mac = '" + mac.ToString() + "']");
                myxmlnode.Attributes["network_ip"].Value = ip.ToString();
                xml_bc.Save(file[1]);
                return 1;
            }
            catch
            {
                return -1;
            }
        }

        //set status node sensor khu bao chay
        public int setStatusSensorBC(string mac, bool status)
        {
            try
            {
                XmlElement element = xml_bc.DocumentElement;
                XmlNode myxmlnode = element.SelectSingleNode("//root/sensor/node[@mac = '" + mac.ToString() + "']");
                myxmlnode.Attributes["status"].Value = status.ToString();
                xml_bc.Save(file[1]);
                return 1;
            }
            catch
            {
                return -1;
            }
        }

        // Lay dia chi mang node sensor khu bao chay
        public string getNetworkIpSensorBC(string mac)
        {
            try
            {
                XmlElement element = xml_bc.DocumentElement;
                XmlNode myxmlnode = element.SelectSingleNode("//root/sensor/node[@mac='" + mac.ToString() + "']");
                return myxmlnode.Attributes[1].Value;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        //tao node luu nhiet do do am
        public void SaveDataDB(float temp, float humi)
        {
            int total_data = Convert.ToInt32(data_bc.Attributes["total"].Value);
            XmlNode node_data = xml_bc.CreateElement("node");

            XmlAttribute id = xml_bc.CreateAttribute("id");
            id.Value = (++total_data).ToString();
            node_data.Attributes.Append(id);

            XmlAttribute temperature = xml_bc.CreateAttribute("temperature");
            temperature.Value = temp.ToString();
            node_data.Attributes.Append(temperature);


            XmlAttribute humidity = xml_bc.CreateAttribute("humidity");
            humidity.Value = humi.ToString();
            node_data.Attributes.Append(humidity);

            data_bc.AppendChild(node_data);
            data_bc.Attributes[0].Value = total_data.ToString();


            xml_bc.Save(file[1]);
        }

        //luu dia chi mac, mang sensor khu bao chay
        public void setSensor_bc(string mac_sensor, string network_ip_sensor, bool status_sensor)
        {
            try
            {
                int total_sensor_bc = Convert.ToInt32(sensor_bc.Attributes["total"].Value);

                XmlNode node_sensor = xml_bc.CreateElement("node");

                XmlAttribute mac = xml_bc.CreateAttribute("mac");
                mac.Value = mac_sensor.ToString();
                node_sensor.Attributes.Append(mac);

                XmlAttribute network_ip = xml_bc.CreateAttribute("network_ip");
                network_ip.Value = network_ip_sensor.ToString();
                node_sensor.Attributes.Append(network_ip);
                
                XmlAttribute status = xml_bc.CreateAttribute("status");
                status.Value = status_sensor.ToString();
                node_sensor.Attributes.Append(status);
                
                XmlAttribute pixel_x = xml_bc.CreateAttribute("pixel_x");
                pixel_x.Value = "0";
                node_sensor.Attributes.Append(pixel_x);
                
                XmlAttribute pixel_y = xml_bc.CreateAttribute("pixel_y");
                pixel_y.Value = "0";
                node_sensor.Attributes.Append(pixel_y);

                sensor_bc.AppendChild(node_sensor);

                total_sensor_bc++;
                sensor_bc.Attributes[0].Value = total_sensor_bc.ToString();

                xml_bc.Save(file[1]);
            }
            catch
            {
            }
        }

        //Kiem tra sensor bao chay
        public string CheckSensorBC(string mac)
        {
            try
            {
                string checksensor = "false";
                XmlNodeList nodeSensor = ((XmlElement)sensor_bc).GetElementsByTagName("node");
                foreach (XmlNode node in nodeSensor)
                {
                    if (node.Attributes[0].Value == mac.ToString())
                    {
                        checksensor = "true";
                    }
                }
                return checksensor;
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }

        //ham tinh trung binh nhiet do
        public float SumTemp()
        {
            try
            {
                float sumTemp = 0;
                int total_data = Convert.ToInt32(data_bc.Attributes["total"].Value);
                XmlNodeList nodelist = ((XmlElement)data_bc).GetElementsByTagName("node");
                foreach (XmlNode nodedata in nodelist)
                {
                    sumTemp += float.Parse(nodedata.Attributes["temperature"].Value);
                }
                return sumTemp / total_data;
            }
            catch
            {
                return -1;
            }
        }

        //ham tinh trung binh do am
        public float SumHumi()
        {
            try
            {
                float sumHumi = 0;
                int total_data = Convert.ToInt32(data_bc.Attributes["total"].Value);
                XmlNodeList nodelist = ((XmlElement)data_bc).GetElementsByTagName("node");
                foreach (XmlNode nodedata in nodelist)
                {
                    sumHumi += float.Parse(nodedata.Attributes["humidity"].Value);
                }
                return sumHumi / total_data;
            }
            catch
            {
                return -1;
            }
        }

         /***************************************************************************************
         *Cac ham update co so du lieu
         * **************************************************************************************/
        //update sensor khi gui du lieu
        public void updateSensor(string mac, string ip, float temp, float humi, float energySensor, string time)
        {
            try
            {
                XmlElement element = xml.DocumentElement;
                XmlNode node = element.SelectSingleNode("//root/sensor/node[@mac = '" + mac.ToString() + "']");
                node.Attributes[1].Value = ip.ToString();
                node.Attributes[5].Value = temp.ToString();
                node.Attributes[6].Value = humi.ToString();
                node.Attributes[7].Value = energySensor.ToString();
                node.Attributes[9].Value = time.ToString();
                xml.Save(file[0]);
            }
            catch
            {
            }
        }
        
         /***************************************************************
         * Cau hinh tu may tinh nhung
         **************************************************************/
        //Luu thoi gian tu dong tat bom
        public int setTimeActor(int value)
        {
            try
            {
                XmlElement element = xml.DocumentElement;
                XmlNode myxmlnode = element.SelectSingleNode("//root/general/config");
                myxmlnode.Attributes["control"].Value = value.ToString();
                xml.Save(file[0]);
                return 1;
            }
            catch
            {
                return -1;
            }
        }

        //Lay thoi gian tu dong tat bom
        public int getTimeActor()
        {
            try
            {
                XmlElement element = xml.DocumentElement;
                XmlNode myxmlnode = element.SelectSingleNode("//root/general/config");
                return int.Parse(myxmlnode.Attributes["control"].Value);
            }
            catch
            {
                return -1;
            }
        }

        //luu thoi gian update dong ho bao chay
        public int setTimeAlarm(int value)
        {
            try
            {
                XmlElement element = xml.DocumentElement;
                XmlNode myxmlnode = element.SelectSingleNode("//root/general/config");
                myxmlnode.Attributes["alarm"].Value = value.ToString();
                xml.Save(file[0]);
                return 1;
            }
            catch
            {
                return -1;
            }
        }

        ////Lay thoi gian update dong ho bao chay
        public int getTimeAlarm()
        {
            try
            {
                XmlElement element = xml.DocumentElement;
                XmlNode myxmlnode = element.SelectSingleNode("//root/general/config");
                return int.Parse(myxmlnode.Attributes["alarm"].Value);
            }
            catch
            {
                return -1;
            }
        }

        /***************************************************
         * 
         * *************************************************/
        //Lay thoi gian van da tuoi
        public int getTimeVan(int id)
        {
            try
            {
                XmlElement element = xml.DocumentElement;
                XmlNode myxmlnode = element.SelectSingleNode("//root/van/val[@id='" + id.ToString() + "']");
                return Convert.ToInt32(myxmlnode.Attributes[4].Value);

            }
            catch
            {
                return -1;
            }
        }

        //set thoi gian bat bom
        public int setTimeVan(int id, int time)
        {
            try
            {
                XmlElement element = xml.DocumentElement;
                XmlNode myxmlnode = element.SelectSingleNode("//root/van/val[@id = '" + id.ToString() + "']");
                myxmlnode.Attributes[4].Value = time.ToString();
                xml.Save(file[0]);
                return 1;
            }
            catch
            {
                return -1;
            }
        }
        //set khoang thoi gian bat dau dien ra viec tuoi
        public int setTimeStart(int time)
        {
            try
            {
                XmlElement element = xml.DocumentElement;
                XmlNode myxmlnode = element.SelectSingleNode("//root/general/config");
                myxmlnode.Attributes["start"].Value = time.ToString();
                xml.Save(file[0]);
                return 1;
            }
            catch
            {
                return -1;
            }
        }
        //set khoang thoi gian ket thuc viec tuoi
        public int setTimeFinish(int time)
        {
            try
            {
                XmlElement element = xml.DocumentElement;
                XmlNode myxmlnode = element.SelectSingleNode("//root/general/config");
                myxmlnode.Attributes["finish"].Value = time.ToString();
                xml.Save(file[0]);
                return 1;
            }
            catch
            {
                return -1;
            }
        }
        //Lay thoi gian bat dau tuoi
        public int getTimeStart()
        {
            try
            {
                XmlElement element = xml.DocumentElement;
                XmlNode myxmlnode = element.SelectSingleNode("//root/general/config");
                return int.Parse(myxmlnode.Attributes["start"].Value);
            }
            catch
            {
                return -1;
            }
        }
        //Lay thoi gian bat dau tuoi
        public int getTimeFinish()
        {
            try
            {
                XmlElement element = xml.DocumentElement;
                XmlNode myxmlnode = element.SelectSingleNode("//root/general/config");
                return int.Parse(myxmlnode.Attributes["finish"].Value);
            }
            catch
            {
                return -1;
            }
        }
    }
}
