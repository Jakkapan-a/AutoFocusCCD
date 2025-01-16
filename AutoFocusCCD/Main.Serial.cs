using AutoFocusCCD.Utilities;
using System;

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
                Logger.Info("Received empty packet.");
                return;
            }

            switch(e.PacketData.Command)
            {
                case (byte)CommandType.Data:
                    if (e.PacketData.Mode1 == 0x01 && e.PacketData.Mode2 == 0x02)
                    {
                        // Create a new SensorData object
                        SensorData sensorData = new SensorData();

                        // Copy the first 4 bytes to voltageBytes
                        byte[] voltageBytes = new byte[4];
                        Array.Copy(e.PacketData.Value, 0, voltageBytes, 0, 4);
                        sensorData.voltage_V = BitConverter.ToSingle(voltageBytes, 0);

                        // Copy the next 4 bytes to currentBytes
                        byte[] currentBytes = new byte[4];
                        Array.Copy(e.PacketData.Value, 4, currentBytes, 0, 4);
                        sensorData.current_mA = BitConverter.ToSingle(currentBytes, 0);

                        Console.WriteLine($"Voltage: {sensorData.voltage_V:F2} V, Current: {sensorData.current_mA:F2} mA, Power: {(sensorData.voltage_V * sensorData.current_mA / 1000):F2} W");
                    }
                    break;
                default:
                    //Logger.Info("Received packet: " + e.PacketData.ToString());
                    break;
            }
        }
    }
}
