using AutoFocusCCD.Utilities;
using System;
using System.Drawing;
using System.Text;

namespace AutoFocusCCD
{
    partial class Main
    {
        private EnhancedPacketHandler enhancedPacketHandler = null;
        public DeviceControl deviceControl;

        private void InitializeSerial()
        {
            enhancedPacketHandler = new EnhancedPacketHandler();
            enhancedPacketHandler.OnSerialError += EnhancedPacketHandler_OnSerialError;
            enhancedPacketHandler.OnPacketReceived += EnhancedPacketHandler_OnPacketReceivedHandler;
            enhancedPacketHandler.OnPacketReceivedAscii += EnhancedPacketHandler_OnPacketReceivedAscii;
            deviceControl = new DeviceControl(enhancedPacketHandler);
        }

        private void EnhancedPacketHandler_OnPacketReceivedAscii(object sender, PacketAcsiiEventArgs e)
        {
            if (e.PacketData.Length == 0)
            {
                Console.WriteLine("Received packet with no data.");
                return;
            }
            //Console.WriteLine("Received ASCII packet:");
            //Console.WriteLine($"Text: {e.PacketData}");
            //Console.WriteLine($"Length: {e.PacketData.Length} bytes");

            // $INA_DATA: 1.04,-0.50#
            // $SENSOR1:ON#
            // $SENSOR1:OFF#


            // Remove $ #
            string package = e.PacketData.Substring(1, e.PacketData.Length - 2);
            if (package.Contains("INA_DATA"))
            {
                string[] parts = package.Split(':');
                if (parts.Length == 2)
                {
                    string[] values = parts[1].Split(',');
                    if (values.Length == 2)
                    {
                        float voltage = float.Parse(values[0]);
                        float current = float.Parse(values[1]);
                        SensorData sensorData = new SensorData
                        {
                            voltage_V = voltage,
                            current_mA = current
                        };

                        Console.WriteLine($"Voltage: {sensorData.voltage_V:F2} V");
                        Console.WriteLine($"Current: {sensorData.current_mA:F2} mA");

                        byte[] bytes = new byte[8];
                        byte[] voltageBytes = BitConverter.GetBytes(sensorData.voltage_V);
                        byte[] currentBytes = BitConverter.GetBytes(sensorData.current_mA);

                        Array.Copy(voltageBytes, 0, bytes, 0, 4);
                        Array.Copy(currentBytes, 0, bytes, 4, 4);

                        UpdateCurrentVoltage(bytes);
                    }
                }
            }
            else if (package.Contains("SENSOR1"))
            {
                bool state = package.Contains("ON");
                UpdateSensorStatus(state);
            }
        }

        private void EnhancedPacketHandler_OnSerialError(object sender, SerialErrorEventArgs e)
        {
            Logger.Info("Serial error: " + e.Error);
        }

        public bool isSensorActive = false;
        private void EnhancedPacketHandler_OnPacketReceivedHandler(object sender, PacketDataEventArgs e)
        {
            if (e.PacketData.ValueSize == 0)
            {
                Console.WriteLine("Received packet with no data.");
                return;
            }

            switch (e.PacketData.Command)
            {
                case (byte)CommandType.Data:
                    // Update current voltage
                    if (e.PacketData.Mode1 == 0x01 && e.PacketData.Mode2 == 0x02)
                    {
                        UpdateCurrentVoltage(e.PacketData.Value);
                    }
                    else if (e.PacketData.Mode1 == 0x01 && e.PacketData.Mode2 == 0x06)
                    {
                        bool isActive = e.PacketData.Value[0] == 0x01;
                        this.isSensorActive = isActive;
                        if (InvokeRequired)
                        {
                            Invoke(new Action(() => UpdateSensorStatus(isActive)));
                        }
                        else
                        {
                            UpdateSensorStatus(isActive);
                        }
                    }
                    break;
                case (byte)CommandType.Ack:
                    Console.WriteLine("Received ACK packet.");
                    break;

                case (byte)CommandType.Nack:
                    Console.WriteLine("Received NACK packet.");
                    break;
                case (byte)CommandType.Request:
                    Console.WriteLine("Received Request packet.");
                    break;
                case (byte)CommandType.Response:
                    Console.WriteLine("Received Response packet.");
                    if(e.PacketData.Mode1 == 0x01 && e.PacketData.Mode2 == 0x06)
                    {
                        bool isActive = e.PacketData.Value[0] == 0x01;
                        this.isSensorActive = isActive;
                        if(InvokeRequired)
                        {
                            Invoke(new Action(() => UpdateSensorStatus(isActive)));
                        }
                        else
                        {
                            UpdateSensorStatus(isActive);
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private void UpdateSensorStatus(bool isActive)
        {
            if(InvokeRequired)
            {
                Invoke(new Action(() => UpdateSensorStatus(isActive)));
                return;
            }

            this.toolStripStatusLabelSensor.Text = isActive ? "Sensor: Active" : "Sensor: Inactive";
            this.toolStripStatusLabelSensor.ForeColor = isActive ? System.Drawing.Color.Green : System.Drawing.Color.Red;

            if (toolStripStatusLabelSensor.Text == "Sensor: Active")
            {
                if (this.lbTitle.Text != "Waiting for start..." || this._product == null)
                {
                    return;
                }

                if(txtEmp.Text.Length <= 0)
                {
                    this.lbTitle.Text = "Please scan employee card";
                    this.lbTitle.BackColor = Color.Yellow;
                    this.lbTitle.ForeColor = Color.Black;
                    this.txtEmp.Focus();
                    return;
                }

                this.countStart = 3;
                this.lbTitle.Text = $"Start process in {this.countStart} seconds";
                this.lbTitle.BackColor = Color.Orange;
                this.lbTitle.ForeColor = Color.Black;
                this.timerOnStartProcess.Interval = Preferences().Processing.Interval;
                this.timerOnStartProcess.Start();
                this.deviceControl.SetLED(DeviceControl.Mode2Type.LED_BLUE,true);
            }
            else
            {
                timerOnStartProcess.Stop();
                // Clear data
                lbTitle.Text = "Please scan QR code";
                lbTitle.BackColor = Color.Yellow;
                lbTitle.ForeColor = Color.Black;
                this.countStart = 0;
                //this._product = null;
                this.pictureBoxPredict.Visible = false;
                this.txtQr.Text = "";
                this.txtQr.Focus();
                if(this.lbTitle.Text == "Please scan employee card")
                {
                    this.txtEmp.Focus();
                }

                this.deviceControl.TurnOffAllRelays();
                this.deviceControl.TurnOffAllLEDs();

            }

        }

        private SensorData sensorData1 = new SensorData();

        private void UpdateCurrentVoltage(byte[] data)
        {
            if (data.Length != 8)
            {
                Console.WriteLine("Received data with invalid length.");
                return;
            }

            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateCurrentVoltage(data)));
                return;
            }

            // Create a new SensorData object
            SensorData sensorData = new SensorData();
            // Copy the first 4 bytes to voltageBytes
            byte[] voltageBytes = new byte[4];
            Array.Copy(data, 0, voltageBytes, 0, 4);
            sensorData.voltage_V = BitConverter.ToSingle(voltageBytes, 0);
            // Copy the next 4 bytes to currentBytes
            byte[] currentBytes = new byte[4];
            Array.Copy(data, 4, currentBytes, 0, 4);
            sensorData.current_mA = BitConverter.ToSingle(currentBytes, 0);
            // Update the UI
            lbVoltage.Text = $"{sensorData.voltage_V:F2} V";
            lbCurrent.Text = $"{(sensorData.current_mA < 0 ? 0 : sensorData.current_mA):F2} mA";

            sensorData1 = sensorData;
        }

        public void SendText(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                Console.WriteLine("Data is empty.");
                return;
            }

            try
            {
                byte[] bytes = Encoding.ASCII.GetBytes(data);

                // 
                if (bytes.Length > 255)
                {
                    Console.WriteLine($"Data is too long. Max 255 chars, current: {bytes.Length}");
                    return;
                }

                // packet
                var packet = new PacketData
                {
                    Mode1 = 0x01,
                    Mode2 = 0x05,
                    Command = (byte)CommandType.Data,
                    Value = bytes
                };

                // packet
                if (enhancedPacketHandler?.SendPacket(packet) == true)
                {
                    Console.WriteLine("Sent Text Data:");
                    Console.WriteLine($"Hex: {BitConverter.ToString(bytes).Replace("-", " ")}");
                    Console.WriteLine($"Text: {data}");
                    Console.WriteLine($"Length: {bytes.Length} bytes");
                }
                else
                {
                    Console.WriteLine("Failed to send packet.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending text: {ex.Message}");
            }
        }

    }
}
