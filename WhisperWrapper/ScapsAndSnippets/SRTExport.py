
import math

def srt_timestamp(t):
    h = int(t // 3600)
    m = int((t % 3600) // 60)
    s = int(t % 60)
    ms = int((t - math.floor(t)) * 1000)
    return f"{h:02}:{m:02}:{s:02},{ms:03}"

def write_srt(lines, path="output.srt", show_speaker=True):
    with open(path, "w", encoding="utf-8") as f:
        for i, line in enumerate(lines, start=1):
            f.write(f"{i}\n")
            f.write(f"{srt_timestamp(line['start'])} --> {srt_timestamp(line['end'])}\n")
            if show_speaker:
                f.write(f"{line['speaker']}: {line['text']}\n\n")
            else:
                f.write(f"{line['text']}\n\n")

write_srt(lines, "file_with_speakers.srt", show_speaker=True)
