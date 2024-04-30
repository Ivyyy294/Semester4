//LED
int incomingByte[2];
int ledPin = 13;
bool LED = false;

//Distance
int Echo_EingangsPin = 8; // Echo input pin
int Trigger_AusgangsPin = 7; // Trigger output pin
int maximumRange = 300;
int minimumRange = 2;
long distance;
long duration;

void setup() {
  pinMode (ledPin, OUTPUT);
  pinMode (Trigger_AusgangsPin, OUTPUT);
  pinMode (Echo_EingangsPin, INPUT);
  digitalWrite(ledPin, LOW);
  // put your setup code here, to run once:
  Serial.begin(9600);
}

void loop() 
{

  if (Serial.available() > 0)
  {
    
    while (Serial.peek() == 'L')
    {
      Serial.read();
      incomingByte[0] = Serial.parseInt();

      if (incomingByte[0] == 1)
        LED = true;
      else
        LED = false;
    }
    while (Serial.available() > 0)
      Serial.read();
  } 

  ledCheck();

  String data = String(GetDistance());
  Serial.println (data);
}

void ledCheck()
{
  if (LED)
    digitalWrite(ledPin, HIGH);
  else
    digitalWrite(ledPin, LOW);
}

float GetDistance()
{
  // Distance measurement is started by means of the 10us long trigger signal
 digitalWrite (Trigger_AusgangsPin, HIGH);
 delayMicroseconds (10);
 digitalWrite (Trigger_AusgangsPin, LOW);
  
 // Now we wait at the echo input until the signal has been activated
 // and then the time measured how long it remains activated
 float duration = pulseIn (Echo_EingangsPin, HIGH);
  
 // Now the distance is calculated using the recorded time
 float distance = duration / 58.2;

 return distance;
}