using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;
using ICSharpCode.SharpZipLib.GZip;
using System.Threading;
using System.ComponentModel;

namespace eStreamingAPP
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
          

    }
        private Thread t;

        private string Decompress(string text)
        {
            Byte[] bytes = Convert.FromBase64String(text);
            MemoryStream memStream = new MemoryStream(bytes);
            GZipInputStream gzipStream = new GZipInputStream(memStream);
            byte[] data = new byte[2048];
            char[] chars = new char[2048]; 
            Decoder decoder = Encoding.UTF8.GetDecoder();
            StringBuilder sb = new StringBuilder();

            while (true)
            {
                int size = gzipStream.Read(data, 0, data.Length);
                if (size == 0)
                    break;
                int n = decoder.GetChars(data, 0, size, chars, 0);
                sb.Append(chars, 0, n);
            }
            return sb.ToString();

        }


        public void VykonaniProgramu()
        {
            //-------------------------------
            // Stazeni JSONu
            //-------------------------------
            dataGridView1.Invoke((MethodInvoker)delegate { dataGridView1.Rows.Clear(); });
            textBox5.Invoke((MethodInvoker)delegate { textBox5.Text = "Retrieving and decompressing data"; });

            try
            {
                WebClient webClient = new WebClient();
                webClient.Headers.Add("AuthToken", textBox4.Text);
                string result = webClient.DownloadString("https://api.travelcloudpro.eu/v1/cache/flyfrom?origin=" + textBox3.Text + "&pointOfSale=" + textBox1.Text);

                // Demo: https://demo.travelcloudpro.eu/v1/flyfrom?origin=  
                // Prod: https://api.travelcloudpro.eu/v1/cache/flyfrom?origin=



                //-------------------------------
                // Deserializace JSONu
                //-------------------------------
                var jss = new JavaScriptSerializer();
                var dict = jss.Deserialize<Dictionary<string, dynamic>>(result);

                //-------------------------------
                // Vytazeni informace z JSONu
                //-------------------------------
                string dataSource = (dict["data"]["base64GzippedResponse"]);

                //-------------------------------
                // Decompress(dataSource);
                //-------------------------------
                string decompressedData = Decompress(dataSource);
                dataSource = "";

                //-------------------------------
                //Deserializace JSONu
                //-------------------------------
                var serializer = new JavaScriptSerializer();
                dynamic item = serializer.Deserialize<object>(decompressedData);

                int resultCount = 0;

                dataGridView1.Invoke((MethodInvoker)delegate { dataGridView1.ColumnCount = 9; });


                dataGridView1.Invoke((MethodInvoker)delegate { dataGridView1.Columns[0].Name = "Price"; });
                dataGridView1.Invoke((MethodInvoker)delegate { dataGridView1.Columns[1].Name = "Currency"; });
                dataGridView1.Invoke((MethodInvoker)delegate { dataGridView1.Columns[2].Name = "From"; });
                dataGridView1.Invoke((MethodInvoker)delegate { dataGridView1.Columns[3].Name = "To"; });
                dataGridView1.Invoke((MethodInvoker)delegate { dataGridView1.Columns[4].Name = "There"; });
                dataGridView1.Invoke((MethodInvoker)delegate { dataGridView1.Columns[5].Name = "Back"; });
                dataGridView1.Invoke((MethodInvoker)delegate { dataGridView1.Columns[6].Name = "Carrier"; });
                dataGridView1.Invoke((MethodInvoker)delegate { dataGridView1.Columns[7].Name = "Last modified"; });
                dataGridView1.Invoke((MethodInvoker)delegate { dataGridView1.Columns[7].Width = 160; });

                dataGridView1.Invoke((MethodInvoker)delegate { dataGridView1.Columns[8].Name = "Age in hours"; });
                dataGridView1.Invoke((MethodInvoker)delegate { dataGridView1.Columns[8].Width = 90; });

      
                string flightTypeCheckBox = "RT_Connected";
                if (checkBox1.Checked)
                { flightTypeCheckBox = "OW_Connected"; }
                
                foreach (string keyDestIATA in item.Keys)
                {

                    foreach (string keyFlightType in item[keyDestIATA].Keys)
                    {
                        
                        if (keyFlightType == flightTypeCheckBox)
                            
                        {
                            string lastModifiedCell = (item[keyDestIATA][keyFlightType]["lastModified"]);
                            decimal recordAgeInMsCell = (item[keyDestIATA][keyFlightType]["recordAgeInMs"]) / 3600000;
                            decimal amountCell = (item[keyDestIATA][keyFlightType]["amount"]);
                            string currencyCell = (item[keyDestIATA][keyFlightType]["currency"]);
                            string validatingCarrierCell = (item[keyDestIATA][keyFlightType]["validatingCarrier"]);
                            string originalRequest = (item[keyDestIATA][keyFlightType]["originalRequest"]);

                            string departureDateCell = originalRequest.Substring(0, 8);

                            string destinationDateCell = " ";
                            if (flightTypeCheckBox == "RT_Connected")
                            { destinationDateCell = originalRequest.Substring(14, 8); }

                            string departureIATACell = originalRequest.Substring(8, 3);
                            string destinationIATACell = originalRequest.Substring(11, 3);

                            dataGridView1.Invoke((MethodInvoker)delegate { dataGridView1.Rows.Add(amountCell, currencyCell, departureIATACell, destinationIATACell,
                            departureDateCell, destinationDateCell, validatingCarrierCell, lastModifiedCell, recordAgeInMsCell); });

                            resultCount++;

                            
                            textBox2.Invoke((MethodInvoker)delegate { textBox2.Text = resultCount.ToString(); });

                        }

                    }

                }

                textBox5.Invoke((MethodInvoker)delegate { textBox5.Text = "Completed"; });
                dataGridView1.Invoke((MethodInvoker)delegate { dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Ascending); });

            }
            catch (Exception exc)
            {
                if ((exc.ToString().Contains("401")) || (exc.ToString().Contains("403")))
                {

                    MessageBox.Show("You are not authorized");

                }


                else
                {

                    MessageBox.Show("Nothing to display");

                }

            }


            button3.Invoke((MethodInvoker)delegate { button3.Enabled = Enabled; });


        }

     
        


        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            
        }



        private void Form1_Load(object sender, EventArgs e)
        {
       
        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }



        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            t = new Thread(VykonaniProgramu);
            t.Start();
            button3.Enabled = false;

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
