
def assign_speaker_to_word(w, speaker_segments):
    # Find the speaker segment that overlaps the word the most
    best = None
    best_overlap = 0.0
    for seg in speaker_segments:
        s, e = seg["start"], seg["end"]
        overlap = max(0.0, min(w["end"], e) - max(w["start"], s))
        if overlap > best_overlap:
            best_overlap = overlap
            best = seg
    return best["speaker"] if best else "UNKNOWN"

# Attach speakers to each word
for w in words:
    w["speaker"] = assign_speaker_to_word(w, speaker_segments)


# Group continuous words by speaker and by small gaps (e.g., 1.0s) to form subtitle lines
lines = []
if words:
    cur_speaker = words[0]["speaker"]
    cur_start = words[0]["start"]
    cur_end = words[0]["end"]
    cur_text = [words[0]["text"]]

    gap_threshold = 1.0  # seconds; tune to taste

    for w in words[1:]:
        same_speaker = (w["speaker"] == cur_speaker)
        gap = w["start"] - cur_end

        if same_speaker and gap <= gap_threshold:
            cur_text.append(w["text"])
            cur_end = w["end"]
        else:
            lines.append({
                "speaker": cur_speaker,
                "start": cur_start,
                "end": cur_end,
                "text": " ".join(cur_text)
            })
            # start a new line
            cur_speaker = w["speaker"]
            cur_start = w["start"]
            cur_end = w["end"]
            cur_text = [w["text"]]

    # flush last
    lines.append({
        "speaker": cur_speaker,
        "start": cur_start,
        "end": cur_end,
        "text": " ".join(cur_text)
    })
