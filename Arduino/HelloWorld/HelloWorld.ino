/*
  Blink

  Turns an LED on for one second, then off for one second, repeatedly.

  Most Arduinos have an on-board LED you can control. On the UNO, MEGA and ZERO
  it is attached to digital pin 13, on MKR1000 on pin 6. LED_BUILTIN is set to
  the correct LED pin independent of which board is used.
  If you want to know what pin the on-board LED is connected to on your Arduino
  model, check the Technical Specs of your board at:
  https://www.arduino.cc/en/Main/Products

  modified 8 May 2014
  by Scott Fitzgerald
  modified 2 Sep 2016
  by Arturo Guadalupi
  modified 8 Sep 2016
  by Colby Newman

  This example code is in the public domain.

  https://www.arduino.cc/en/Tutorial/BuiltInExamples/Blink
*/
#include "Arduino_LED_Matrix.h"

ArduinoLEDMatrix matrix;

// the setup function runs once when you press reset or power the board
void setup() {
  // initialize digital pin LED_BUILTIN as an output.
  pinMode(0, INPUT);
  pinMode(1, OUTPUT);
  pinMode(2, OUTPUT);

  pinMode(8, INPUT);
  pinMode(9, OUTPUT);

  matrix.begin();
}

// the loop function runs over and over again forever
void loop() {

  bool showRed = digitalRead (0) || InRange(10);

  if (showRed)
  {
      digitalWrite(1, HIGH);  // turn the LED on (HIGH is the voltage level)
      digitalWrite(2, LOW); 
      delay(1000);     
  }
  else
  {
      digitalWrite(1, LOW);  // turn the LED on (HIGH is the voltage level)
      digitalWrite(2, HIGH); 
  }

  byte frame[8][12] = {
    { 0, 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0 },
    { 0, 1, 0, 0, 1, 0, 1, 0, 0, 1, 0, 0 },
    { 0, 1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0 },
    { 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0 },
    { 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0 },
    { 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 0 },
    { 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0 },
    { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
  };

  matrix.renderBitmap(frame, 8, 12);
}

bool InRange (long range)
{
  // Distance measurement is started by means of the 10us long trigger signal
  digitalWrite (9, HIGH);
  delayMicroseconds (10);
  digitalWrite (9, LOW);

  long duration = pulseIn (8, HIGH);
  
 // Now the distance is calculated using the recorded time
  long distance = duration / 58.2;

  return distance <= range;
}

void BlinkRed (int duration)
{
  Blink (1, duration);
}

void BlinkGreen (int duration)
{
  Blink (0, duration);
}

void Blink (int pin, int duration)
{
  digitalWrite(pin, HIGH);  // turn the LED on (HIGH is the voltage level)
  delay(duration);                      // wait for a second
  digitalWrite(pin, LOW);   // turn the LED off by making the voltage LOW
  //delay(duration);                  // wait for a second

}
