PROMPT START

I want to design and implement a distributed, secure messaging system with the following constraints and architecture:

Core Requirements
Closed group: ~20–30 users (scalable up to ~100)
Desktop application in C# (.NET)
Local database: SQLite
Single device per user (strict) — no multi-device support
Low throughput system (not real-time chat)
Users may have limited or intermittent internet access
Some users are inside restricted networks (e.g., country-level filtering)
Network Model
No central server
Fully peer-to-peer (P2P) system
Store–carry–forward (delay-tolerant) communication
Users sync opportunistically:
Direct internet sync (when possible)
Indirect sync via bridge users (who move between restricted and open networks)
Data Model
Append-only message log (no updates, no deletes)
Each node stores all messages (even if unreadable)
SQLite schema:
Messages table with:
Id (GUID, primary key)
SenderId
ReceiverId
Topic
Timestamp
Ciphertext (encrypted message body)
Nonce
Signature
Identity Model
On first run:
User enters a name/alias
App generates:
Ed25519 keypair (identity + signing)
X25519 keypair (encryption)
UserId = hash(public_key)
Identity is stored locally only
No multi-device support
Security Model
Message body is encrypted
Metadata remains plaintext:
SenderId, ReceiverId, Timestamp, Topic
Encryption:
Per-message encryption using:
X25519 (ECDH) → shared key
Symmetric encryption (e.g., libsodium “box”)
Signing:
Each message is signed using sender’s private key
Signature verified on receipt
Message Visibility
All nodes store all messages
Each client filters locally:
User can only read messages where:
ReceiverId == current user
Sync Protocol
Peer-to-peer sync over HTTP or TCP
Each node acts as both client and server
Basic endpoints:
GET /messages?since=timestamp
POST /messages
Sync behavior:
Exchange missing messages
Use timestamp or cursor-based incremental sync
Deduplicate via Id (GUID)
Network Constraints Handling
Must work with:
High latency
Intermittent connectivity
Partial network partitions
Key mechanism:
Bridge users:
Sync inside restricted network
Later sync with external peers
Propagate data back
Performance Constraints
Optimize for low bandwidth:
Batch transfers
Compression (gzip)
Incremental sync only
SQLite is sufficient for storage
Design Principles
Keep system simple
Avoid blockchain / consensus
No global ordering required
Eventual consistency is acceptable
Prefer robustness over real-time behavior
What I need from you

Help me implement this step-by-step, including:

C# architecture and project structure
SQLite schema and indexing
Cryptography implementation (libsodium or equivalent)
Sync protocol design (API + message formats)
Conflict-free replication logic
Practical code examples

Keep answers technical, concise, and implementation-focused.

We go step by step. 
PROMPT END