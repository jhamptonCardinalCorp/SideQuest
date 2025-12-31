# At a glance:
#   segments give you readable chunks for subtitles.
#   words carry granular timestamps for future diarization mapping.
#   The JSON envelope (segments, words, meta) becomes your API contract for downstream modules.

# transcribe.py
from faster_whisper import WhisperModel
import json, math, os, sys

def srt_timestamp(t):
    h = int(t // 3600); m = int((t % 3600) // 60); s = int(t % 60)
    ms = int((t - math.floor(t)) * 1000)
    return f"{h:02}:{m:02}:{s:02},{ms:03}"

def write_srt(lines, path):
    with open(path, "w", encoding="utf-8") as f:
        for i, line in enumerate(lines, start=1):
            f.write(f"{i}\n")
            f.write(f"{srt_timestamp(line['start'])} --> {srt_timestamp(line['end'])}\n")
            f.write(f"{line['text']}\n\n")

def write_txt(lines, path):
    with open(path, "w", encoding="utf-8") as f:
        for line in lines:
            f.write(f"{line['text']}\n")

def write_json(obj, path):
    with open(path, "w", encoding="utf-8") as f:
        json.dump(obj, f, ensure_ascii=False, indent=2)

def transcribe(media_path, model_name="large-v3", device="cuda", compute_type="float16",
               language=None, task="transcribe"):
    model = WhisperModel(model_name, device=device, compute_type=compute_type)

    segments, info = model.transcribe(
        media_path,
        vad=True,
        word_timestamps=True,
        language=language,     # e.g., "en"
        task=task              # "transcribe" or "translate"
    )

    # Build segment-level lines (no speakers yet)
    lines = []
    words = []
    for seg in segments:
        lines.append({"start": seg.start, "end": seg.end, "text": seg.text})
        if seg.words:
            for w in seg.words:
                words.append({"start": w.start, "end": w.end, "text": w.word})

    base = os.path.splitext(media_path)[0]
    write_srt(lines, base + ".srt")
    write_txt(lines, base + ".txt")
    write_json({"segments": lines, "words": words, "meta": {
        "language": info.language, "duration": info.duration
    }}, base + ".json")

if __name__ == "__main__":
    media = sys.argv[1]
    transcribe(media)
