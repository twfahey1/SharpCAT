using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;
using System.Diagnostics;

namespace SimpleSerial
{

    

    public partial class Form1 : Form
    {
        // Add this variable 
        string RxString;
        string trannedStroke;

        bool realtimeStarted = false;
        List<DictionaryEntry> LoadedDictionary = new List<DictionaryEntry>();

        public Form1()
        {
            InitializeComponent();
        }

        

        private void buttonStart_Click(object sender, EventArgs e)
        {
            if (!realtimeStarted)
            {
                if ((portComboBox.Text == "Available Ports") || (portComboBox.Text == ""))
                {
                    MessageBox.Show("Select a valid COM port for writer.");
                    return;
                }

                portComboBox.Enabled = false;
                baudRateCombo.Enabled = false;
                this.buttonStart.Text = "Stop";

                
                serialPort1.PortName = portComboBox.Text;
                serialPort1.BaudRate = Int32.Parse(baudRateCombo.Text);
                serialPort1.Open();

                if (serialPort1.IsOpen)
                {                    
                    textBox1.ReadOnly = false;
                }

                realtimeStarted = true;
            }           

            else
            {
                if (serialPort1.IsOpen)
                {
                    serialPort1.Close();
                    textBox1.ReadOnly = true;
                }

                portComboBox.Enabled = true;
                baudRateCombo.Enabled = true;
                realtimeStarted = false;
                buttonStart.Text = "Start";
            }
            
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (serialPort1.IsOpen) serialPort1.Close();
            Properties.Settings.Default.lastComPort = portComboBox.Text;
            Properties.Settings.Default.lastBaudRate = baudRateCombo.Text;
            Properties.Settings.Default.Save();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            // If the port is closed, don't try to send a character.
            if (!serialPort1.IsOpen) return;

            // If the port is Open, declare a char[] array with one element.
            char[] buff = new char[1];

            // Load element 0 with the key character.
            buff[0] = e.KeyChar;

            // Send the one character buffer.
            serialPort1.Write(buff, 0, 1);

            // Set the KeyPress event as handled so the character won't
            // display locally. If you want it to display, omit the next line.
            e.Handled = true;
        }

        private void DisplayText(object sender, EventArgs e)
        {
            try
            {
                trannedStroke = null;
                rawList.Items.Add(RxString);
                String[] splitInput = RxString.Split('/');
                var rawStroke = splitInput[1];
                rawStroke = rawStroke.Replace("SeCe", "S");
                rawStroke = rawStroke.Replace("Te", "T");
                rawStroke = rawStroke.Replace("Pe", "P");
                rawStroke = rawStroke.Replace("He", "H");

                rawStroke = rawStroke.Replace("Fe", "F");
                rawStroke = rawStroke.Replace("Ne", "-P");
                rawStroke = rawStroke.Replace("Le", "L");
                rawStroke = rawStroke.Replace("Ye", "T");

                rawStroke = rawStroke.Replace("Ke", "K");
                rawStroke = rawStroke.Replace("We", "W");
                rawStroke = rawStroke.Replace("Re", "R");
                rawStroke = rawStroke.Replace("Kd", "K");
                rawStroke = rawStroke.Replace("Wd", "W");
                rawStroke = rawStroke.Replace("Rd", "R");

                rawStroke = rawStroke.Replace("De", "D");
                rawStroke = rawStroke.Replace("Ze", "Z");

                rawStroke = rawStroke.Replace("Qe", "R");
                rawStroke = rawStroke.Replace("Be", "B");
                rawStroke = rawStroke.Replace("Ge", "G");
                rawStroke = rawStroke.Replace("Xe", "S");

                rawStroke = rawStroke.Replace("Ae", "A");
                rawStroke = rawStroke.Replace("Oe", "O");
                rawStroke = rawStroke.Replace("Ee", "E");
                rawStroke = rawStroke.Replace("Ue", "U");

                for (int i = 0; i <= 10; i++)
                {
                    if (rawStroke.Contains(i.ToString()))
                    {
                        //rawStroke = rawStroke.Replace(i.ToString(), "");
                        try
                        {
                            int indexOfNumber = rawStroke.IndexOf(i.ToString());
                            int indexOfShadow = rawStroke.IndexOf(i.ToString()) - 1;
                            rawStroke = rawStroke.Replace(rawStroke[indexOfNumber].ToString(), "");
                            rawStroke = rawStroke.Replace(rawStroke[indexOfShadow].ToString(), "");

                        }
                        catch (Exception ex)
                        {
                            exList.Items.Add(ex.ToString());
                        }
                        break;
                    }

                }
            

                foreach (DictionaryEntry entry in LoadedDictionary)
                {
                    if (entry.Steno == rawStroke)
                    {
                        trannedStroke = entry.Text;
                    }
                }
            
                //Hardcoded dictionary entry test...
                if (rawStroke == "S-T-P-H-") rawStroke = "\n";
                if (trannedStroke != null)
                {

                    textBox1.AppendText(trannedStroke + " ");
                }
                else
                {
                    textBox1.AppendText(rawStroke + " ");
                }
            }
            catch (Exception exx)
            {

            }
        }

        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            //MessageBox.Show("data");

            RxString = serialPort1.ReadExisting();
            this.Invoke(new EventHandler(DisplayText));
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string[] availablePorts = SerialPort.GetPortNames();
            foreach (string port in availablePorts)
            {

                portComboBox.Items.Add(port);
            }

            if (Properties.Settings.Default.lastBaudRate != null)
            {
                baudRateCombo.Text = Properties.Settings.Default.lastBaudRate;
            }

            if (Properties.Settings.Default.lastComPort != null)
            {
                portComboBox.Text = Properties.Settings.Default.lastComPort;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {

            this.Close();
        }

        private void sendDataBtn_Click(object sender, EventArgs e)
        {
            
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stream myStream = null;
            OpenFileDialog theDialog = new OpenFileDialog();
            theDialog.Title = "Open Text File";
            theDialog.Filter = "TXT files|*.txt";
            theDialog.InitialDirectory = @"C:\";
            string line;
            if (theDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = theDialog.OpenFile()) != null)
                    {
                        using (myStream)
                        {
                            System.IO.StreamReader file = new System.IO.StreamReader(myStream);
                            while ((line = file.ReadLine()) != null)
                            {
                                textBox1.AppendText(line);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(saveFile.FileName, textBox1.Text);
            }
        }

        private void deviceManagerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            Process.Start("devmgmt.msc");
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void loadDictionaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stream myStream = null;
            OpenFileDialog theDialog = new OpenFileDialog();
            theDialog.Title = "Open Dictionary File";
            theDialog.Filter = "RTF files|*.rtf";
            theDialog.InitialDirectory = @"C:\";
            string line;
            if (theDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = theDialog.OpenFile()) != null)
                    {
                        using (myStream)
                        {
                            System.IO.StreamReader file = new System.IO.StreamReader(myStream);
                            while ((line = file.ReadLine()) != null)
                            {
                                try
                                {
                                    string stenoEntry = "";
                                    string textDefinition = "";
                                    string[] lineSplit = line.Split('\\');
                                    string[] definitionSplit = line.Split('}');
                                    textDefinition = definitionSplit[1];
                                    //lineSplit[2].Replace("cxs ", "");
                                    string[] stenoSplit = lineSplit[2].Split('/');
                                    
                                    for (int i = 0; i < stenoSplit.Length; i++)
                                    {
                                        stenoEntry = stenoEntry + stenoSplit[i];
                                    }

                                    stenoSplit = stenoEntry.Split('}');
                                    stenoEntry = stenoSplit[0];
                                    stenoEntry = stenoEntry.Replace("cxs ", "");

                                    LoadedDictionary.Add(new DictionaryEntry() { Steno = stenoEntry, Text = textDefinition });

                                }
                                catch (Exception ex)
                                {
                                    Console.Write(ex.ToString());
                                }

                                
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
            foreach (DictionaryEntry entry in LoadedDictionary)
            {
                rawList.Items.Add(entry.Steno + " - " + entry.Text);
            }
        }
    }
}