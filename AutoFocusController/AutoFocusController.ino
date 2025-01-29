/**
 * @file AutoFocusController.ino
 * @author Jakkapan
 * @brief
 * @version 0.1
 * @date 2024-05-15
 *
 * @copyright Copyright (c) 2024
 * Board: Leonardo
 */

#include <TcBUTTON.h>
#include <TcPINOUT.h>
#include "Keyboard.h"
#include "TcBUZZER.h"
#include <Wire.h>
#include <Adafruit_INA219.h>
#include "EnhancedPacketHandler.h"

//*********************** INPUT Sensor ***********************//
/*
 * Up to 4 boards may be connected. Addressing is as follows:
 * Board 0: Address = 0x40 Offset = binary 00000 (no jumpers required)
 * Board 1: Address = 0x41 Offset = binary 00001 (bridge A0 as in the photo above)
 * Board 2: Address = 0x44 Offset = binary 00100 (bridge A1)
 * Board 3: Address = 0x45 Offset = binary 00101 (bridge A0 & A1)
 */
Adafruit_INA219 ina219(0x40);

/*
 * Enhanced Packet Protocol Structure:
 * -----------------------------------------------------
 * |Byte| Description       | Size  | Value            |
 * |----+-------------------+-------+------------------|
 * | 1  | START marker      | 1     | 0xAA            |
 * | 2-3| Packet Size       | 2     | High + Low byte |
 * | 4  | MODE1             | 1     | User defined    |
 * | 5  | MODE2             | 1     | User defined    |
 * | 6  | Command (CMD)     | 1     | Command type    |
 * | 7  | Sequence (SEQ)    | 1     | 0-255 cycle     |
 * | 8+ | Payload (Value)   | N     | User data       |
 * | -3 | Length Check      | 1     | Payload length  |
 * | -2 | Checksum          | 1     | XOR of all bytes|
 * | -1 | END marker        | 1     | 0xFF            |
 * -----------------------------------------------------
 */
EnhancedPacketHandler packetHandler;



#define BUFFER_SIZE_DATA 50
// -------------------- INPUT ----------------------- //
#define SENSOR_PIN 10
void sensorEvent(bool state);
TcBUTTON sensor(SENSOR_PIN);

#define BUZZER_PIN 6
TcBUZZER buzzerPass(BUZZER_PIN, true);

// -------------------- OUTPUT ----------------------- //
#define LED_RED_PIN 11
TcPINOUT LED_RED(LED_RED_PIN);

#define LED_GREEN_PIN 4
TcPINOUT LED_GREEN(LED_GREEN_PIN);

#define LED_BLUE_PIN 5
TcPINOUT LED_BLUE(LED_BLUE_PIN);

#define RELAY1_NOT_PIN 7                     //
TcPINOUT RELAY1_NOT(RELAY1_NOT_PIN, false);  // 6v control for out of STEP down

#define RELAY2_PVM_PIN 8                     //
TcPINOUT RELAY2_PVM(RELAY2_PVM_PIN, false);  // 4.6v control for out of ECU

#define RELAY3_PVM_PIN 9  //
TcPINOUT RELAY3_PVM(RELAY3_PVM_PIN, false);

uint8_t RESULT = 0;
uint32_t lastTime = 0;
bool isSensorOn = false;

void setup() {
  Serial.begin(9600);
  packetHandler.begin(Serial1, 9600);
  packetHandler.setCallback(handlePacket);
  // Sensor
  sensor.OnEventChange(sensorEvent);
  // Keyboard
  Keyboard.begin();
  // Buzzer
  buzzerPass.setTime(200);
  buzzerPass.total = 2;

  // LED
  LED_RED.off();
  LED_GREEN.off();
  LED_BLUE.off();
  // RELAY
  RELAY1_NOT.off();
  RELAY2_PVM.off();
  RELAY3_PVM.off();
  // INA219
  if (!ina219.begin()) {
    Serial.println("Failed to find INA219 chip");
    uint8_t data[] = { 0x00, 0x01, 0x01, 0x01 };
    EnhancedPacketHandler::PacketData sendPacket = {
      .mode1 = 0x03,
      .mode2 = 0x02,
      .command = CMD_DATA,
      .sequence = 0,  // Will be auto-incremented
      .value = data,
      .valueSize = sizeof(data)
    };
    while (!ina219.begin()) {
      if (packetHandler.sendPacket(sendPacket)) {
        Serial.println("Packet sent");
      }
      delay(1000);
    }
  }
  buzzerPass.total = 2;
}

void loop() {
  sensor.update();
  buzzerPass.update();

  uint32_t currentTime = millis();

  if (currentTime - lastTime > 1000) {
    secondTick();
    lastTime = currentTime;
  } else if (currentTime < lastTime) {
    lastTime = currentTime;  // Overflow
  }

  EnhancedPacketHandler::PacketData receivedPacket;
  if (packetHandler.receivePacket(receivedPacket)) {
    // ... more code 
  }
  if (RESULT == 2) {
    buzzerPass.total = 4;
  }
}

void turnOffAllLEDs() {
    LED_RED.off();
    LED_GREEN.off();
    LED_BLUE.off();
}

void turnOffAllRelays() {
    RELAY1_NOT.off();
    RELAY2_PVM.off();
    RELAY3_PVM.off();
}

void turnOffAll() {
    turnOffAllLEDs();
    turnOffAllRelays();
}

void handleLED(byte value, TcPINOUT& ledToTurn, TcPINOUT& other1, TcPINOUT& other2, byte resultCode = 0) {
    if (value == 0x01) {
        ledToTurn.on();
        other1.off();
        other2.off();
        RESULT = resultCode;
    } else {
        ledToTurn.off();
        other1.off();
        other2.off();
        RESULT = 0;
    }
}

void handleRelay(byte value, TcPINOUT& relayToTurn, TcPINOUT& other1, TcPINOUT& other2) {
    if (value == 0x01) {
        relayToTurn.on();
        other1.off();
        other2.off();
    } else {
        relayToTurn.off();
        other1.off();
        other2.off();
    }
}


void handlePacket(EnhancedPacketHandler::PacketData& packet) {
     if (packet.command != CMD_DATA) {
        switch (packet.command) {
            case CMD_REQUEST:
                Serial.println("Request received");
               if(packet.mode1 == 0x01 && packet.mode2 == 0x06) {
                  delay(10);
                  byte data[] = { 0x00 };
                  if (isSensorOn) {
                    data[0] = 0x01;
                  }
                  EnhancedPacketHandler::PacketData sendPacket = {
                    .mode1 = 0x01,
                    .mode2 = 0x06,
                    .command = CMD_RESPONSE,
                    .sequence = packet.sequence,  // Will be auto-incremented
                    .value = data,
                    .valueSize = 1
                  };

                  // Send response
                  if (packetHandler.sendPacket(sendPacket)) {
                    Serial.println("ACK sent");
                  }
               }
                break;
            case CMD_ACK:
                Serial.println("ACK received");
                break;
            default:
                Serial.println("Unknown command received");
                break;
        }
        return;
    }

    // check mode1
    if (packet.mode1 != MODE1_SEND) return;

    // handle mode2
    switch (packet.mode2) {
        case MODE2_KEYBOARD:
            handleKeyboard(packet);
            break;

        case MODE2_LED_RED:
            if(packet.value[0] == 0x01) {
                buzzerPass.total = 2;
                RESULT = 2;
            }

            handleLED(packet.value[0], LED_RED, LED_GREEN, LED_BLUE, 2);
            break;

        case MODE2_LED_GREEN:
            if (packet.value[0] == 0x01) {
                buzzerPass.total = 2;
                RESULT = 1;
            }
            
            handleLED(packet.value[0], LED_GREEN, LED_RED, LED_BLUE, 1);
            break;

        case MODE2_LED_BLUE:
            handleLED(packet.value[0], LED_BLUE, LED_RED, LED_GREEN, 1);
            break;

        case MODE2_LED_OFF_ALL:
            turnOffAllLEDs();
            RESULT = 0;
            break;

        case MODE2_6V_NOT_PWM:
            handleRelay(packet.value[0], RELAY1_NOT, RELAY3_PVM, RELAY3_PVM);
            // handleRelay(packet.value[0], RELAY2_PVM, RELAY3_PVM, RELAY3_PVM);
            break;

        case MODE2_4V6_PWM:
            handleRelay(packet.value[0], RELAY2_PVM, RELAY1_NOT, RELAY3_PVM);
            break;

        case MODE2_OFF_PWM_AND_NOT:
            RELAY1_NOT.off();
            RELAY2_PVM.off();
            RELAY3_PVM.off();
            break;
    }
}

void handleKeyboard(const EnhancedPacketHandler::PacketData& packet) {
    char textBuffer[256];
    memset(textBuffer, 0, sizeof(textBuffer));
    memcpy(textBuffer, packet.value, packet.valueSize);
    
    Serial.print("Received Text: ");
    Serial.println(textBuffer);
    
    // send text to keyboard
    for(int i = 0; textBuffer[i] != '\0'; i++) {
        Keyboard.print(textBuffer[i]);
    }
    
    // send return key
    Keyboard.press(KEY_RETURN);
    Keyboard.releaseAll();
}

struct SensorData {
  float busvoltage;
  float current_mA;
} __attribute__((packed));


void secondTick() {
  float busvoltage = ina219.getBusVoltage_V();
  float current_mA = ina219.getCurrent_mA();

  SensorData data = {
    .busvoltage = busvoltage,
    .current_mA = current_mA
  };

  EnhancedPacketHandler::PacketData sendPacket = {
    .mode1 = 0x01,
    .mode2 = 0x02,
    .command = CMD_DATA,
    .sequence = 0,             // Will be auto-incremented
    .value = (uint8_t*)&data,  // Cast to uint8_t pointer to send raw data
    .valueSize = sizeof(data)
  };

  if (packetHandler.sendPacket(sendPacket)) {
    Serial.print("==> ");
    Serial.print("Bus Voltage: ");
    Serial.print(busvoltage);
    Serial.print(" V");
    Serial.print("Current: ");
    Serial.print(current_mA);
    Serial.println(" mA");
  }
}

void sensorEvent(bool state) {
  uint8_t data[] = { 0x00 };
  if (state) {
    data[0] = 0x01;
  } else {
    data[0] = 0x00;
    // OFF ALL LED & RELAY & BUZZER
    // LED_RED.off();
    // LED_GREEN.off();
    // LED_BLUE.off();
    // RELAY1_NOT.off();
    // RELAY2_PVM.off();
    // RELAY3_PVM.off();
    // buzzerPass.total = 0;
  }
  
  isSensorOn = state;
  // mode 1 = 0x01 , mode 2 = 0x06
  EnhancedPacketHandler::PacketData sendPacket = {
    .mode1 = 0x01,
    .mode2 = 0x06,
    .command = CMD_DATA,
    .sequence = 0,  // Will be auto-incremented
    .value = data,
    .valueSize = sizeof(data)
  };

  if (packetHandler.sendPacket(sendPacket)) {
    Serial.print("Packet sent: ");
    Serial.println(state ? "ON" : "OFF");
  }
}

void ResponsePacket(EnhancedPacketHandler::PacketData& packet) {
  EnhancedPacketHandler::PacketData sendPacket = {
    .mode1 = 0x01,
    .mode2 = 0x02,
    .command = CMD_RESPONSE,
    .sequence = packet.sequence,  // Will be auto-incremented
    .value = packet.value,
    .valueSize = packet.valueSize
  };

  if (packetHandler.sendPacket(sendPacket)) {
    Serial.println("Packet sent");
  }
}