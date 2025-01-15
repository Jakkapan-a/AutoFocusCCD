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