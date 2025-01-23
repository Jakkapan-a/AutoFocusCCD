using System;
using System.IO.Ports;
using System.Threading;


namespace AutoFocusCCD.Utilities
{
    /// <summary>
    /// Structure to hold sensor data
    /// </summary>
    [Serializable]
    struct SensorData
    {
        public float voltage_V;
        public float current_mA;
    }
    /// <summary>
    /// Constants for packet protocol
    /// </summary>
    public static class PacketConstants
    {
        public const byte START_BYTE = 0xAA;
        public const byte END_BYTE = 0xFF;
        public const int MAX_PACKET_SIZE = 512;
        public const int HEADER_SIZE = 7;     // START + SIZE(2) + MODE1 + MODE2 + CMD + SEQ
        public const int FOOTER_SIZE = 3;     // LENGTH + CHECKSUM + END
        public const int MIN_PACKET_SIZE = HEADER_SIZE + FOOTER_SIZE;
    }

    /// <summary>
    /// Command types for packet communication
    /// </summary>
    public enum CommandType : byte
    {
        None = 0x00,
        Data = 0x01,
        Ack = 0x02,
        Nack = 0x03,
        Request = 0x04,
        Response = 0x05
    }

    /// <summary>
    /// Structure to hold packet data
    /// </summary>
    public class PacketData
    {
        public byte Mode1 { get; set; }
        public byte Mode2 { get; set; }
        public byte Command { get; set; }
        public byte Sequence { get; set; }
        public byte[] Value { get; set; }
        public int ValueSize => Value?.Length ?? 0;

        public override string ToString()
        {
            return $"Mode1: {Mode1:X2}, Mode2: {Mode2:X2}, Command: {Command:X2}, Sequence: {Sequence}, Value: {BitConverter.ToString(Value)}";
        }
    }

    public class PacketDataEventArgs : EventArgs
    {
        public PacketData PacketData { get; }
        public DateTime Timestamp { get; }

        public PacketDataEventArgs(PacketData packetData)
        {
            PacketData = packetData;
            Timestamp = DateTime.Now;
        }
    }

    public class SerialErrorEventArgs : EventArgs
    {
        public Exception Error { get; }
        public DateTime Timestamp { get; }

        public SerialErrorEventArgs(Exception error)
        {
            Error = error;
            Timestamp = DateTime.Now;
        }
    }

    public class EnhancedPacketHandler
    {
        // Event for notifying serial port errors
        public event EventHandler<SerialErrorEventArgs> OnSerialError;

        // Connection recovery settings
        private const int MAX_RECOVERY_ATTEMPTS = 3;
        private const int RECOVERY_DELAY_MS = 1000;
        private int recoveryAttempts = 0;

        private readonly object serialLock = new object();
        private bool isDisposed = false;
        private byte[] buffer;
        private int writeIndex;
        private bool isReceiving;
        private byte sequenceNumber;
        private SerialPort serialPort;
        public event EventHandler<PacketDataEventArgs> OnPacketReceived;

        public EnhancedPacketHandler()
        {
            buffer = new byte[PacketConstants.MAX_PACKET_SIZE];
            writeIndex = 0;
            isReceiving = false;
            sequenceNumber = 0;
        }

        /// <summary>
        /// Initialize serial communication with specified port name and baud rate
        /// </summary>
        public void Begin(string portName, int baudRate)
        {

            // if serial port is already open, close it
            if (serialPort != null)
            {
                serialPort.Close();
                serialPort.Dispose();
            }

            // Initialize serial port
            serialPort = new SerialPort(portName, baudRate)
            {
                DtrEnable = true,
                RtsEnable = true
            };
            buffer = new byte[PacketConstants.MAX_PACKET_SIZE];
            writeIndex = 0;
            isReceiving = false;
            sequenceNumber = 0;

            serialPort.Open();
            serialPort.DataReceived += SerialPort_DataReceived;
        }

        /// <summary>
        /// Calculate checksum of data
        /// </summary>
        private byte CalculateChecksum(byte[] data, int length)
        {
            byte checksum = 0;
            for (int i = 0; i < length; i++)
            {
                checksum ^= data[i];
            }
            return checksum;
        }

        /// <summary>
        /// Reset receiver state
        /// </summary>
        private void ResetReceiver()
        {
            isReceiving = false;
            writeIndex = 0;
        }

        /// <summary>
        /// Validate received packet
        /// </summary>
        private bool ValidatePacket(int size)
        {
            // Check END marker
            if (buffer[writeIndex - 1] != PacketConstants.END_BYTE)
                return false;

            // Verify length check
            if (buffer[writeIndex - 3] != (size & 0xFF))
                return false;

            // Verify checksum
            byte receivedChecksum = buffer[writeIndex - 2];
            byte calculatedChecksum = CalculateChecksum(buffer, writeIndex - 2);

            return receivedChecksum == calculatedChecksum;
        }

        /// <summary>
        /// Send packet data
        /// </summary>
        public bool SendPacket(PacketData packet)
        {
            if (packet.ValueSize + PacketConstants.MIN_PACKET_SIZE > PacketConstants.MAX_PACKET_SIZE)
                return false;

            int currentIndex = 0;

            // Header
            buffer[currentIndex++] = PacketConstants.START_BYTE;
            buffer[currentIndex++] = (byte)((packet.ValueSize >> 8) & 0xFF); // Size high byte
            buffer[currentIndex++] = (byte)(packet.ValueSize & 0xFF);        // Size low byte
            buffer[currentIndex++] = packet.Mode1;
            buffer[currentIndex++] = packet.Mode2;
            buffer[currentIndex++] = packet.Command;
            buffer[currentIndex++] = sequenceNumber++; // Auto-increment sequence

            // Payload
            if (packet.Value != null && packet.ValueSize > 0)
            {
                Array.Copy(packet.Value, 0, buffer, currentIndex, packet.ValueSize);
                currentIndex += packet.ValueSize;
            }

            // Footer
            buffer[currentIndex++] = (byte)(packet.ValueSize & 0xFF); // Length check
            byte checksum = CalculateChecksum(buffer, currentIndex);
            buffer[currentIndex++] = checksum;
            buffer[currentIndex++] = PacketConstants.END_BYTE;

            try
            {
                if (serialPort?.IsOpen == true)
                {
                    serialPort.Write(buffer, 0, currentIndex);
                    return true;
                }
            }
            catch (Exception ex)
            {
                // Log the error
                LogSerialError(ex);

                // Try to recover the connection if possible
                TryRecoverConnection();

                // Notify any observers of the error
                OnSerialError?.Invoke(this, new SerialErrorEventArgs(ex));

                return false;
            }
            return false;
        }

        /// <summary>
        /// Handle serial port data received event
        /// </summary>
        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (serialPort == null) return;
            try
            {
                while (serialPort.BytesToRead > 0)
                {
                    byte inByte = (byte)serialPort.ReadByte();
                    ProcessByte(inByte);
                }
            }
            catch (Exception)
            {
                ResetReceiver();
            }
        }

        /// <summary>
        /// Process received byte
        /// </summary>
        private void ProcessByte(byte inByte)
        {
            if (!isReceiving)
            {
                if (inByte == PacketConstants.START_BYTE)
                {
                    isReceiving = true;
                    writeIndex = 0;
                    buffer[writeIndex++] = inByte;
                }
            }
            else
            {
                buffer[writeIndex++] = inByte;

                // Check if we have the size bytes
                if (writeIndex >= 3)
                {
                    int expectedSize = (buffer[1] << 8) | buffer[2];
                    int totalSize = expectedSize + PacketConstants.MIN_PACKET_SIZE;

                    // Validate expected size
                    if (totalSize > PacketConstants.MAX_PACKET_SIZE)
                    {
                        ResetReceiver();
                        return;
                    }

                    // Check if we have received the complete packet
                    if (writeIndex == totalSize)
                    {
                        if (!ValidatePacket(expectedSize))
                        {
                            ResetReceiver();
                            return;
                        }

                        // Extract packet data
                        var packet = new PacketData
                        {
                            Mode1 = buffer[3],
                            Mode2 = buffer[4],
                            Command = buffer[5],
                            Sequence = buffer[6],
                            Value = new byte[expectedSize]
                        };

                        // Copy payload
                        Array.Copy(buffer, 7, packet.Value, 0, expectedSize);

                        // Raise event if there are any subscribers
                        OnPacketReceived?.Invoke(this, new PacketDataEventArgs(packet));

                        ResetReceiver();
                    }
                }
            }
        }

        /// <summary>
        /// Close serial port connection
        /// </summary>
        private void LogSerialError(Exception ex)
        {
            var errorMessage = $"Serial Port Error: {ex.Message}";
            if (ex is TimeoutException)
            {
                errorMessage = $"Serial Port Timeout: {ex.Message}";
            }
            else if (ex is InvalidOperationException)
            {
                errorMessage = $"Serial Port Invalid Operation: {ex.Message}";
            }
            else if (ex is UnauthorizedAccessException)
            {
                errorMessage = $"Serial Port Access Denied: {ex.Message}";
            }

            // TODO: Implement actual logging mechanism (e.g., to file or logging service)
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {errorMessage}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
        }

        private bool TryRecoverConnection()
        {
            if (recoveryAttempts >= MAX_RECOVERY_ATTEMPTS)
            {
                Console.WriteLine("Max recovery attempts reached. Manual intervention required.");
                return false;
            }

            lock (serialLock)
            {
                try
                {
                    recoveryAttempts++;
                    Console.WriteLine($"Attempting to recover connection (Attempt {recoveryAttempts}/{MAX_RECOVERY_ATTEMPTS})");

                    // Close the existing connection if it's open
                    if (serialPort?.IsOpen == true)
                    {
                        serialPort.Close();
                    }

                    // Wait before attempting to reconnect
                    Thread.Sleep(RECOVERY_DELAY_MS);

                    // Attempt to reopen the connection
                    if (serialPort != null && !serialPort.IsOpen)
                    {
                        serialPort.Open();
                        if (serialPort.IsOpen)
                        {
                            Console.WriteLine("Connection recovered successfully");
                            recoveryAttempts = 0;
                            return true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogSerialError(ex);
                }
            }

            return false;
        }

        public void Close()
        {
            if (serialPort == null) return;

            var port = serialPort;
            serialPort = null; // ยกเลิกการอ้างอิงก่อนเพื่อป้องกัน event handler

            try
            {
                if (port.IsOpen)
                {
                    port.DataReceived -= SerialPort_DataReceived;
                    Thread.Sleep(100); // รอให้ event handler ปัจจุบันทำงานเสร็จ
                    port.Close();
                }
                port.Dispose();
            }
            catch (Exception ex)
            {
                LogSerialError(ex);
                OnSerialError?.Invoke(this, new SerialErrorEventArgs(ex));
            }
            finally
            {
                ResetReceiver();
                buffer = null;
            }
        }
    }
}