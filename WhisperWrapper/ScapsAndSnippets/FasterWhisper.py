# # Example code using Faster Whisper to transcribe an audio file
# # Install faster-whisper via pip if you haven't already:

# from faster_whisper import WhisperModel
# model = WhisperModel("large-v3", device="cuda", compute_type="float16")  # "int8_float16" for lower VRAM
# segments, info = model.transcribe("path/to/file.mp4")
# for s in segments:
#     print(s.start, s.end, s.text)

#     # pip install pyannote.audio



from faster_whisper import WhisperModel

# Pick a model that fits your VRAM/CPU. large-v3 is accurate but heavy.
model = WhisperModel("large-v3", device="cuda", compute_type="float16")  # "int8_float16" lowers VRAM

# word_timestamps=True gives per-word timing. vad=True helps split speech/non-speech.
segments, info = model.transcribe(
    "path/to/file.mp4",
    vad=True,
    word_timestamps=True,
    language=None,          # let it auto-detect, or set "en" to force English
    task="transcribe"       # or "translate" to English
)

# Collect a normalized list of words with timestamps
words = []  # each item: {"start": float, "end": float, "text": str}
for seg in segments:
    if seg.words:  # seg.words is a list of faster_whisper.transcribe.Word objects
        for w in seg.words:
            words.append({"start": w.start, "end": w.end, "text": w.word})

#Tips:
# • If you’re on CPU, consider medium or small, or switch to compute_type="int8" for speed and memory relief.
# • vad=True helps keep segments cleaner; you can also pre‑normalize audio (16k mono) for consistency.

