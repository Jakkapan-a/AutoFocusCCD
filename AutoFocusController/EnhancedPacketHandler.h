#ifndef ENHANCED_PACKET_H
#define ENHANCED_PACKET_H

#include <Arduino.h>

/*
 * Enhanced Packet Protocol Structure:
 * -----------------------------------------------------
 * |Byte| Description        | Size  | Value            |
 * |----+-------------------+-------+------------------|
 * | 1  | START marker      | 1     | 0xAA            |
 * | 2-3| Packet Size       | 2     | High + Low byte |
 * | 4  | MODE1             | 1     | User defined    |
 * | 5  | MODE2             | 1     | User defined    |
 * | 6  | Command (CMD)     | 1     | Command type    |
 * | 7  | Sequence (SEQ)    | 1     | 0-255 cycle    |
 * | 8+ | Payload (Value)   | N     | User data      |
 * | -3 | Length Check      | 1     | Payload length |
 * | -2 | Checksum         | 1     | XOR of all bytes|
 * | -1 | END marker       | 1     | 0xFF           |
 * -----------------------------------------------------
 */


// Protocol constants
#define START_BYTE 0xAA
#define END_BYTE 0xFF
#define MAX_PACKET_SIZE 512
#define HEADER_SIZE 7     // START + SIZE(2) + MODE1 + MODE2 + CMD + SEQ
#define FOOTER_SIZE 3     // LENGTH + CHECKSUM + END
#define MIN_PACKET_SIZE (HEADER_SIZE + FOOTER_SIZE)

// Command types
enum CommandType {
    CMD_NONE = 0x00,
    CMD_DATA = 0x01,
    CMD_ACK = 0x02,
    CMD_NACK = 0x03,
    CMD_REQUEST = 0x04,
    CMD_RESPONSE = 0x05
};

enum Mode1Type {
    MODE1_NONE = 0x00,
    MODE1_SEND = 0x01,
    MODE1_RESPONSE = 0x02,
    MODE1_ERROR_MODE = 0x03
};

enum Mode2Type {
    MODE2_NONE = 0x00,
    MODE2_INIT_LOADED = 0x01,
    MODE2_INA219 = 0x02,
    MODE2_RELAY = 0x03,
    MODE2_UNUSED = 0x04,
    MODE2_KEYBOARD = 0x05,
    MODE2_SENSOR_STD = 0x06,
    MODE2_LED_RED = 0x07,
    MODE2_LED_GREEN = 0x08,
    MODE2_LED_BLUE = 0x09,
    MODE2_LED_OFF_ALL = 0x10,
    MODE2_6V_NOT_PWM = 0x11,
    MODE2_4V6_PWM = 0x12,
    MODE2_OFF_PWM_AND_NOT = 0x13
};

class EnhancedPacketHandler {
public:
    // Structure to hold packet data
    struct PacketData {
        uint8_t mode1;
        uint8_t mode2;
        uint8_t command;
        uint8_t sequence;
        uint8_t* value;
        uint16_t valueSize;
    };

    EnhancedPacketHandler();  // Constructor
    
    // Public methods
    bool sendPacket(PacketData& packet);
    bool receivePacket(PacketData& packet);
    
    // Utility methods
    void begin(unsigned long baudRate);  // Default Serial
    void begin(HardwareSerial &serial, unsigned long baudRate);  // Hardware Serial
    void setSerial(Stream &serial);  // Set any Stream object
    Stream* getSerial() { return serialPort; }
    void setCallback(void (*callback)(PacketData&));

private:
    uint8_t buffer[MAX_PACKET_SIZE];
    uint16_t writeIndex;
    bool isReceiving;
    uint8_t sequenceNumber;
    void (*packetCallback)(PacketData&);
    Stream* serialPort;

    // Private utility methods
    uint8_t calculateChecksum(uint8_t* data, uint16_t length);
    bool validatePacket(uint16_t size);
    void resetReceiver();
};

#endif // ENHANCED_PACKET_H