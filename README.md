# MXChip IoT DevKit Tone Analyzer [![js-standard-style](https://img.shields.io/badge/code%20style-standard-brightgreen.svg?style=flat)](https://github.com/feross/standard)
A tone analyzer application built for the Microsoft Devkit AZ316. It can analyze a registration of your voice to tell you the sentiment or the emotions behind the choice of your words.

## Motivation
This project is a little experiment to find out how we can possibly concatenate the different AI services that are available. 

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
* Set up the Speech and Text Analytics Cognitive Services
* Set up the [IBM Watson Tone Analyzer](https://www.ibm.com/watson/services/tone-analyzer/)
* You will need to create a Function App on your Azure and publish the ToneAnalyserFunction of this repo
* Copy personal connection strings and API keys from the services to the Azure Function application main class
* Copy Function endpoint to AZURE_FUNCTION_URL in the DevKit code
* Upload the device code contained in the ToneAnalyserDevice folder to the DevKit and you are ready to go

## API Reference
* [Microsoft Text Analytics](https://docs.microsoft.com/en-gb/azure/cognitive-services/text-analytics/)
* [Microsoft Speech Service](https://docs.microsoft.com/nb-no/azure/cognitive-services/speech-service/)
* [IBM Watson Tone Analyzer](https://cloud.ibm.com/apidocs/tone-analyzer#introduction)

## How to use?
#### 1. Press A to change the mode
<img src="Media/step_1.jpeg" height="320">

#### 2. Select mode ("Sentiment" for Text Analytics Cognitive Service and "Emotion" for Watson Tone Analyzer)
<img src="Media/step_2.jpeg" height="320">

#### 3. Record your voice
<img src="Media/step_3.jpeg" height="320">

#### 4. Wait for the result
<img src="Media/step_4.jpeg" height="320">

## Special thanks
[Jim Bennet](https://github.com/jimbobbennett) for hosting the [MXChip Workshop](https://github.com/jimbobbennett/MXChip-Workshop) at the University of Plymouth where he introducing me to the IoT DevKit and Azure for the first time.
