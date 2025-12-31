# Shell
# You’ll need a Hugging Face access token to pull the pretrained diarization pipeline. Create one in your HF account, then:
#pip install pyannote.audio
#export HF_TOKEN="hf_your_personal_access_token"


from pyannote.audio import Pipeline

# Pretrained diarization pipeline. You can swap models later if needed.
pipeline = Pipeline.from_pretrained(
    "pyannote/speaker-diarization",
    use_auth_token=os.environ["HF_TOKEN"]
)

# Runs local; GPU recommended for speed, but CPU works for smaller files.
diarization = pipeline("path/to/file.mp4")  # accepts wav/mp3/mp4 etc.

# diarization is a timeline with speaker-labelled segments
# Example entry: segment.start, segment.end, track = speaker label e.g. "SPEAKER_00"
speaker_segments = []
for turn, _, speaker in diarization.itertracks(yield_label=True):
    speaker_segments.append({
        "start": turn.start,
        "end": turn.end,
        "speaker": speaker
    })

#Notes:
#- pyannote works locally; GPU (CUDA) speeds it up.
#- You can constrain expected speakers or tune hyper‑parameters later if needed.
