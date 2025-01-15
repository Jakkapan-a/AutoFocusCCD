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
EnhancedPacketHandler packetHandler;

void handlePacket(EnhancedPacketHandler::PacketData& packet) {
  // Process received packet based on command type
  switch (packet.command) {
    case CMD_DATA:
      // Handle data command
      Serial.println("Data packet received");
      // Process packet.value with length packet.valueSize
      break;

    case CMD_REQUEST:
      // Handle request command
      Serial.println("Request received");
      // Send response packet
      uint8_t responseData[] = { 0x01, 0x02, 0x03 };
      EnhancedPacketHandler::PacketData response = {
        .mode1 = packet.mode1,
        .mode2 = packet.mode2,
        .command = CMD_RESPONSE,
        .sequence = 0,  // Will be auto-incremented
        .value = responseData,
        .valueSize = sizeof(responseData)
      };
      packetHandler.sendPacket(response);
      break;

    default:
      Serial.println("Unknown command received");
      break;
  }
}


#define BUFFER_SIZE_DATA 128
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
// -------------------- SERIAL 0 -------------------- //
bool startReceived0 = false;
bool endReceived0 = false;
const char startChar0 = '$';
const char endChar0 = '#';
char inputString0[BUFFER_SIZE_DATA];
int inputStringLength0 = 0;

uint32_t lastTime = 0;
// uint32_t currentTime = 0;
// void _serialEvent() {
//   while (Serial.available()) {
//     byte inChar = (byte)Serial.read();
//     if (inChar == startChar0) {
//       startReceived0 = true;
//       inputStringLength0 = 0;
//       memset(inputString0, 0, BUFFER_SIZE_DATA);
//     } else if (startReceived0 && inChar == endChar0) {
//       endReceived0 = true;
//     } else if (startReceived0) {
//       if (inputStringLength0 < BUFFER_SIZE_DATA - 1) {
//         inputString0[inputStringLength0++] = inChar;
//       } else {
//         startReceived0 = false;
//         endReceived0 = false;
//         inputStringLength0 = 0;
//       }
//     }
//   }
// }

// -------------------- SERIAL 1 -------------------- //
bool startReceived1 = false;
bool endReceived1 = false;
const char startChar1 = '$';
const char endChar1 = '#';
char inputString1[BUFFER_SIZE_DATA];
int inputStringLength1 = 0;
// void serialEvent1() {
//   while (Serial1.available()) {
//     byte inChar = (byte)Serial1.read();
//     if (inChar == startChar1) {
//       startReceived1 = true;
//       inputStringLength1 = 0;
//       memset(inputString1, 0, BUFFER_SIZE_DATA);
//     } else if (startReceived1 && inChar == endChar1) {
//       endReceived1 = true;
//     } else if (startReceived1) {
//       if (inputStringLength1 < BUFFER_SIZE_DATA - 1) {
//         inputString1[inputStringLength1++] = inChar;
//       } else {
//         startReceived1 = false;
//         endReceived1 = false;
//         inputStringLength1 = 0;
//       }
//     }
//   }
// }

void setup() {
  Serial.begin(9600);
  // Serial1.begin(9600);
  Serial.println("$START:OK#");
  // Serial1.println("$START:OK#");
    packetHandler.begin(Serial1, 9600);
  packetHandler.setCallback(handlePacket);

  // Sensor
  sensor.OnEventChange(sensorEvent);

  // Keyboard
  Keyboard.begin();
  // Clear buffer
  memset(inputString1, 0, BUFFER_SIZE_DATA);

  buzzerPass.setTime(200);
  buzzerPass.total = 2;

  LED_RED.off();
  LED_GREEN.off();
  LED_BLUE.off();

  RELAY1_NOT.off();
  RELAY2_PVM.off();
  RELAY3_PVM.off();

  if (!ina219.begin()) {
    Serial.println("Failed to find INA219 chip");
    // Serial1.println("$INA219:FAIL#");
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
      // Serial1.println("$INA219:FAIL#");
      if(packetHandler.sendPacket(sendPacket)) {
        Serial.println("Packet sent");
      }
      delay(1000);
    }
  }
}

void loop() {
  sensor.update();
  buzzerPass.update();
  // _serialEvent();
  // manageSerial0();
  // manageSerial1();

  uint32_t currentTime = millis();
  if (currentTime - lastTime > 1000) {
    // secondTick();

    uint8_t data[] = { 0x01, 0x02, 0x03, 0x04 };
    EnhancedPacketHandler::PacketData sendPacket = {
      .mode1 = 0x01,
      .mode2 = 0x02,
      .command = CMD_DATA,
      .sequence = 0,  // Will be auto-incremented
      .value = data,
      .valueSize = sizeof(data)
    };

    if(packetHandler.sendPacket(sendPacket)) {
      Serial.println("Packet sent");
    }

    lastTime = currentTime;
  } else if (currentTime < lastTime) {
    lastTime = currentTime;  // Overflow
  }

    EnhancedPacketHandler::PacketData receivedPacket;
    if(packetHandler.receivePacket(receivedPacket)) {
        // Packet will be handled in callback function
        Serial.println("Packet received");
    }



  if (RESULT == 2) {
    buzzerPass.total = 4;
  }
}
void secondTick() {
  float busvoltage = ina219.getBusVoltage_V();
  float current_mA = ina219.getCurrent_mA();
  Serial.print("Bus Voltage:   ");
  Serial.print(busvoltage);
  Serial.println(" V");
  Serial.print("Current:       ");
  Serial.print(current_mA);
  Serial.println(" mA");

  Serial1.print("$INA_DATA:");
  Serial1.print(busvoltage);
  Serial1.print(",");
  Serial1.print(current_mA);
  Serial1.println("#");
}

void manageSerial0() {
  if (startReceived0 && endReceived0) {
    startReceived0 = false;
    endReceived0 = false;
    inputString0[inputStringLength0] = '\0';
    Serial.println(inputString0);
    parseData(inputString0);
    memset(inputString0, 0, BUFFER_SIZE_DATA);
    inputStringLength0 = 0;
  }
}
void manageSerial1() {
  if (startReceived1 && endReceived1) {
    startReceived1 = false;
    endReceived1 = false;
    inputString1[inputStringLength1] = '\0';
    Serial.println(inputString1);
    parseData(inputString1);
    memset(inputString1, 0, BUFFER_SIZE_DATA);
    inputStringLength1 = 0;
  }
}

void parseData(String dataInput) {

  dataInput.trim();
  if (dataInput.indexOf("CONN:") != -1) {
    // Send dataInput to MES
    // String extract = extractdataInput(dataInput, "STATUS_SERVER:");

    // Serial.println("$RECEIVED:OK#");
    Serial1.println("$RECEIVED:OK#");
  } else if (dataInput.indexOf("KBD:") != -1) {
    String serialData = extractData(dataInput, "KBD:");
    if (serialData.length() > 0) {
      for (int i = 0; i < serialData.length(); i++) {
        Keyboard.print(serialData[i]);
      }
      Keyboard.press(KEY_RETURN);
      Keyboard.releaseAll();
    }

    buzzerPass.setTime(200);
    // if (dataInput.indexOf("KBD:clear") == -1)
    // {
    //   buzzerPass.total = 2;
    // }
  } else if (dataInput.indexOf("CMD:") != -1) {
    String serialData = extractData(dataInput, "CMD:");
    Serial1.print("$");
    Serial1.print(serialData);
    Serial1.println("#");
  } else if (dataInput.indexOf("RAY:") != -1) {
    String serialData = extractData(dataInput, "RAY:");
    if (serialData.indexOf("RST") != -1) {
      RELAY1_NOT.off();
      RELAY2_PVM.off();
      RELAY3_PVM.off();
      RESULT = 0;
      buzzerPass.total = 0;
    } else if (serialData.indexOf("NOT1") != -1) {
      RELAY1_NOT.on();
      RELAY2_PVM.off();
      RELAY3_PVM.off();
    } else if (serialData.indexOf("NOT0") != -1) {
      RELAY1_NOT.off();
      RELAY2_PVM.off();
      RELAY3_PVM.off();
    } else if (serialData.indexOf("PVM1") != -1) {
      RELAY1_NOT.off();
      RELAY2_PVM.on();
      RELAY3_PVM.off();
    } else if (serialData.indexOf("PVM0") != -1) {
      RELAY1_NOT.off();
      RELAY2_PVM.off();
      RELAY3_PVM.off();
    } else {
      RELAY1_NOT.off();
      RELAY2_PVM.off();
      RELAY3_PVM.off();
    }
  } else if (dataInput.indexOf("LED:") != -1) {
    String serialData = extractData(dataInput, "LED:");

    if (serialData.indexOf("R") != -1) {
      LED_RED.on();
      LED_GREEN.off();
      LED_BLUE.off();
      RESULT = 2;
    } else if (serialData.indexOf("G") != -1) {
      LED_RED.off();
      LED_GREEN.on();
      LED_BLUE.off();
      RESULT = 1;
      buzzerPass.total = 2;
    } else if (serialData.indexOf("B") != -1) {
      LED_RED.off();
      LED_GREEN.off();
      LED_BLUE.on();
      RESULT = 1;
      // buzzerPass.total = 1;
    } else {
      LED_RED.off();
      LED_GREEN.off();
      LED_BLUE.off();
      RESULT = 0;
    }
  } else if (dataInput.indexOf("BZ:") != -1) {
    String serialData = extractData(dataInput, "BZ:");
    if (serialData == "1") {
      buzzerPass.total = 1;
    } else if (serialData == "2" || serialData == "PASS") {
      buzzerPass.total = 2;
    } else if (serialData == "3") {
      buzzerPass.total = 3;
    } else if (serialData == "4" || serialData == "FAIL" || serialData == "NG") {
      buzzerPass.total = 4;
      RESULT = 2;
    } else {
      buzzerPass.total = 0;
    }
  }
}

String extractData(String dataInput, String key) {
  int keyIndex = dataInput.indexOf(key);  // Find the position of the key
  if (keyIndex == -1) {
    return "";  // Return 0 if key not found
  }

  int startIndex = keyIndex + key.length();           // Start index for the number
  int endIndex = dataInput.indexOf(",", startIndex);  // Find the next comma after the key
  if (endIndex == -1) {
    endIndex = dataInput.length();  // If no comma, assume end of string
  }
  String valueStr = dataInput.substring(startIndex, endIndex);  // Extract the substring
  return valueStr;                                              // Return the extracted string
}
void sensorEvent(bool state) {
  if (state) {
    Serial.println("SENSOR1:ON");
    Serial1.println("$SENSOR1:ON#");
  } else {
    Serial.println("SENSOR1:OFF");
    Serial1.println("$SENSOR1:OFF#");
  }
}