using System;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;

namespace testeProjeto
{
    class Program
    {

        private static int counter = 0;
        private static readonly object lockObject = new object();
        public static void Increment()
        {
            lock (lockObject)
            {
                counter++;
            }
        }
        public static void Decrement()
        {
            lock (lockObject)
            {
                counter--;
            }
        }
        public static int Counter
        {
            get
            {
                lock (lockObject)
                {
                    return counter;
                }
            }
        }
        public static async Task Talk(string text)
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
                //Console.WriteLine("Digita o que queres que diga");
                //Console.Write("> ");
                //string text = Console.ReadLine();

                //Esperar até que tudo seja dito
                using (var result = await synthesizer.SpeakTextAsync(text))
                {
                    if (result.Reason == ResultReason.SynthesizingAudioCompleted)
                    {
                        Console.WriteLine($"Texto sintetisado para [{text}]");
                        if(Counter == 5)
                        {
                            System.Environment.Exit(1);
                        }
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
                    Console.WriteLine($"Reconhecemos: {result2.Text}");
                    if (result2.Text == "OK." || result2.Text == "Sim." || result2.Text.Contains("OK."))
                    {
                        Increment();
                        if (Counter == 1)
                        {
                            await Talk("Bloco 2");
                        }
                        if(Counter == 2)
                        {
                            await Talk("Nível 0");

                        }
                        if (Counter == 3)
                        {
                            await Talk("Dígito 1");
                        }
                        if (Counter == 4)
                        {
                            if (result2.Text.Contains("265"))
                            {
                                await Talk("265, 3 caixas");
                            }
                            else
                            {
                                Decrement();
                                await Talk("ID inválido, 265 seria o correto");
                            }
                        }
                        if(Counter == 5)
                        {
                            if (result2.Text.Contains("3"))
                            {
                                await Talk("Processo concluido!");
                            }
                            else
                            {
                                Decrement();
                                await Talk("Número inválido, 3 caixas seria o correto");
                            }
                        }
                    }
                    else
                    {
                        String repeat;
                        if (Counter == 0)
                        {
                            repeat = "já está no corredor 1 ?";
                            await Talk("Não entendi o que foi dito ," + repeat);
                        }
                        if (Counter == 1)
                        {
                            repeat = "já localizou o bloco 2?";
                            await Talk("Não entendi o que foi dito ," + repeat);
                        }
                        if (Counter == 2)
                        {
                            repeat = "já localizou o nível 0?";
                            await Talk("Não entendi o que foi dito ," + repeat);
                        }
                        if (Counter == 3)
                        {
                            repeat = "já localizou o dígito 1?";
                            await Talk("Não entendi o que foi dito ," + repeat);
                        }
                        if (Counter == 4)
                        {
                            repeat = "já obteve as 3 caixas?";
                            await Talk("Não entendi o que foi dito ," + repeat);
                        }
                       
                    }

                }
                else if (result2.Reason == ResultReason.NoMatch)
                {
                    Console.WriteLine($"NOMATCH: Speech could not be recognized.");
                    if(Counter == 0)
                    {
                        await Talk("Já está no corredor 1?");
                    }
                    if(Counter == 1)
                    {
                        await Talk("Já localizou o bloco 2?");
                    }
                    if(Counter == 2)
                    {
                        await Talk("Já localizou o nível 0?");
                    }
                    if(Counter == 3)
                    {
                        await Talk("Já localizou o dígito 1?");
                    }
                    if (Counter == 4)
                    {
                        await Talk("Já obteve as 3 caixas?");
                    }
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
            await Talk("Vá para o corredor 1");
        }
    }
}