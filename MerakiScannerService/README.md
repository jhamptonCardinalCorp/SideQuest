" — this is a neat little service"

### What the code does

This project sets up a **background service** using `BackgroundService` and hosts a minimal **web API** using `WebApplication` inside that service. Here's a breakdown of its behavior:

#### `GET /api/meraki`
- Returns a hardcoded string: `"FMIG123456"`.
- This is likely used as a **validation token** — Meraki webhooks often require a validation step where the server must echo back a token to confirm it can receive events.

#### `POST /api/meraki`
- Accepts a JSON payload (likely from Meraki).
- Logs the raw payload to a file: `meraki_payloads.txt`.
- Tries to parse the JSON and extract the `"observations"` array.
- Logs the number of devices observed, if present.
- Handles JSON parsing errors gracefully and logs them.

### What this project is likely for

Based on the name `MerakiScannerService` and the behavior:

- This looks like a **Meraki scanning API receiver** — Meraki access points can send **Bluetooth and Wi-Fi scanning data** to a webhook endpoint.
- The `"observations"` field is typical of Meraki scanning payloads, which contain detected devices.
- The service is probably meant to **collect and log presence data** from Meraki APs for later analysis or integration.

### Why the name might be off

The name `MerakiScannerService` is actually pretty close to accurate, but I was making multiple projects that day, it’s possible I mixed up naming conventions. 
This could also be called:

- `MerakiWebhookReceiver`
- `MerakiPresenceLogger`
- `MerakiValidatorService`

… `MerakiScannerService` might have been a placeholder or a blend of ideas.

MerakiPresenceReceiver would be a more descriptive name, but the existing name is still quite fitting given the context.
