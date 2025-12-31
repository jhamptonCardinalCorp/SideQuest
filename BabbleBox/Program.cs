//      BabbleBox       //

using System;
using System.Runtime.CompilerServices;
using System.Speech.Synthesis;
using System.Threading.Tasks;

class Program
{
    static SpeechSynthesizer synth = new SpeechSynthesizer();

    //  Starting values.
    static VoiceGender voiceGender = VoiceGender.NotSet;
    static VoiceAge voiceAge = VoiceAge.NotSet;
    static int voiceRate = 0;
    static int voiceVolume = 80;
    static string voiceOutput = "";
    static Queue<string> script = new Queue<string>();

    //  What to say.
    //static List<string> script = new List<string>();

    static async Task Main()
    {
        //  Set default voice.
        TuneVoice();

        // Example script.
        //string script = "Hello! This is your script being read aloud by babble box.";

        // Speak the script aloud.
        //synth.Speak(script);

        Babble();
        await ScriptWriter();
        

    }

    // Set voice and speed as desired.
    public static void TuneVoice(
        VoiceGender? gender = null, 
        VoiceAge? age = null, 
        int? rate = null, 
        int? volume = null, 
        string? outputDevice = null)
    {
        voiceGender = gender ?? voiceGender;
        voiceAge = age ?? voiceAge;
        voiceRate = rate ?? voiceRate;
        voiceVolume = volume ?? voiceVolume;
        voiceOutput = outputDevice ?? voiceOutput;

        synth.SelectVoiceByHints(voiceGender, voiceAge);
        synth.Rate = voiceRate; // -10 to 10
        synth.Volume = voiceVolume;
        synth.SetOutputToDefaultAudioDevice();
    }

    public static async Task Babble(/* List<string> whatToSay, */CancellationToken ct = default)
    {
        while (!ct.IsCancellationRequested)
        {
            if (script.Count > 0)
            {
                synth.Speak(script.Dequeue());       //  Say the first/oldest line.
            }
            await Task.Delay(1000);                     //  Pause for better flow.

        }
    }

    public static async Task ScriptWriter(CancellationToken ct = default)
    {
        string writtenLine = "";
        while (!ct.IsCancellationRequested)
        {
            Console.WriteLine("What should I say?");
            writtenLine = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(writtenLine))
            {
                script.Enqueue(writtenLine);
            }
            await Task.Delay(1000);
        }
        return;
    }
}
