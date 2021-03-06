﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace ss_gen
{
    public enum CardTypes { BoFA, Citi };
    public partial class Form1 : Form
    {
        List<CardTypes> comboChoices = new List<CardTypes> { CardTypes.BoFA, CardTypes.Citi };

        public Form1()
        {
            InitializeComponent();
            comboBox1.DataSource = comboChoices;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string url = textBox4.Text;
            string sessionId = textBox1.Text;
            string length = numericUpDown1.Value.ToString();
            string amount = amountUpDown.Value.ToString();
            string cookie = textBox3.Text;
            string result = SSGetNumber(url, sessionId, length, amount, cookie: cookie);
            textBox2.Text += result + "\r\n";
        }

        private string SSGetNumber(string backendURI, string sessionId, string length, string amount, string cookie = null)
        {

            using (var client = new WebClient())
            {
                client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                string data = "";
                //var data = new {
                //    CardType = "7",
                //    CumulativeLimit = "11.00",
                //    IssuerId = "1",
                //    CPNType = "MA",
                //    VCardId = "3652249",
                //    MsgNo = "5",
                //    Locale = "en",
                //    Request = "GetCPN",
                //    Version = "FLEXWEBCARD-BOFA_4_0_31_0",
                //    SessionId = sessionId,
                //    ValidFor = "12"
                //};
                if ((CardTypes)comboBox1.SelectedItem == CardTypes.BoFA)
                {
                    data =
                        "CardType=13&CumulativeLimit=" + 900 + "&IssuerId=1&CPNType=MA&VCardId=4705867&MsgNo=2&Locale=en&Request=GetCPN&Version=FLEXWEBCARD-BOFA_4_0_46_0&SessionId=" + sessionId + "&ValidFor=" + 12;
                }
                /*
                    Request:GetCPN
                    Version:FLEXWEBCARD-CITI_4_0_547_0
                    Indicator:VANGen
                    IssuerId:1
                    CPNType:MA
                    WebcardType:THIN
                    ValidFor:12
                    Locale:en
                    MsgNo:3
                    SessionId:xxx
                    VCardId:6199644
                    CardType:2542418
                    CumulativeLimit:21
                 */
                if ((CardTypes)comboBox1.SelectedItem == CardTypes.Citi)
                {
                    data =
                    // "Indicator=VANGen&WebcardType=THIN&CardType=2542418&CumulativeLimit=" + 900 + "&IssuerId=1&CPNType=MA&VCardId=6199644&MsgNo=5&Locale=en&Request=GetCPN&Version=FLEXWEBCARD%2DCITI%5F4%5F0%5F547%5F0&SessionId=" + sessionId + "&ValidFor=" + 12;
                    "WebcardType=THIN&SessionId=" + sessionId + "&Indicator=VANGen&Locale=en&Request=GetCPN&CPNType=SP&Version=FLEXWEBCARD%2DCITI%5F4%5F0%5F547%5F0&VCardId=8899549&MsgNo=1&CardType=2542418&IssuerId=1" + "&CumulativeLimit=" + 9000 + "&ValidFor=" + 12;
                }
                if (cookie != null)
                {
                    client.Headers.Add("Cookie", cookie);
                }

                //disable tls1.0
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                var responseString = client.UploadString(backendURI, "POST", data);
                NameValueCollection responseCollection = HttpUtility.ParseQueryString(responseString);

                if (responseCollection["Action"] == "Error")
                {
                    return "";
                }

                string ccStr = responseCollection["PAN"];
                ccStr = ccStr.Substring(0, 4) + " " + ccStr.Substring(4, 4) + " " + ccStr.Substring(8, 4) + " " +
                        ccStr.Substring(12, 4);
                string expStr = responseCollection["Expiry"];
                string cvvStr = responseCollection["AVV"];
                string result = ccStr + "," + cvvStr + "," + expStr + ",$" + 900;

                return result;
            }

            return "";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 5; i++)
            {
                button1_Click(sender, e);
            }
        }
    }
}