# NCRMSFTCognitiveSpeech

# Sample Repository to convert Speech to Text using Microsoft Cognitive Services Speech SDK

# Overview

This project is a sample to convert Speech (WAV file format) to text and text to speech using Microsoft Cognitive Services Speech SDK. To find out more about the Microsoft Cognitive Services Speech SDK itself, please visit the SDK documentation site at: https://docs.microsoft.com/en-us/azure/cognitive-services/speech-service/.

This sample was written as C# Console app for .NET Framework on Windows using Visual Studio 2017.


# Build and run the samples

# 1. Setting up Azure Cognitive Services - SpeechServices instance
   
   i. Before you can use Azure SpeechServices you need to add instance to your Azure account. It is categorized as Azure Congitive Services so from dashboard find Cognitive Services in portal.azure.com
   
   ii. Add or Create cognitive services button to create a new SpeechService Cognitive Service instance. In the search bar type "Speech" and in the result list you will Speech item available


   iii. select Speech item from the result list and populate the mandatory fields


   iv. for demo or development, choose F0 tier which is free and comes with cetain limitations. Click Create button and your SpeechService instance is ready for usage


   v. Click on the name of your SpeechService instance you created and got to Overview option


   vi. You will need region value in the code in order to instantiate SpeechConfig class instance which you will use to instantiate SpeechRecognizer class which will communicate with your SpeechService instance. I choose East US region to keep my instance in, so the value in my code will be eastus


   vii. The second value you need to configure SpeechRecognizer is SpeechService key. Select Keys section for selected SpeechService in Azure portal and you will get your instance keys in the right page of the portal page.
  
  
# 2. Install Microsoft.CognitiveServices.Speech version 1.6.0 from Nuget Package manager 
   
Note: the samples make use of the Microsoft Cognitive Services Speech SDK. By downloading the Microsoft Cognitive Services Speech SDK, you acknowledge its license, see Speech SDK license agreement.


# 3. Replace with your own subscription key and service region for the following code in program.cs from vii and vi from step#1 

var config = SpeechTranslationConfig.FromSubscription("YourSubscriptionKey", "YourServiceRegion"); 


# 4. Pass wav file with fully qualified path in line 16 in program.cs to convert to text


Note: This application expects bitrate of 128kbps or higher for wav file. The application will not convert speech to text and
will abort after executing "await recognizer.StartContinuousRecognitionAsync().ConfigureAwait(true);" and doesn't throw exception



# Workaround: Convert wav file to higher bitrate for the purposes of this demo. Below are the steps performed before running the application

   a. Convert pyron wav file to m4a format and save to disk
   
   
   b. Convert m4a format file created in step a to wav file with minimum 128kbps bitrate


# 5. Run console app and the converted speech will be displayed as text on the console

