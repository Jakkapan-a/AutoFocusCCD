using AutoFocusCCD.Utilities;
using System;
using System.Text;

namespace AutoFocusCCD
{
    partial class Main
    {
        private EnhancedPacketHandler enhancedPacketHandler = null;
        private void InitializeSerial()
        {
            enhancedPacketHandler = new EnhancedPacketHandler();
            enhancedPacketHandler.OnSerialError += EnhancedPacketHandler_OnSerialError;
            enhancedPacketHandler.OnPacketReceived += EnhancedPacketHandler_OnPacketReceivedHandler;
        }

        private void EnhancedPacketHandler_OnSerialError(object sender, SerialErrorEventArgs e)
        {
            Logger.Info("Serial error: " + e.Error);
        }

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
                        if (isActive)
                        {
                            Console.WriteLine("Received data packet with active status.");
                        }
                        else
                        {
                            Console.WriteLine("Received data packet with inactive status.");
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
                    break;
                default:
                    break;
            }
        }

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
            lbCurrent.Text = $"{(sensorData.current_mA < 0 ? 0 : sensorData.current_mA):F2}";
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
