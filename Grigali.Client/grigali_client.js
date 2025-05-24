const { ethers } = require("ethers");
const { buildPoseidon } = require("circomlibjs");
const snarkjs = require("snarkjs");
const fs = require("fs");
const path = require("path");
const readline = require("readline");

/**
 * Grigali CLI Tool
 * A complete command-line interface for interacting with Grigali mixer
 *
 * Usage: node grigali_client.js <private_key> <backend_url> <contract_address> <rpc_url>
 */

class GrigaliCLI {
  constructor(privateKey, backendUrl, contractAddress, rpcUrl) {
    this.privateKey = privateKey.startsWith("0x")
      ? privateKey
      : "0x" + privateKey;
    this.backendUrl = backendUrl.endsWith("/")
      ? backendUrl.slice(0, -1)
      : backendUrl;
    this.contractAddress = contractAddress;
    this.rpcUrl = rpcUrl;

    // Initialize ethers components
    this.provider = new ethers.JsonRpcProvider(rpcUrl);
    this.wallet = new ethers.Wallet(this.privateKey, this.provider);

    // Contract ABI
    this.contractABI = [
      "function deposit(bytes32 _commitment) external payable",
      "function withdraw(tuple(uint256[2] a, uint256[2][2] b, uint256[2] c) _proof, bytes32 _root, bytes32 _nullifierHash, address _recipient, address _relayer, uint256 _fee) external payable",
      "function denomination() external view returns (uint256)",
      "function isSpent(bytes32 _nullifierHash) external view returns (bool)",
      "function isKnownRoot(bytes32 _root) external view returns (bool)",
    ];

    this.contract = new ethers.Contract(
      contractAddress,
      this.contractABI,
      this.wallet,
    );
    this.poseidon = null;

    // Circuit files
    this.circuitWasm = "withdraw.wasm";
    this.circuitZkey = "circuit_final.zkey";

    console.log("🚀 Grigali CLI Tool Initialized");
    console.log(`📍 Contract: ${contractAddress}`);
    console.log(`🌐 RPC: ${rpcUrl}`);
    console.log(`🔗 Backend: ${backendUrl}`);
    console.log(`👤 Address: ${this.wallet.address}`);
  }

  async initialize() {
    try {
      console.log("\n⏳ Initializing Poseidon hash function...");
      this.poseidon = await buildPoseidon();

      console.log("⏳ Fetching contract information...");
      const denomination = await this.contract.denomination();
      this.denomination = denomination;

      const balance = await this.provider.getBalance(this.wallet.address);

      console.log(`💰 Denomination: ${ethers.formatEther(denomination)} ETH`);
      console.log(`💳 Your Balance: ${ethers.formatEther(balance)} ETH`);
      console.log("✅ Initialization complete!\n");

      return true;
    } catch (error) {
      console.error("❌ Initialization failed:", error.message);
      return false;
    }
  }

  async showMenu() {
    const rl = readline.createInterface({
      input: process.stdin,
      output: process.stdout,
    });

    const question = (prompt) =>
      new Promise((resolve) => rl.question(prompt, resolve));

    while (true) {
      console.log("\n═══════════════════════════════════════");
      console.log("🎭 GRIGALI MIXER CLI");
      console.log("═══════════════════════════════════════");
      console.log("1. 📥 Make Deposit");
      console.log("2. 📤 Make Withdrawal");
      console.log("3. 📊 Check Balance");
      console.log("4. 🔍 Check Deposit Status");
      console.log("5. ℹ️  Show Contract Info");
      console.log("6. 📋 List My Deposits");
      console.log("7. ❌ Exit");
      console.log("═══════════════════════════════════════");

      const choice = await question("Select option (1-7): ");

      try {
        switch (choice.trim()) {
          case "1":
            await this.makeDeposit();
            break;
          case "2":
            await this.makeWithdrawal();
            break;
          case "3":
            await this.checkBalance();
            break;
          case "4":
            await this.checkDepositStatus();
            break;
          case "5":
            await this.showContractInfo();
            break;
          case "6":
            await this.listMyDeposits();
            break;
          case "7":
            console.log("👋 Goodbye!");
            rl.close();
            return;
          default:
            console.log("❌ Invalid option. Please select 1-7.");
        }
      } catch (error) {
        console.error("❌ Error:", error.message);
      }
    }
  }

  async makeDeposit() {
    console.log("\n📥 MAKING DEPOSIT");
    console.log("─────────────────");

    const rl = readline.createInterface({
      input: process.stdin,
      output: process.stdout,
    });

    const question = (prompt) =>
      new Promise((resolve) => rl.question(prompt, resolve));

    try {
      const nullifierStr = await question("Enter nullifier (number): ");
      const nullifier = BigInt(nullifierStr);

      console.log("\n⏳ Generating commitment...");
      const commitment = this.poseidon([nullifier, BigInt(0)]);
      const commitmentHex = this.toHex32(commitment);

      console.log(`🔐 Commitment: ${commitmentHex}`);
      console.log(`💰 Amount: ${ethers.formatEther(this.denomination)} ETH`);

      const confirm = await question("\nConfirm deposit? (y/N): ");
      if (confirm.toLowerCase() !== "y") {
        console.log("❌ Deposit cancelled");
        rl.close();
        return;
      }

      console.log("\n⏳ Sending deposit transaction...");
      const tx = await this.contract.deposit(commitmentHex, {
        value: this.denomination,
      });

      console.log(`📋 Transaction sent: ${tx.hash}`);
      console.log("⏳ Waiting for confirmation...");

      const receipt = await tx.wait();
      console.log(`✅ Deposit confirmed in block ${receipt.blockNumber}`);

      // Save deposit info
      const depositInfo = {
        nullifier: nullifier.toString(),
        commitment: commitmentHex,
        txHash: tx.hash,
        blockNumber: receipt.blockNumber,
        timestamp: new Date().toISOString(),
        amount: ethers.formatEther(this.denomination),
      };

      this.saveDeposit(depositInfo);
      console.log(`💾 Deposit saved to deposits.json`);
      console.log(`🔑 IMPORTANT: Remember your nullifier: ${nullifier}`);
    } catch (error) {
      console.error("❌ Deposit failed:", error.message);
    } finally {
      rl.close();
    }
  }

  async makeWithdrawal() {
    console.log("\n📤 MAKING WITHDRAWAL");
    console.log("─────────────────────");

    const rl = readline.createInterface({
      input: process.stdin,
      output: process.stdout,
    });

    const question = (prompt) =>
      new Promise((resolve) => rl.question(prompt, resolve));

    try {
      const nullifierStr = await question("Enter nullifier for withdrawal: ");
      const recipientAddress = await question(
        "Enter recipient address (press Enter for your address): ",
      );

      const nullifier = BigInt(nullifierStr);
      const recipient = recipientAddress.trim() || this.wallet.address;

      console.log("\n⏳ Calculating commitment...");
      const commitment = this.poseidon([nullifier, BigInt(0)]);
      const commitmentHex = this.toHex32(commitment);

      console.log(`🔐 Commitment: ${commitmentHex}`);
      console.log(`📤 Recipient: ${recipient}`);

      console.log("\n⏳ Fetching merkle path from backend...");
      const merkleData = await this.getMerklePath(commitmentHex);

      console.log("⏳ Calculating nullifier hash...");
      const nullifierHash = this.poseidon([
        nullifier,
        BigInt(1),
        BigInt(merkleData.Index),
      ]);
      const nullifierHashHex = this.toHex32(nullifierHash);

      console.log("⏳ Checking if already spent...");
      const isSpent = await this.contract.isSpent(nullifierHashHex);
      if (isSpent) {
        throw new Error("This nullifier has already been spent!");
      }

      console.log("⏳ Verifying root...");
      const isValidRoot = await this.contract.isKnownRoot(merkleData.Root);
      if (!isValidRoot) {
        throw new Error("Invalid merkle root!");
      }

      const confirm = await question("\nConfirm withdrawal? (y/N): ");
      if (confirm.toLowerCase() !== "y") {
        console.log("❌ Withdrawal cancelled");
        rl.close();
        return;
      }

      console.log("\n⏳ Generating zero-knowledge proof...");
      const proof = await this.generateProof({
        root: merkleData.Root,
        nullifierHash: nullifierHashHex,
        recipient: recipient,
        relayer: ethers.ZeroAddress,
        fee: "0",
        nullifier: nullifier.toString(),
        pathElements: merkleData.Path,
        pathIndices: merkleData.Indices,
      });

      console.log("⏳ Submitting withdrawal transaction...");
      const tx = await this.contract.withdraw(
        proof,
        merkleData.Root,
        nullifierHashHex,
        recipient,
        ethers.ZeroAddress,
        0,
      );

      console.log(`📋 Transaction sent: ${tx.hash}`);
      console.log("⏳ Waiting for confirmation...");

      const receipt = await tx.wait();
      console.log(`✅ Withdrawal confirmed in block ${receipt.blockNumber}`);
      console.log(`💰 Funds sent to: ${recipient}`);
    } catch (error) {
      console.error("❌ Withdrawal failed:", error.message);
    } finally {
      rl.close();
    }
  }

  async checkBalance() {
    console.log("\n💳 BALANCE CHECK");
    console.log("─────────────────");

    const balance = await this.provider.getBalance(this.wallet.address);
    console.log(`Address: ${this.wallet.address}`);
    console.log(`Balance: ${ethers.formatEther(balance)} ETH`);
  }

  async checkDepositStatus() {
    console.log("\n🔍 CHECK DEPOSIT STATUS");
    console.log("───────────────────────");

    const rl = readline.createInterface({
      input: process.stdin,
      output: process.stdout,
    });

    const question = (prompt) =>
      new Promise((resolve) => rl.question(prompt, resolve));

    try {
      const commitmentHex = await question("Enter commitment hash: ");

      console.log("⏳ Checking backend...");
      const response = await fetch(
        `${this.backendUrl}/api/v1/deposit/details/${commitmentHex}`,
      );

      if (response.ok) {
        const data = await response.json();
        console.log("✅ Deposit found in backend");
        console.log(`Index: ${data.Index}`);
        console.log(`Root: ${data.Root}`);
        console.log(`Path Elements: ${data.Path.length}`);
      } else {
        console.log("❌ Deposit not found in backend");
      }
    } catch (error) {
      console.error("❌ Error checking deposit:", error.message);
    } finally {
      rl.close();
    }
  }

  async showContractInfo() {
    console.log("\nℹ️  CONTRACT INFORMATION");
    console.log("──────────────────────────");

    try {
      const denomination = await this.contract.denomination();
      const balance = await this.provider.getBalance(this.contractAddress);

      console.log(`Contract Address: ${this.contractAddress}`);
      console.log(`Denomination: ${ethers.formatEther(denomination)} ETH`);
      console.log(`Contract Balance: ${ethers.formatEther(balance)} ETH`);
      console.log(`RPC URL: ${this.rpcUrl}`);
      console.log(`Backend URL: ${this.backendUrl}`);
    } catch (error) {
      console.error("❌ Error fetching contract info:", error.message);
    }
  }

  async listMyDeposits() {
    console.log("\n📋 MY DEPOSITS");
    console.log("──────────────");

    try {
      const deposits = this.loadDeposits();

      if (deposits.length === 0) {
        console.log("No deposits found.");
        return;
      }

      deposits.forEach((deposit, index) => {
        console.log(`\n${index + 1}. Deposit:`);
        console.log(`   Nullifier: ${deposit.nullifier}`);
        console.log(`   Commitment: ${deposit.commitment}`);
        console.log(`   Amount: ${deposit.amount} ETH`);
        console.log(`   Tx Hash: ${deposit.txHash}`);
        console.log(`   Date: ${new Date(deposit.timestamp).toLocaleString()}`);
      });
    } catch (error) {
      console.error("❌ Error loading deposits:", error.message);
    }
  }

  async getMerklePath(commitment) {
    const response = await fetch(
      `${this.backendUrl}/api/v1/deposit/details/${commitment}`,
    );

    if (!response.ok) {
      throw new Error(
        `Backend request failed: ${response.status} ${response.statusText}`,
      );
    }

    return await response.json();
  }

  async generateProof(inputs) {
    if (!fs.existsSync(this.circuitWasm) || !fs.existsSync(this.circuitZkey)) {
      throw new Error(
        `Circuit files not found. Please ensure ${this.circuitWasm} and ${this.circuitZkey} are in the current directory.`,
      );
    }

    const { proof, publicSignals } = await snarkjs.groth16.fullProve(
      inputs,
      this.circuitWasm,
      this.circuitZkey,
    );

    return {
      a: [proof.pi_a[0], proof.pi_a[1]],
      b: [
        [proof.pi_b[0][1], proof.pi_b[0][0]],
        [proof.pi_b[1][1], proof.pi_b[1][0]],
      ],
      c: [proof.pi_c[0], proof.pi_c[1]],
    };
  }

  toHex32(value) {
    const hex = this.poseidon.F.toString(value, 16);
    return "0x" + hex.padStart(64, "0");
  }

  saveDeposit(depositInfo) {
    const depositsFile = "deposits.json";
    let deposits = [];

    if (fs.existsSync(depositsFile)) {
      deposits = JSON.parse(fs.readFileSync(depositsFile, "utf8"));
    }

    deposits.push(depositInfo);
    fs.writeFileSync(depositsFile, JSON.stringify(deposits, null, 2));
  }

  loadDeposits() {
    const depositsFile = "deposits.json";

    if (!fs.existsSync(depositsFile)) {
      return [];
    }

    return JSON.parse(fs.readFileSync(depositsFile, "utf8"));
  }
}

// Main execution
async function main() {
  console.log("🎭 Grigali Mixer CLI Tool");
  console.log("═══════════════════════════\n");

  // Check command line arguments
  if (process.argv.length !== 6) {
    console.log(
      "Usage: node grigali_client.js <private_key> <backend_url> <contract_address> <rpc_url>",
    );
    console.log("\nExample:");
    console.log(
      "node grigali_client.js 0x1234... http://localhost:5055 0xabc123... https://rpc.sepolia.org",
    );
    process.exit(1);
  }

  const [, , privateKey, backendUrl, contractAddress, rpcUrl] = process.argv;

  try {
    const cli = new GrigaliCLI(privateKey, backendUrl, contractAddress, rpcUrl);

    const initialized = await cli.initialize();
    if (!initialized) {
      process.exit(1);
    }

    await cli.showMenu();
  } catch (error) {
    console.error("❌ Fatal error:", error.message);
    process.exit(1);
  }
}

// Handle process termination
process.on("SIGINT", () => {
  console.log("\n👋 Exiting...");
  process.exit(0);
});

// Run the CLI
if (require.main === module) {
  main().catch(console.error);
}

module.exports = { GrigaliCLI };
