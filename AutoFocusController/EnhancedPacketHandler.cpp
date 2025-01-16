#include "EnhancedPacketHandler.h"

EnhancedPacketHandler::EnhancedPacketHandler() {
  writeIndex = 0;
  isReceiving = false;
  sequenceNumber = 0;
  packetCallback = nullptr;
}

void EnhancedPacketHandler::begin(unsigned long baudRate) {
  Serial.begin(baudRate);
  serialPort = &Serial;
}

void EnhancedPacketHandler::begin(HardwareSerial &serial, unsigned long baudRate) {
  serial.begin(baudRate);
  serialPort = &serial;
}

void EnhancedPacketHandler::setSerial(Stream &serial) {
  serialPort = &serial;
}

void EnhancedPacketHandler::setCallback(void (*callback)(PacketData &)) {
  packetCallback = callback;
}

uint8_t EnhancedPacketHandler::calculateChecksum(uint8_t *data, uint16_t length) {
  uint8_t checksum = 0;
  for (uint16_t i = 0; i < length; i++) {
    checksum ^= data[i];
  }
  return checksum;
}

void EnhancedPacketHandler::resetReceiver() {
  isReceiving = false;
  writeIndex = 0;
}

bool EnhancedPacketHandler::validatePacket(uint16_t size) {
  // Check END marker
  if (buffer[writeIndex - 1] != END_BYTE) {
    return false;
  }

  // Verify length check
  if (buffer[writeIndex - 3] != (size & 0xFF)) {
    return false;
  }

  // Verify checksum
  uint8_t receivedChecksum = buffer[writeIndex - 2];
  uint8_t calculatedChecksum = calculateChecksum(buffer, writeIndex - 2);

  return (receivedChecksum == calculatedChecksum);
}

bool EnhancedPacketHandler::sendPacket(PacketData &packet) {
  // Validate packet size
  if (packet.valueSize + MIN_PACKET_SIZE > MAX_PACKET_SIZE) {
    return false;
  }

  uint16_t currentIndex = 0;

  // Header
  buffer[currentIndex++] = START_BYTE;
  buffer[currentIndex++] = (packet.valueSize >> 8) & 0xFF;  // Size high byte
  buffer[currentIndex++] = packet.valueSize & 0xFF;         // Size low byte
  buffer[currentIndex++] = packet.mode1;
  buffer[currentIndex++] = packet.mode2;
  buffer[currentIndex++] = packet.command;
  buffer[currentIndex++] = sequenceNumber++;  // Auto-increment sequence

  // Payload
  memcpy(&buffer[currentIndex], packet.value, packet.valueSize);
  currentIndex += packet.valueSize;

  // Footer
  buffer[currentIndex++] = packet.valueSize & 0xFF;  // Length check
  // Calculate checksum before incrementing index
  uint8_t checksum = calculateChecksum(buffer, currentIndex);
  buffer[currentIndex++] = checksum;
  buffer[currentIndex++] = END_BYTE;

  // Send complete packet
  if (serialPort) {
    serialPort->write(buffer, currentIndex);
    return true;
  }
}

bool EnhancedPacketHandler::receivePacket(PacketData &packet) {
  while (serialPort && serialPort->available()) {
    uint8_t inByte = serialPort->read();

    if (!isReceiving) {
      if (inByte == START_BYTE) {
        isReceiving = true;
        writeIndex = 0;
        buffer[writeIndex++] = inByte;
      }
    } else {
      buffer[writeIndex++] = inByte;

      // Check if we have the size bytes
      if (writeIndex >= 3) {
        uint16_t expectedSize = (buffer[1] << 8) | buffer[2];
        uint16_t totalSize = expectedSize + MIN_PACKET_SIZE;

        // Validate expected size
        if (totalSize > MAX_PACKET_SIZE) {
          resetReceiver();
          return false;
        }

        // Check if we have received the complete packet
        if (writeIndex == totalSize) {
          if (!validatePacket(expectedSize)) {
            resetReceiver();
            return false;
          }

          // Extract packet data
          packet.mode1 = buffer[3];
          packet.mode2 = buffer[4];
          packet.command = buffer[5];
          packet.sequence = buffer[6];
          packet.valueSize = expectedSize;
          packet.value = &buffer[7];

          // Call callback if registered
          if (packetCallback != nullptr) {
            packetCallback(packet);
          }

          resetReceiver();
          return true;
        }
      }
    }
  }
  return false;
}