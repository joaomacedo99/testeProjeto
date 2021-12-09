using System;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;

namespace testeProjeto
{
    class Program
    {
        public static async Task Talk()
        {
            //Definir a configuração
            var config = SpeechConfig.FromSubscription("18d7c45cbb6544ef8a7e6f08834692ff", "westeurope");
            config.SpeechSynthesisLanguage = "pt-PT";
            config.SpeechSynthesisVoiceName = "pt-PT-FernandaNeural";
            config.SpeechRecognitionLanguage = "pt-PT";
            // Criar o gerador de voz com a respetiva configuração
            using (var synthesizer = new SpeechSynthesizer(config))
            {
                //Definir o que vai ser dito
                Console.WriteLine("Digita o que queres que diga");
                Console.Write("> ");
                string text = Console.ReadLine();

                //Esperar até que tudo seja dito
                using (var result = await synthesizer.SpeakTextAsync(text))
                {
                    if (result.Reason == ResultReason.SynthesizingAudioCompleted)
                    {
                        Console.WriteLine($"Speech synthesized to speaker for text [{text}]");
                        await Listen();
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

        public static async Task Listen()
        {

            //Definir a configuração
            var config = SpeechConfig.FromSubscription("18d7c45cbb6544ef8a7e6f08834692ff", "westeurope");
            config.SpeechSynthesisLanguage = "pt-PT";
            config.SpeechSynthesisVoiceName = "pt-PT-FernandaNeural";
            config.SpeechRecognitionLanguage = "pt-PT";

            //Criar detetor de voz 
            using (var recognizer = new SpeechRecognizer(config))
            {
                Console.WriteLine("Diz alguma coisa...");

                var result2 = await recognizer.RecognizeOnceAsync();
                if (result2.Reason == ResultReason.RecognizedSpeech)
                {
                    Console.WriteLine($"We recognized: {result2.Text}");
                    if (result2.Text == "OK.")
                    {
                        await Talk();
                    }

                }
                else if (result2.Reason == ResultReason.NoMatch)
                {
                    Console.WriteLine($"NOMATCH: Speech could not be recognized.");
                }
                else if (result2.Reason == ResultReason.Canceled)
                {
                    var cancellation = CancellationDetails.FromResult(result2);
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


        static async Task Main()
        {
            await Talk();
        }
    }
}