using pol.Core.Models;
using pol.Crypto;
using pol.Data.Repositories;
using pol.Services;
using Pol.Data;
using System.Linq;
using System.Security.Principal;
using System.Text.Json;


namespace pol
{
    public partial class Form1 : Form
    {
        private UserIdentity _myIdentity; //Add a Class-Level Variable

        public Form1()
        {
            InitializeComponent();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {

            Sodium.SodiumCore.Init(); // Always initialize Sodium first

            try
            {
                Sodium.SodiumCore.Init();

                // 1. Setup Database
                string dbPath = "pol_data.db";
                using var dbContext = new DatabaseContext(dbPath);
                await dbContext.InitializeAsync();

                // 2. Load or Create Identity (Crucial for both PCs)
                _myIdentity = LoadMyIdentity("me.json");
                if (_myIdentity == null)
                {
                    _myIdentity = CryptoService.GenerateIdentity();
                    SaveMyIdentity(_myIdentity, "me.json");
                    MessageBox.Show("New Identity Created!");
                }
                else
                {
                    MessageBox.Show($"Welcome back, User ID: {_myIdentity.UserId.Substring(0, 8)}...");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Startup Error: " + ex.Message);
            }

        }

        private async void button1_Click(object sender, EventArgs e)
        {


            try
            {
                Sodium.SodiumCore.Init();

                // 1. Setup DB
                string dbPath = "pol_data.db";
                using var dbContext = new DatabaseContext(dbPath);
                await dbContext.InitializeAsync();
                var repo = new NetMessageRepository(dbContext);

                // 2. Validate Input
                string receiverPubKeyHex = txtReceiverKey.Text.Trim().Replace(" ", "").Replace("\n", "").Replace("\r", "");

                if (string.IsNullOrEmpty(receiverPubKeyHex))
                {
                    MessageBox.Show("Please paste the Receiver's Public Key in the box first!");
                    return;
                }

                // 3. Create Message (Using YOUR identity, encrypting for RECEIVER)
                var newMessage = MessagingService.CreateMessage(
                    topic: "Hello from PC B",
                    body: "B: This is a test message across the network! From B",
                    receiverPublicKeyHex: receiverPubKeyHex,
                    messageType: 0,
                    senderIdentity: _myIdentity // Use the identity loaded in Form_Load
                );

                // 4. Save to Local DB
                await repo.AddMessageAsync(newMessage);
                MessageBox.Show("Message created and saved locally!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {

            try
            {
                Sodium.SodiumCore.Init();
                string dbPath = "pol_data.db";
                using var dbContext = new DatabaseContext(dbPath);
                await dbContext.InitializeAsync();
                var repo = new NetMessageRepository(dbContext);

                // 1. Load our Identity (Receiver)
                UserIdentity myIdentity = LoadMyIdentity("me.json");
                if (myIdentity == null) { MessageBox.Show("No identity!"); return; }

                // 2. Get the message
                var allMessages = await repo.GetAllMessagesAsync();
                var lastMessage = allMessages.LastOrDefault();

                if (lastMessage != null)
                {
                    // 3. Get SENDER's Key from the textbox
                    string senderPubKeyHex = txtSenderKey.Text.Trim().Replace(" ", "").Replace("\n", "").Replace("\r", "");

                    if (string.IsNullOrEmpty(senderPubKeyHex))
                    {
                        MessageBox.Show("Please paste the SENDER's Public Key to decrypt/verify this message.");
                        return;
                    }

                    byte[] senderPubKey = Convert.FromHexString(senderPubKeyHex);

                    // 4. Read (using our identity + sender's key)
                    string content = MessagingService.ReadMessage(
                        message: lastMessage,
                        recipientIdentity: myIdentity,
                        senderSigningPublicKey: senderPubKey, // Use the key from textbox
                        senderEncryptionPublicKey: senderPubKey // Use the key from textbox
                    );

                    MessageBox.Show($"Success!\nTopic: {lastMessage.Topic}\nContent: {content}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Read Error: " + ex.Message);
            }

        }
        //--
        private void SaveMyIdentity(UserIdentity identity, string filename)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(identity, options);
            File.WriteAllText(filename, json);
        }

        private UserIdentity LoadMyIdentity(string filename)
        {
            if (!File.Exists(filename)) return null;

            string json = File.ReadAllText(filename);
            return JsonSerializer.Deserialize<UserIdentity>(json);
        }

        private async void btnStartServer_Click(object sender, EventArgs e)
        {
            try
            {
                Sodium.SodiumCore.Init();
                string dbPath = "pol_data.db";
                using var dbContext = new DatabaseContext(dbPath);
                await dbContext.InitializeAsync();
                var repo = new NetMessageRepository(dbContext);

                // Use the IP from the textbox
                
                string myIp = txtMyIP.Text.Trim();
                if (string.IsNullOrEmpty(myIp)) myIp = "localhost";

                // DEBUG: Show exactly what we are binding to
                MessageBox.Show($"I am trying to bind to: http://{myIp}:5000/");

                var server = new pol.Network.P2PServer(repo);
                server.Start(myIp, 5000);


                btnStartServer.Text = "Server Running";
                btnStartServer.Enabled = false;
                MessageBox.Show($"Server Started on http://{myIp}:5000");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Server Error: " + ex.Message);
            }
        }

        private async void btnSync_Click(object sender, EventArgs e)
        {
            try
            {
                Sodium.SodiumCore.Init();
                string dbPath = "pol_data.db";
                using var dbContext = new DatabaseContext(dbPath);
                await dbContext.InitializeAsync();
                var repo = new NetMessageRepository(dbContext);

                string targetUrl = txtTargetIP.Text.Trim();
                if (string.IsNullOrEmpty(targetUrl))
                {
                    MessageBox.Show("Enter a Target IP (e.g., 192.168.1.5)");
                    return;
                }

                // Ensure URL format
                if (!targetUrl.StartsWith("http"))
                {
                    targetUrl = $"http://{targetUrl}:5000";
                }

                var client = new pol.Network.P2PClient(repo);

                MessageBox.Show("Starting Sync...");
                await client.SyncAsync(targetUrl);

                MessageBox.Show("Sync Complete!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sync Error: " + ex.Message);
            }
        }

        private void btnShowKey_Click(object sender, EventArgs e)
        {

            if (_myIdentity == null)
            {
                MessageBox.Show("Identity not loaded yet.");
                return;
            }

            string pubKeyHex = Convert.ToHexString(_myIdentity.EncryptionPublicKey);
            string userId = _myIdentity.UserId;

            // Copy to clipboard for easy pasting
            Clipboard.SetText(pubKeyHex);

            MessageBox.Show(
                $"Your User ID: {userId}\n\n" +
                $"Your Public Key (copied to clipboard):\n{pubKeyHex}"
            );
        }
        // ... inside Form1 class ...

    }
}
