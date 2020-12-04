# MXChip IoT DevKit Tone Analyzer [![js-standard-style](https://img.shields.io/badge/code%20style-standard-brightgreen.svg?style=flat)](https://github.com/feross/standard)
A tone analyzer application built for the Microsoft Devkit AZ316. The app analyses the sentences you record to then come up with an accurate analysis of the tone of your words.

## How it works
<img src="Media/diagram.png">
The IoT DevKit records your voice then posts an HTTP request to trigger an Azure Function. The Azure Function calls the speech-to-text Cognitive Service API to do a transcript of your recording. The function then sends the transcript to an text analysis service of your choice (either the Microsoft's Text Analytics Cognitive service or the IBM's Watson Tone Analyzer). After Azure Functions gets the analysis from the chosen service, it then sends a C2D message to the device through the IoT Hub. Finally the analysis is displayed on the screen.

## Requirements
* MXChip IoT DevKit
* Visual Studio Code
* [Arduino](https://www.arduino.cc/) installed on your machine (note: Windows make sure you use the installer for Windows XP and up!)
* Microsoft Azure account
* IBM Cloud account

## How to set it up
* [Configure the MXChip board](https://github.com/jimbobbennett/MXChip-Workshop/blob/master/Steps/ConfigureTheBoard.md) 
* [Configure Visual Studio Code](https://github.com/jimbobbennett/MXChip-Workshop/blob/master/Steps/ConfigureVSCode.md)
* [Connect IoT DevKit AZ3166 to Azure IoT Hub](https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-arduino-iot-devkit-az3166-get-started)
* Make sure you create the resources in Azure for the Speech and Text Analytics Cognitive Services
* Set up the [Watson Tone Analyzer] service on IBM Cloud(https://www.ibm.com/watson/services/tone-analyzer/)
* Copy your personal API endpoints and keys of the services you created to the ToneAnalyserFunction app
* Publish the ToneAnalyserFunction app
* Get ToneAnalyserFunction endpoint and copy it to "AZURE_FUNCTION_URL" in the ToneAnalyserDevice's arduino code
* Upload the ToneAnalyserDevice's arduino code to the MXChip board

## How to use
#### 1. Press A to change the mode
<img src="Media/step_1.jpeg" height="320">

#### 2. Select mode ("Sentiment" for Text Analytics Cognitive Service and "Emotion" for Watson Tone Analyzer)
<img src="Media/step_2.jpeg" height="320">

#### 3. Record your voice
<img src="Media/step_3.jpeg" height="320">

#### 4. Wait for the result
<img src="Media/step_4.jpeg" height="320">

## Known issues
Unfortunately the software is able to record a bit more than two seconds worth of audio. This is due to the limited memory of the IoT Devkit AZ316.

## Contribute
Feel free to create PRs for this repo if you have any code suggestion!

## Special thanks
[Jim Bennett](https://github.com/jimbobbennett) for hosting the [MXChip Workshop](https://github.com/jimbobbennett/MXChip-Workshop) at the University of Plymouth in which he introduced me to the IoT DevKit and Microsoft Azure for the very first time.

## References
* [Microsoft Text Analytics](https://docs.microsoft.com/en-gb/azure/cognitive-services/text-analytics/)
* [Microsoft Speech Service](https://docs.microsoft.com/nb-no/azure/cognitive-services/speech-service/)
* [IBM Watson Tone Analyzer](https://cloud.ibm.com/apidocs/tone-analyzer#introduction)
* Code inspiration: [IoT DevKit Translator](https://github.com/Azure-Samples/mxchip-iot-devkit-translator/blob/master/Device/DevKitTranslator.ino)
