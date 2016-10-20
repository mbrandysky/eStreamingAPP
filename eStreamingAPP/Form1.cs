using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;
using ICSharpCode.SharpZipLib.GZip;
using System.Threading;

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
                result = "";

                //-------------------------------
                // Vytazeni informace z JSONu
                //-------------------------------
                string dataSource = (dict["data"]["base64GzippedResponse"]);
                //textBox1.Text = dataSource;

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

                string setTypeOfSearch = "Direct";

                try
                {


                    dataGridView1.Invoke((MethodInvoker)delegate { dataGridView1.ColumnCount = 9;});

                    dataGridView1.Invoke((MethodInvoker)delegate { dataGridView1.Columns[0].Name = "FlightType"; });
                    dataGridView1.Invoke((MethodInvoker)delegate { dataGridView1.Columns[1].Name = "PointOfSale"; });
                    dataGridView1.Invoke((MethodInvoker)delegate { dataGridView1.Columns[2].Name = "Origin"; });
                    dataGridView1.Invoke((MethodInvoker)delegate { dataGridView1.Columns[3].Name = "Destination"; });
                    dataGridView1.Invoke((MethodInvoker)delegate { dataGridView1.Columns[4].Name = "DepartureDate"; });
                    dataGridView1.Invoke((MethodInvoker)delegate { dataGridView1.Columns[5].Name = "ReturnDate"; });
                    dataGridView1.Invoke((MethodInvoker)delegate { dataGridView1.Columns[6].Name = "PlatingCarrier"; });
                    dataGridView1.Invoke((MethodInvoker)delegate { dataGridView1.Columns[7].Name = "Amount"; });
                    dataGridView1.Invoke((MethodInvoker)delegate { dataGridView1.Columns[8].Name = "Currency"; });



                    int pocetRadku = 0;
                    while (pocetRadku < pocetRadku + 1)
                    {


                        try
                        {

                            if ((item[pocetRadku]["Direct"]) != null)
                            { setTypeOfSearch = "Direct"; }

                        }
                        catch
                        {

                            setTypeOfSearch = "Connected";


                        }


                        string pointOfSale = (item[pocetRadku]["pointOfSale"]);
                        string origin = (item[pocetRadku]["origin"]);
                        string destination = (item[pocetRadku]["destination"]);

                        string flightType = setTypeOfSearch;
                        string direct_departureDate = (item[pocetRadku][setTypeOfSearch]["departureDate"]);
                        string direct_returnDate = (item[pocetRadku][setTypeOfSearch]["returnDate"]);
                        string direct_currency = (item[pocetRadku][setTypeOfSearch]["currency"]);
                        string direct_platingCarrier = (item[pocetRadku][setTypeOfSearch]["platingCarrier"]);
                        decimal direct_amount = (item[pocetRadku][setTypeOfSearch]["amount"]);

                              dataGridView1.Invoke((MethodInvoker)delegate { dataGridView1.Rows.Add(flightType, pointOfSale, origin, destination, direct_departureDate, direct_returnDate, direct_platingCarrier, direct_amount, direct_currency); });

                        dataGridView1.Invoke((MethodInvoker)delegate { textBox2.Text = (pocetRadku+1).ToString(); });

                        pocetRadku++;

                    }


                }

                catch 
                {

                    dataGridView1.Invoke((MethodInvoker)delegate { dataGridView1.Sort(dataGridView1.Columns[7], System.ComponentModel.ListSortDirection.Ascending); });

                    dataGridView1.Invoke((MethodInvoker)delegate { textBox5.Text = "Completed"; });

                    

                }


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








        private void button1_Click(object sender, EventArgs e)
        {
            //-------------------------------
            // Stazeni JSONu
            //-------------------------------

            dataGridView1.Rows.Clear();
            textBox5.Text = "Retrieving and decompressing data";

            try
            {
                WebClient webClient = new WebClient();

                webClient.Headers.Add("AuthToken", textBox4.Text);
                string result = webClient.DownloadString("https://api.travelcloudpro.eu/v1/cache/flyfrom?origin=" + textBox3.Text + "&pointOfSale=" + textBox1.Text);

                // Demo: https://demo.travelcloudpro.eu/v1/flyfrom?origin=
  


                //-------------------------------
                // Deserializace JSONu
                //-------------------------------
                var jss = new JavaScriptSerializer();
                var dict = jss.Deserialize<Dictionary<string, dynamic>>(result);
                result = "";

                //-------------------------------
                // Vytazeni informace z JSONu
                //-------------------------------
                string dataSource = (dict["data"]["base64GzippedResponse"]);
                //textBox1.Text = dataSource;

                //-------------------------------
                // Decompress(dataSource);
                //-------------------------------
                string decompressedData = Decompress(dataSource);
                dataSource = "";
                //textBox2.Text = decompressedData;


                //-------------------------------
                //Deserializace JSONu
                //-------------------------------

                var serializer = new JavaScriptSerializer();

                dynamic item = serializer.Deserialize<object>(decompressedData);

                string setTypeOfSearch = "Direct";

                try
                {
                    


                    dataGridView1.ColumnCount = 9;


                    dataGridView1.Columns[0].Name = "FlightType";
                    dataGridView1.Columns[1].Name = "PointOfSale";
                    dataGridView1.Columns[2].Name = "Origin";
                    dataGridView1.Columns[3].Name = "Destination";
                    dataGridView1.Columns[4].Name = "DepartureDate";
                    dataGridView1.Columns[5].Name = "ReturnDate";
                    dataGridView1.Columns[6].Name = "PlatingCarrier";
                    dataGridView1.Columns[7].Name = "Amount";
                    dataGridView1.Columns[8].Name = "Currency";




                    int pocetRadku = 0;
                    while (pocetRadku < pocetRadku + 1)
                    {


                        // setTypeOfSearch = "Connected";

                        try
                            {

                                if ((item[pocetRadku]["Direct"]) != null)
                                { setTypeOfSearch = "Direct"; }

                            }
                            catch
                            {

                            setTypeOfSearch = "Connected";


                        }


                                string pointOfSale = (item[pocetRadku]["pointOfSale"]);
                                string origin = (item[pocetRadku]["origin"]);
                                string destination = (item[pocetRadku]["destination"]);

                                string flightType = setTypeOfSearch;
                                string direct_departureDate = (item[pocetRadku][setTypeOfSearch]["departureDate"]);
                                string direct_returnDate = (item[pocetRadku][setTypeOfSearch]["returnDate"]);
                                string direct_currency = (item[pocetRadku][setTypeOfSearch]["currency"]);
                                string direct_platingCarrier = (item[pocetRadku][setTypeOfSearch]["platingCarrier"]);
                                decimal direct_amount = (item[pocetRadku][setTypeOfSearch]["amount"]);

                                //string[] row = new string[] { flightType, pointOfSale, origin, destination, direct_departureDate, direct_returnDate, direct_platingCarrier, direct_amount.ToString(), direct_currency };
                                //dataGridView1.Rows.Add(row);
                                dataGridView1.Rows.Add(flightType, pointOfSale, origin, destination, direct_departureDate, direct_returnDate, direct_platingCarrier, direct_amount, direct_currency);

                        textBox2.Text = pocetRadku.ToString();
                        pocetRadku++;

                    }
        

                }

                catch 
                {
                    //dataGridView1.Rows.Add("End");
                    //MessageBox.Show(ex.ToString());
                    dataGridView1.Sort(dataGridView1.Columns[7], System.ComponentModel.ListSortDirection.Ascending);
                    textBox5.Text = "Completed";

                }


            }
            catch (Exception exc)
            {
                if ((exc.ToString().Contains("401")) || (exc.ToString().Contains("403")) )
                {

                    MessageBox.Show("You are not authorized");

                }


                else
                {

                    MessageBox.Show("Nothing to display");

                }
                



            }
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
    }
}
