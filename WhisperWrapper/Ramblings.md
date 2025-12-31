
This is for converting audio to text using OpenAI's Whisper model.
It also includes diarization functionality using pyannote.audio.
	- This means that it can identify different speakers in the audio.
# Whisper Wrapper with Diarization

The ultimate goal is to have it ingest a .wav or similar audio file, 
then export a text file with the transcribed text, including speaker labels. 
Most likely, this will be in a json format for easy parsing later on.

Currently, we are in need of a model for diarization. The pyannote.audio library is a good choice for this task.
The Whisper model from OpenAI will be used for transcription.

We need to download the necessary models from Hugging Face and set up the environment accordingly.
Due to the Cisco firewall, we may need to manually download the models and place them in the appropriate directories.
So, until we have that set up, we will focus on the transcription part using Whisper.

The transcription process will involve loading the audio file, processing it with the Whisper model, and then saving the output to a text file.

As of 22Dec2025, the Whisper model has been setup, but remains untested. 
To conduct a test, you will need need to modify appsettings to point to a valid audio file on your system.
The output path will also probably need to be modified.

		...I'm tired.
