using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoFocusCCD.Utilities
{
    public class DeviceControl
    {
        private readonly EnhancedPacketHandler enhancedPacketHandler;
        public enum Mode1Type : byte
        {
            None = 0x00,
            Send = 0x01,
            Response = 0x02,
            Error = 0x03
        }
        public enum Mode2Type : byte
        {
            None = 0x00,
            Init = 0x01,
            Ina219 = 0x02,
            Relay = 0x03,
            LED = 0x04,
            Keyboard = 0x05,
            Sensor = 0x06,
            LED_RED = 0x07,
            LED_GREEN = 0x08,
            LED_BLUE = 0x09,
            LED_OFF_ALL = 0x10,
            RELAY_6V_NOT_PWM = 0x11,
            RELAY_4V6_PWM = 0x12,
            RELAY_OFF_ALL = 0x13
        }


        public DeviceControl(EnhancedPacketHandler handler)
        {
            this.enhancedPacketHandler = handler;
        }

        public bool SetLED(Mode2Type ledType, bool turnOn)
        {
            if (ledType != Mode2Type.LED_RED &&
                ledType != Mode2Type.LED_GREEN &&
                ledType != Mode2Type.LED_BLUE)
            {
                Console.WriteLine("Invalid LED type");
                return false;
            }

            var packet = new PacketData
            {
                Mode1 = 0x01,
                Mode2 = (byte)ledType,
                Command = (byte)CommandType.Data,
                Value = new byte[] { (byte)(turnOn ? 0x01 : 0x00) }
            };

            if (enhancedPacketHandler?.SendPacket(packet) == true)
            {
                Console.WriteLine($"LED {ledType} set to {(turnOn ? "ON" : "OFF")}");
                return true;
            }
            return false;
        }

        public bool TurnOffAllLEDs()
        {
            var packet = new PacketData
            {
                Mode1 = 0x01,
                Mode2 = (byte)Mode2Type.LED_OFF_ALL,
                Command = (byte)CommandType.Data,
                Value = new byte[] { 0x00 }
            };

            if (enhancedPacketHandler?.SendPacket(packet) == true)
            {
                Console.WriteLine("All LEDs turned OFF");
                return true;
            }
            return false;
        }

        public bool SetRelay(Mode2Type relayType, bool turnOn)
        {
            if (relayType != Mode2Type.RELAY_6V_NOT_PWM &&
                relayType != Mode2Type.RELAY_4V6_PWM)
            {
                Console.WriteLine("Invalid Relay type");
                return false;
            }

            var packet = new PacketData
            {
                Mode1 = 0x01,
                Mode2 = (byte)relayType,
                Command = (byte)CommandType.Data,
                Value = new byte[] { (byte)(turnOn ? 0x01 : 0x00) }
            };

            if (enhancedPacketHandler?.SendPacket(packet) == true)
            {
                Console.WriteLine($"Relay {relayType} set to {(turnOn ? "ON" : "OFF")}");
                return true;
            }
            return false;
        }

        public bool TurnOffAllRelays()
        {
            var packet = new PacketData
            {
                Mode1 = 0x01,
                Mode2 = (byte)Mode2Type.RELAY_OFF_ALL,
                Command = (byte)CommandType.Data,
                Value = new byte[] { 0x00 }
            };

            if (enhancedPacketHandler?.SendPacket(packet) == true)
            {
                Console.WriteLine("All Relays turned OFF");
                return true;
            }
            return false;
        }

        public bool Command(byte mode1, byte mode2, CommandType command, byte[] value = null)
        {
            var packet = new PacketData
            {
                Mode1 = mode1,
                Mode2 = mode2,
                Command = (byte)command,
                Value = value ?? new byte[] { 0x00 }
            };

            if (enhancedPacketHandler?.SendPacket(packet) == true)
            {
                Console.WriteLine($"Command {command} sent");
                return true;
            }
            return false;
        }

        public bool SendAscci(string text)
        {
            var packet = new PacketData
            {
                Mode1 = 0x01,
                Mode2 = 0x00,
                Command = (byte)CommandType.Data,
                Value = Encoding.ASCII.GetBytes(text)
            };
            if (enhancedPacketHandler?.SendPacket(packet) == true)
            {
                Console.WriteLine($"Text '{text}' sent");
                return true;
            }
            return false;
        }
    }
}
