# FirstSparrow 🕊️

**A Privacy-Preserving Ethereum Mixer Implementation**

FirstSparrow is a complete implementation of an anonymous transaction system inspired by Tornado Cash, built with .NET Core backend, smart contracts, and zero-knowledge proofs. It enables users to break the on-chain link between deposited and withdrawn funds, providing financial privacy on Ethereum.

## 📋 Table of Contents

- [Overview](#-overview)
- [How Anonymous Transactions Work](#-how-anonymous-transactions-work)
- [Architecture](#-architecture)
- [System Components](#-system-components)
- [Security & Privacy](#-security--privacy)
- [Installation & Setup](#-installation--setup)
- [Usage Guide](#-usage-guide)
- [API Documentation](#-api-documentation)
- [Development](#-development)
- [Contributing](#-contributing)

## 🎯 Overview

FirstSparrow implements a **mixer protocol** that allows users to deposit ETH into a smart contract and later withdraw it to a different address, breaking the on-chain connection between the two transactions. This is achieved through:

1. **Zero-Knowledge Proofs** - Prove ownership without revealing which deposit you own
2. **Merkle Trees** - Efficiently store and verify deposit commitments
3. **Relayer Infrastructure** - Enable anonymous withdrawals without gas fee linkage
4. **Backend Services** - Track blockchain state and provide merkle paths

### Key Features

- ✅ **Complete Privacy**: Break on-chain links between deposits and withdrawals
- ✅ **Zero-Knowledge Proofs**: Uses zk-SNARKs for cryptographic privacy guarantees
- ✅ **Relayer Support**: Optional relayer system for enhanced anonymity
- ✅ **Production Ready**: Full .NET backend with database persistence
- ✅ **CLI Tools**: Easy-to-use command-line interface
- ✅ **REST API**: Backend services for merkle path retrieval

## 🔒 How Anonymous Transactions Work

### The Privacy Problem

Traditional Ethereum transactions are completely transparent:

```
Address A → Smart Contract (Deposit 1 ETH)
Address B ← Smart Contract (Withdraw 1 ETH)
```

**Problem**: If Address B needs ETH for gas fees, where does it come from? This creates a traceable link.

### The FirstSparrow Solution

1. **Deposit Phase**: User generates a random secret and commits to it
2. **Mixing Phase**: Multiple users deposit, creating an anonymity set
3. **Withdrawal Phase**: User proves ownership without revealing which deposit
4. **Privacy Enhancement**: Optional relayer pays gas fees

```
    Deposit Phase                 Withdrawal Phase
┌─────────────────┐              ┌─────────────────┐
│ Generate Secret │              │ Generate Proof  │
│ Calculate Hash  │              │ Submit to Pool  │
│ Deposit to Pool │    ────→     │ Relayer Submits │
│ Join Anon Set   │              │ Funds to Target │
└─────────────────┘              └─────────────────┘
```

### Cryptographic Primitives

#### 1. Commitment Scheme
```
commitment = poseidon(nullifier, 0)
```
- **nullifier**: Secret number known only to depositor
- **0**: Fixed second parameter for deposits
- **poseidon**: Zero-knowledge friendly hash function

#### 2. Nullifier System
```
nullifierHash = poseidon(nullifier, 1, leafIndex)
```
- Prevents double-spending without revealing the original deposit
- Each nullifier can only be used once
- Links withdrawal to specific deposit position without revealing which

#### 3. Zero-Knowledge Proof
The zk-SNARK circuit proves:
- ✅ You know a secret (nullifier) committed to in the tree
- ✅ The commitment exists in the current merkle tree
- ✅ You haven't spent this nullifier before
- ✅ The withdrawal parameters (recipient, relayer, fee) are correct

**Without revealing**:
- ❌ Which specific deposit is yours
- ❌ Your original deposit address
- ❌ The secret nullifier value

## 🏗️ Architecture

FirstSparrow uses a layered architecture following Domain-Driven Design principles:

```
┌─────────────────────────────────────────────────────────┐
│                    Presentation Layer                    │
│  ┌─────────────────┐  ┌─────────────────┐              │
│  │   REST API      │  │   CLI Client    │              │
│  │ (ASP.NET Core)  │  │  (Node.js)      │              │
│  └─────────────────┘  └─────────────────┘              │
├─────────────────────────────────────────────────────────┤
│                   Application Layer                     │
│  ┌─────────────────┐  ┌─────────────────┐              │
│  │   MediatR       │  │   Services      │              │
│  │  (CQRS/MR)      │  │  (Business)     │              │
│  └─────────────────┘  └─────────────────┘              │
├─────────────────────────────────────────────────────────┤
│                   Infrastructure Layer                  │
│  ┌─────────────────┐  ┌─────────────────┐              │
│  │   Ethereum      │  │   Database      │              │
│  │  (Nethereum)    │  │ (PostgreSQL)    │              │
│  └─────────────────┘  └─────────────────┘              │
├─────────────────────────────────────────────────────────┤
│                      Domain Layer                       │
│  ┌─────────────────┐  ┌─────────────────┐              │
│  │   Entities      │  │   Services      │              │
│  │  (Core Logic)   │  │  (Domain)       │              │
│  └─────────────────┘  └─────────────────┘              │
└─────────────────────────────────────────────────────────┘
```

### Why This Architecture?

**Separation of Concerns**: Each layer has a single responsibility
- **Domain**: Core business logic and entities
- **Application**: Use cases and orchestration
- **Infrastructure**: External systems (blockchain, database)
- **Presentation**: User interfaces and APIs

**Testability**: Clean separation enables comprehensive unit testing

**Maintainability**: Changes in one layer don't cascade to others

**Scalability**: Can easily add new interfaces or swap implementations

## 🧩 System Components

### 1. Smart Contracts (Solidity)

#### **Grigali.sol** - Main Mixer Contract
```solidity
contract Grigali is MerkleTreeWithHistory, ReentrancyGuard {
    mapping(bytes32 => bool) public nullifierHashes;
    uint256 public denomination;
    
    function deposit(bytes32 _commitment) external payable;
    function withdraw(Proof memory _proof, bytes32 _root, bytes32 _nullifierHash, 
                     address payable _recipient, address payable _relayer, uint256 _fee) external;
}
```

**Key Features**:
- Fixed denomination deposits (e.g., 0.1 ETH)
- Merkle tree for efficient commitment storage
- Nullifier tracking to prevent double-spending
- Support for relayer fees

#### **Hasher.sol** - Poseidon Hash Contract
```solidity
contract Hasher {
    function poseidon(bytes32[2] calldata leftRight) public pure returns (bytes32);
}
```

### 2. Backend Services (.NET Core)

#### **FirstSparrow.Api** - REST API Layer
- Provides HTTP endpoints for client applications
- Handles deposit detail queries
- Manages request/response formatting
- Implements global exception handling

#### **FirstSparrow.Application** - Business Logic Layer
- **Deposit Service**: Processes new deposits and updates merkle tree
- **Metadata Service**: Manages blockchain sync state
- **Query Handlers**: Process deposit detail requests using MediatR

#### **FirstSparrow.Infrastructure** - External System Integration
- **Ethereum Service**: Interacts with smart contracts using Nethereum
- **Background Workers**: Continuously sync deposit events from blockchain
- **HTTP Client**: Manages RPC connections

#### **FirstSparrow.Persistence** - Data Access Layer
- **Repository Pattern**: Clean abstraction over database operations
- **Entity Framework**: Object-relational mapping
- **PostgreSQL**: Persistent storage for merkle tree and metadata

### 3. Database Schema

#### **merkle_nodes** Table
```sql
CREATE TABLE merkle_nodes (
    id SERIAL PRIMARY KEY,
    commitment VARCHAR(500) NOT NULL,
    index BIGINT NOT NULL,
    layer INTEGER NOT NULL,
    deposit_timestamp TIMESTAMPTZ DEFAULT NULL,
    create_timestamp TIMESTAMPTZ NOT NULL,
    update_timestamp TIMESTAMPTZ DEFAULT NULL,
    is_deleted BOOLEAN DEFAULT FALSE NOT NULL
);
```

**Purpose**: Stores the complete merkle tree structure
- **Layer 0**: Actual deposit commitments
- **Layer 1-20**: Intermediate and root nodes
- **Index**: Position within each layer

#### **metadata** Table
```sql
CREATE TABLE metadata (
    id SERIAL PRIMARY KEY,
    key VARCHAR(100) NOT NULL UNIQUE,
    value VARCHAR(500) DEFAULT NULL,
    create_timestamp TIMESTAMPTZ NOT NULL,
    update_timestamp TIMESTAMPTZ DEFAULT NULL,
    is_deleted BOOLEAN DEFAULT FALSE NOT NULL
);
```

**Purpose**: Tracks blockchain synchronization state
- Last processed block number
- Configuration parameters

### 4. Client Tools

#### **Grigali CLI** (Node.js)
Interactive command-line interface for:
- Making anonymous deposits
- Generating withdrawal proofs
- Submitting withdrawals
- Managing local deposit records

```bash
node grigali_client.js <private_key> <backend_url> <contract_address> <rpc_url>
```

## 🛡️ Security & Privacy

### Relayer System Deep Dive

#### Why Relayers Are Essential

**The Gas Payment Problem**:
```
Without Relayer:
1. User deposits from Address A
2. User withdraws to fresh Address B  
3. Address B needs ETH for gas fees
4. User sends gas ETH: A → B (PRIVACY BROKEN!)
```

**Relayer Solution**:
```
With Relayer:
1. User deposits from Address A
2. User generates proof offline
3. Relayer submits withdrawal transaction
4. Address B receives funds WITHOUT needing prior ETH
5. Relayer gets fee for service
```

#### How Relayers Work

1. **User generates proof** with specific parameters:
   ```javascript
   proof = generateProof({
     recipient: "0x...",    // Where funds go
     relayer: "0x...",      // Who gets fee  
     fee: "1000000000000000" // Amount in wei
   })
   ```

2. **Relayer verifies proof** and parameters match their service

3. **Smart contract validates** that proof was generated with correct parameters

4. **Funds are distributed**:
    - Recipient gets: `denomination - fee`
    - Relayer gets: `fee`

#### Why Relayers Cannot Cheat

The zero-knowledge proof includes **recipient, relayer, and fee** as public inputs:

```circom
component main {public [root,nullifierHash,recipient,relayer,fee]} = Withdraw(20);
```

**This means**:
- ✅ Proof is only valid for the exact recipient specified
- ✅ Proof is only valid for the exact relayer specified
- ✅ Proof is only valid for the exact fee amount specified
- ✅ Relayer cannot change any of these without invalidating the proof

**Cryptographic Guarantee**: If a relayer tries to send funds elsewhere or take a larger fee, the smart contract will reject the transaction because the proof won't verify.

### Privacy Guarantees

#### What FirstSparrow Hides
- ✅ **Link between deposits and withdrawals**: Cannot determine which deposit corresponds to which withdrawal
- ✅ **Identity of depositor**: Original deposit address is not revealed during withdrawal
- ✅ **Timing correlation**: Deposits and withdrawals can be separated by any amount of time
- ✅ **Amount obfuscation**: All deposits/withdrawals are the same denomination

#### What FirstSparrow Cannot Hide
- ❌ **Total contract activity**: Number of deposits and withdrawals is public
- ❌ **Timing analysis**: If you're the only person depositing/withdrawing, timing correlation is possible
- ❌ **IP address**: Use Tor or VPN when interacting with relayers
- ❌ **Browser fingerprinting**: Use privacy-focused browsers

### Anonymity Set Size

**Privacy scales with usage**:
- 10 deposits = choose from 10 possible sources
- 100 deposits = choose from 100 possible sources
- 1000 deposits = choose from 1000 possible sources

**Best practices**:
- Wait for more deposits before withdrawing
- Use different amounts of time between deposit/withdrawal
- Don't withdraw immediately after depositing

## 🚀 Installation & Setup

### Prerequisites

- **.NET 9.0 SDK** - For backend services
- **Node.js 18+** - For CLI client and circuit tools
- **PostgreSQL 14+** - For database
- **Docker & Docker Compose** - For containerized deployment

### 1. Clone Repository

```bash
git clone https://github.com/your-org/FirstSparrow.git
cd FirstSparrow
```

### 2. Client Setup

```bash
cd Grigali.Client
npm install

# Place your circuit files
cp path/to/withdraw.wasm .
cp path/to/circuit_final.zkey .
```

### 3. Run back-end with database
```bash
# Start everything
docker-compose up --build -d

# Check logs
docker-compose logs -f backend
```

## 📖 Usage Guide

### Making Your First Anonymous Transaction

#### Step 1: Deposit Funds

```bash
cd Grigali.Client
node grigali_client.js YOUR_PRIVATE_KEY http://localhost:5055 CONTRACT_ADDRESS RPC_URL
```

Follow the interactive prompts:
1. Select "Make Deposit"
2. Enter a random nullifier (remember this!)
3. Confirm the transaction

**Important**: Save your nullifier securely - you'll need it to withdraw!

#### Step 2: Wait for Anonymity

For maximum privacy, wait for other users to make deposits. The more deposits in the pool, the stronger your anonymity.

#### Step 3: Withdraw Anonymously

1. Select "Make Withdrawal"
2. Enter your nullifier
3. Enter recipient address (can be any address)
4. Confirm the transaction

The funds will be sent to the recipient address with no on-chain link to your original deposit!

### Using the REST API

#### Get Deposit Details
```bash
GET /api/v1/deposit/details/{commitment}
```

**Response**:
```json
{
  "path": ["0x1234...", "0x5678..."],
  "indices": [0, 1, 0, 1, ...],
  "root": "0xabcd...",
  "index": 42
}
```

This endpoint provides the merkle path needed for withdrawal proof generation.

### Advanced Usage

#### Custom Relayer Integration

```javascript
// Generate proof with relayer parameters
const proof = await generateProof({
  recipient: "0x...",           // Your target address
  relayer: "0x...",            // Relayer service address  
  fee: ethers.parseEther("0.01"), // Fee for relayer (1% of 1 ETH)
  // ... other parameters
});

// Submit to relayer service
await fetch('https://relayer.example.com/relay', {
  method: 'POST',
  body: JSON.stringify({
    proof,
    recipient,
    fee: "0.01"
  })
});
```

#### Batch Operations

```bash
# Multiple deposits with different nullifiers
for i in {1..10}; do
  node deposit.js $PRIVATE_KEY $CONTRACT $RPC $i
  sleep 30
done
```

### Project Structure

```
FirstSparrow/
├── FirstSparrow.Api/              # REST API & Controllers
├── FirstSparrow.Application/      # Business Logic & Use Cases
├── FirstSparrow.Infrastructure/   # External System Integration  
├── FirstSparrow.Persistence/     # Data Access & Repositories
├── FirstSparrow.Application.Tests/ # Unit Tests
├── Grigali.Client/               # Node.js CLI Client
└── docker-compose.yml            # Container Configuration
```

### Building from Source

```bash
# Restore dependencies
dotnet restore

# Build solution
dotnet build

# Run tests
dotnet test

# Run with hot reload
dotnet watch run --project FirstSparrow.Api
```

## ⚠️ Security Notice

**This software is provided for educational and research purposes.**

- Use at your own risk on testnets first
- Audit the code before mainnet deployment
- Ensure compliance with local regulations
- Privacy is not guaranteed against all attack vectors

## 🔗 Resources

- [Tornado Cash Research](https://tornado.cash/)
- [Zero-Knowledge Proofs](https://z.cash/technology/zksnarks/)
- [Merkle Trees](https://en.wikipedia.org/wiki/Merkle_tree)
- [Poseidon Hash Function](https://www.poseidon-hash.info/)

---

*Built with ❤️ for financial privacy and freedom*