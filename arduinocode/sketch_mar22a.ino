#define echoPin 6
#define trigPin 7
#define buzzerPin 2

int engineOn=8;
int engineOff=9;
int disaydinlat1=10;
int disaydinlat2=11;
int maximumRange=50;
int minumumRange=0;
 
void setup() {
 Serial.begin(9600);
 pinMode(engineOn,OUTPUT);
 pinMode(4,OUTPUT);
 pinMode(engineOff,OUTPUT);
 pinMode(disaydinlat1,OUTPUT);
 pinMode(disaydinlat2,OUTPUT);
 pinMode(trigPin,OUTPUT);
 pinMode(echoPin,INPUT);
 pinMode(buzzerPin,OUTPUT);
 
}

void loop() {

   if(Serial.available())
 {
  char gelenBilgi=Serial.read();
  if(gelenBilgi=='1')
  {
    digitalWrite(engineOn,HIGH);
    digitalWrite(4,HIGH);
    digitalWrite(engineOff,LOW);
    digitalWrite(disaydinlat1,HIGH);
    digitalWrite(disaydinlat2,HIGH);
    
    }
  else
  {
      digitalWrite(engineOff,HIGH);
      digitalWrite(4,LOW);
      digitalWrite(engineOn,LOW);
      if(gelenBilgi=='2')
      {
        digitalWrite(disaydinlat1,HIGH);
    digitalWrite(disaydinlat2,HIGH);
        }
        else
        {
           digitalWrite(disaydinlat1,LOW);
         digitalWrite(disaydinlat2,LOW);
          }
  }
    
  
  }
  int olcum=mesafe(maximumRange, minumumRange);
  melodi(olcum*5);
  

  
  delay(100);

}
int mesafe(int maxrange ,int minrange)
{
  long duration, distance;
  digitalWrite(trigPin,LOW);
  delayMicroseconds(2);
  digitalWrite(trigPin,HIGH);
  delayMicroseconds(10);
  digitalWrite(trigPin,LOW);

  duration=pulseIn(echoPin,HIGH);
  distance=duration/58.2;
  delay(50);
  if(distance>=maxrange || distance <= minrange)
  return 0;
  return distance;
}

int melodi(int dly)
{
tone(buzzerPin,440);
delay(dly);
noTone(buzzerPin);
delay(dly);
  
}
