using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using System.IO;


namespace NCRMSFTCognitiveSpeech
{
    class Program
    {
        //TODO - replace keys
        static string SubscriptionKey="", ServiceRegion ="";

        static void Main(string[] args)
        {
        }
        
        /// <summary>
        /// medthod to convert Speech to text using Microphone
        /// </summary>
        /// <returns></returns>
        public static async Task SpeechToTextAsyncwithMicrophone()
        {
            // Creates an instance of a speech config with specified subscription key and service region.
            // Replace with your own subscription key and service region (e.g., "eastus").
            var config = SpeechConfig.FromSubscription(SubscriptionKey, ServiceRegion);

            // Creates a speech recognizer.
            using (var recognizer = new SpeechRecognizer(config))
            {
                Console.WriteLine("Say something...");

                // Starts speech recognition, and returns after a single utterance is recognized. The end of a
                // single utterance is determined by listening for silence at the end or until a maximum of 15
                // seconds of audio is processed.  The task returns the recognition text as result. 
                // Note: Since RecognizeOnceAsync() returns only a single utterance, it is suitable only for single
                // shot recognition like command or query. 
                // For long-running multi-utterance recognition, use StartContinuousRecognitionAsync() instead.

                var result = await recognizer.RecognizeOnceAsync();

                // Checks result.
                if (result.Reason == ResultReason.RecognizedSpeech)
                {
                    Console.WriteLine($"We recognized: {result.Text}");
                }
                else if (result.Reason == ResultReason.NoMatch)
                {
                    Console.WriteLine($"NOMATCH: Speech could not be recognized.");
                }
                else if (result.Reason == ResultReason.Canceled)
                {
                    var cancellation = CancellationDetails.FromResult(result);
                    Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                    if (cancellation.Reason == CancellationReason.Error)
                    {
                        Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                        Console.WriteLine($"CANCELED: ErrorDetails={cancellation.ErrorDetails}");
                        Console.WriteLine($"CANCELED: Did you update the subscription info?");
                    }
                }
            }
        }

        /// <summary>
        /// Method to convert Wav Speech file to Text
        /// </summary>
        /// <param name="wavfileName">fully qualified waf file path</param>
        /// <returns></returns>
        public static async Task SpeechToTextFromWavFileInput(string wavfileName)
        {
            var taskCompleteionSource = new TaskCompletionSource<int>();

            var config = SpeechConfig.FromSubscription(SubscriptionKey, ServiceRegion);

            var transcriptionStringBuilder = new StringBuilder();

            using (var audioInput = AudioConfig.FromWavFileInput(wavfileName))
            {
                using (var recognizer = new SpeechRecognizer(config, audioInput))
                {
                    try
                    {
                        // Subscribes to events.  
                        recognizer.Recognizing += (sender, eventargs) =>
                        {
                            Console.WriteLine(eventargs.Result.Text); //view text as it comes in  
                        };

                        recognizer.Recognized += (sender, eventargs) =>
                        {
                          
                            if (eventargs.Result.Reason == ResultReason.RecognizedSpeech)
                            {
                                transcriptionStringBuilder.Append(eventargs.Result.Text);
                            }
                            else if (eventargs.Result.Reason == ResultReason.NoMatch)
                            {
                                //TODO: Handle not recognized value  
                            }
                        };

                        recognizer.Canceled += (sender, eventargs) =>
                        {
                            if (eventargs.Reason == CancellationReason.Error)
                            {
                                //TODO: Handle error  
                            }

                            if (eventargs.Reason == CancellationReason.EndOfStream)
                            {
                                //invoked when end of stream is reached
                                Console.WriteLine(transcriptionStringBuilder.ToString());
                            }

                            taskCompleteionSource.TrySetResult(0);
                        };

                        recognizer.SessionStarted += (sender, eventargs) =>
                        {
                            // Console.WriteLine(transcriptionStringBuilder.ToString());
                            //Started recognition session  
                        };

                        recognizer.SessionStopped += (sender, eventargs) =>
                        {
                            //Ended recognition session  
                            taskCompleteionSource.TrySetResult(0);
                        };
                        try
                        {
                            // Starts continuous recognition. Uses StopContinuousRecognitionAsync() to stop recognition.  
                            await recognizer.StartContinuousRecognitionAsync().ConfigureAwait(true);

                            // Waits for completion.  
                            // Use Task.WaitAny to keep the task rooted.  
                            Task.WaitAny(new[] { taskCompleteionSource.Task });

                            // Stops recognition.  
                            await recognizer.StopContinuousRecognitionAsync();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Method to convert text to speech wav file
        /// </summary>
        /// <param name="inuptext"></param>
        /// <param name="wavfilename"></param>
        /// <returns></returns>
        public static async Task SynthesisToWaveFileAsync(string inuptext, string wavfilename)
        {
            // Creates an instance of a speech config with specified subscription key and service region.
            // Replace with your own subscription key and service region (e.g., "westus").
            // The default language is "en-us".
            var config = SpeechConfig.FromSubscription(SubscriptionKey, ServiceRegion);

            // Creates a speech synthesizer using file as audio output.
            // Replace with your own audio file name.
            var fileName = wavfilename;
            using (var fileOutput = AudioConfig.FromWavFileOutput(fileName))
            using (var synthesizer = new SpeechSynthesizer(config, fileOutput))
            {
                using (var result = await synthesizer.SpeakTextAsync(inuptext))
                {
                    if (result.Reason == ResultReason.SynthesizingAudioCompleted)
                    {
                        Console.WriteLine($"Speech synthesized for text [{inuptext}], and the audio was saved to [{fileName}]");
                    }
                    else if (result.Reason == ResultReason.Canceled)
                    {
                        var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
                        Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                        if (cancellation.Reason == CancellationReason.Error)
                        {
                            Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                            Console.WriteLine($"CANCELED: ErrorDetails=[{cancellation.ErrorDetails}]");
                            Console.WriteLine($"CANCELED: Did you update the subscription info?");
                        }
                    }
                }
            }
        }
    }
}
