
---

### **Device Inventory Fields**  
From `GET /organizations/{organizationId}/devices`:

- **Identification & Assignment**
  - `name`
  - `serial`
  - `mac`
  - `networkId`
  - `model`
  - `productType` (e.g., appliance, switch, wireless)

- **Location & Contact**
  - `address`
  - `notes`
  - `tags`
  - `lat`, `lng`

- **IP & Connectivity**
  - `lanIp`
  - `wan1Ip`, `wan2Ip`
  - `publicIp`
  - `uplink` (WAN status)

- **Status & Versioning**
  - `status` (online/offline)
  - `firmware`
  - `lastReportedAt`
  - `usage` (bandwidth stats)

- **Hardware**
  - `lanPorts`
  - `wanPorts`
  - `switchProfileId`

---

### **Switch Port Configuration Fields**  
From `GET /devices/{serial}/switch/ports`:

- `portId`
- `enabled`
- `type` (access/trunk)
- `vlan`
- `voiceVlan`
- `allowedVlans`
- `poeEnabled`
- `isolationEnabled`
- `rstpEnabled`
- `stpGuard`
- `linkNegotiation`
- `portScheduleId`
- `udld`
- `macWhitelist`
- `stickyMacWhitelist`
- `stickyMacWhitelistLimit`
- `stormControlEnabled`
- `accessPolicyType`
- `authPolicyEnabled`
- `trusted`
- `profileId`

---

### **Network-Level Configuration (Optional Additions)**
From endpoints like:
- `GET /networks/{networkId}/vlans`
- `GET /networks/{networkId}/ssids`

You can retrieve:
- VLAN IDs, names, subnets, DHCP settings
- SSID names, encryption, splash pages, radius settings

---

### **Pro Tip**
If you're building a full backup, consider pulling:
- **Device inventory**
- **Switch port configs**
- **VLANs**
- **SSIDs**
- **Uplink status** (`GET /devices/{serial}/uplink`)

