#include "AudioClass.h"
#include "AZ3166WiFi.h"
#include "DevKitMQTTClient.h"
#include "OledDisplay.h"
#include "SystemTickCounter.h
#include "azure_config.h"
#include "http_client.h"

#define SERVICES_COUNT 2
#define MAX_RECORD_DURATION 10
#define MAX_UPLOAD_SIZE (64 * 1024)
#define LOOP_DELAY 100
#define PULL_TIMEOUT 30000
#define AUDIO_BUFFER_SIZE ((32000 * MAX_RECORD_DURATION) - 16000 + 44 + 1)
#define ERROR_INFO "Sorry, I can't \r\nhear you."

enum STATUS
{
  Idle,
  Recording,
  Recorded,
  WavReady,
  Uploaded,
  SelectService
};

static int wavFileSize;
static char *waveFile = NULL;
static char azureFunctionUri[192];

// The timeout for retrieving the result
static uint64_t result_timeout_ms;

// Audio instance
static AudioClass &Audio = AudioClass::getInstance();

// AI Service
static int currentService = 1; 
static const char *allServices[SERVICES_COUNT] = {"SentimentAnalysis", "WatsonToneAnalyzer"};

// Indicate the processing status
static STATUS status = Idle;

// Indicate whether WiFi is ready
static bool hasWifi = false;

//////////////////////////////////////////////////////////////////////////////////////////////////////////
// Utilities
static void InitWiFi()
{
  Screen.print(2, "Connecting...");

  if (WiFi.begin() == WL_CONNECTED)
  {
    IPAddress ip = WiFi.localIP();
    Screen.print(1, ip.get_address());
    hasWifi = true;
    Screen.print(2, "Running... \r\n");
  }
  else
  {
    hasWifi = false;
    Screen.print(1, "No Wi-Fi\r\n ");
  }
}

static void EnterIdleState(bool clean = true)
{
  status = Idle;
  if (clean)
  {
    Screen.clean();
  }
  Screen.print(0, "Hold B to talk");
}

static int HttpTriggerAnalyzer(const char *content, int length)
{
  if (content == NULL || length <= 0 || length > MAX_UPLOAD_SIZE)
  {
    Serial.println("Content not valid");
    return -1;
  }

  sprintf(azureFunctionUri, "%s&&source=%s", (char *)AZURE_FUNCTION_URL, allServices;
  HTTPClient client = HTTPClient(HTTP_POST, azureFunctionUri);
  client.set_header("source", allServices);
  const Http_Response *response = client.send(content, length);

  if (response != NULL && response->status_code == 200)
  {
    return 0;
  }
  else
  {
    return -1;
  }
}

static void ShowServices()
{
  char temp[20];

  int idx = (currentService + SERVICES_COUNT - 1) % SERVICES_COUNT;
  sprintf(temp, "  %s", allServices[idx]);
  Screen.print(1, temp);

  sprintf(temp, "> %s", allServices[(idx + 1) % SERVICES_COUNT]);
  Screen.print(2, temp);

  sprintf(temp, "  %s", allServices[(idx + 2) % SERVICES_COUNT]);
  Screen.print(3, temp);
}

//////////////////////////////////////////////////////////////////////////////////////////////////////////
// Callback functions
static void ResultMessageCallback(const char *text, int length)
{
  Serial.printf("Enter message callback, text: %s\r\n", text);
  if (status != Uploaded)
  {
    return;
  }

  EnterIdleState();
  if (text == NULL)
  {
    Screen.print(1, ERROR_INFO);
    return;
  }

  char temp[33];
  int end = min(length, sizeof(temp) - 1);
  memcpy(temp, text, end);
  temp[end] = '\0';
  Screen.print(1, "Tone: ");
  Screen.print(2, temp, true);
  LogTrace("DevKitToneAnalyzerSucceed");
}

//////////////////////////////////////////////////////////////////////////////////////////////////////////
// Actions
static void DoIdle()
{
  if (digitalRead(USER_BUTTON_A) == LOW)
  {
    // Enter Select Service mode
    status = SelectService;
    Screen.clean();
    Screen.print(0, "Press B Scroll");
    ShowServices();
  }
  else if (digitalRead(USER_BUTTON_B) == LOW)
  {
    // Enter the Recording mode
    Screen.clean();
    Screen.print(0, "Recording...");
    Screen.print(1, "Release to send\r\nMax duraion: \r\n1.5 sec");
    memset(waveFile, 0, AUDIO_BUFFER_SIZE);
    Audio.format(8000, 16);
    Audio.startRecord(waveFile, AUDIO_BUFFER_SIZE, MAX_RECORD_DURATION);
    status = Recording;
  }

  DevKitMQTTClient_Check();
}

static void DoRecording()
{
  if (digitalRead(USER_BUTTON_B) == HIGH)
  {
    Audio.stop();
    status = Recorded;
  }
}

static void DoRecorded()
{
  Audio.getWav(&wavFileSize);
  if (wavFileSize > 0)
  {
    wavFileSize = Audio.convertToMono(waveFile, wavFileSize, 16);
    if (wavFileSize <= 0)
    {
      Serial.println("ConvertToMono failed! ");
      EnterIdleState();
    }
    else
    {
      status = WavReady;
      Screen.clean();
      Screen.print(0, "Processing...");
      Screen.print(1, "Uploading...");
    }
  }
  else
  {
    Serial.println("No Data Recorded! ");
    EnterIdleState();
  }
}

static void DoWavReady()
{
  if (wavFileSize <= 0)
  {
    EnterIdleState();
    return;
  }

  int ret = HttpTriggerToneAnalyzer(waveFile, wavFileSize);
  Serial.printf("Azure Function return value: %d\r\n", ret);
  if (ret == 0)
  {
    status = Uploaded;
    Screen.print(1, "Receiving...");
    // Start retrieving result timeout clock
    result_timeout_ms = SystemTickCounterRead();
  }
  else
  {
    Serial.println("Error happened when analyzing: Azure Function failed");
    EnterIdleState();
    Screen.print(1, ERROR_INFO);
  }
}

static void DoUploader()
{
  DevKitMQTTClient_ReceiveEvent();

  if ((int)(SystemTickCounterRead() - result_timeout_ms) >= PULL_TIMEOUT)
  {
    // Timeout
    EnterIdleState();
    Screen.print(1, "Client Timeout");
  }
}

static void DoSelectService()
{
  if (digitalRead(USER_BUTTON_B) == LOW)
  {
    currentService = (currentService + 1) % SERVICES_COUNT;
    ShowServices();
  }
  else if (digitalRead(USER_BUTTON_A) == LOW)
  {
    EnterIdleState();
  }
  DevKitMQTTClient_Check();
}

//////////////////////////////////////////////////////////////////////////////////////////////////////////
// Arduino sketch
void setup()
{
  Screen.init();
  Screen.print(0, "DevKitToneAnalyzer");

  if (strlen(AZURE_FUNCTION_URL) == 0)
  {
    Screen.print(2, "No Azure Func");
    return;
  }

  Screen.print(2, "Initializing...");
  pinMode(USER_BUTTON_A, INPUT);
  pinMode(USER_BUTTON_B, INPUT);

  Screen.print(3, " > Serial");
  Serial.begin(115200);

  // Initialize the WiFi module
  Screen.print(3, " > WiFi");
  hasWifi = false;
  InitWiFi();
  if (!hasWifi)
  {
    return;
  }
  LogTrace("DevKitToneAnalyzerSetup");
  
  DevKitMQTTClient_SetMessageCallback(ResultMessageCallback);

  // Audio
  Screen.print(3, " > Audio");
  waveFile = (char *)malloc(AUDIO_BUFFER_SIZE);
  if (waveFile == NULL)
  {
    Screen.print(3, "Audio init fails");
    Serial.println("No enough Memory!");
    return;
  }

  Screen.print(1, "Talk:   Hold  B\r\n \r\nSetting:Press A");
}

void loop()
{
  if (hasWifi)
  {
    switch (status)
    {
    case Idle:
      DoIdle();
      break;

    case Recording:
      DoRecording();
      break;

    case Recorded:
      DoRecorded();
      break;

    case WavReady:
      DoWavReady();
      break;

    case Uploaded:
      DoUploader();
      break;

    case SelectService:
      DoSelectService();
      break;
    }
  }
  delay(LOOP_DELAY);
}