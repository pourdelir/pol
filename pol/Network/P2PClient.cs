using pol.Core.Models;
using pol.Data.Repositories;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace pol.Network
{
    public class P2PClient
    {
        private readonly HttpClient _httpClient;
        private readonly INetMessageRepository _repo;

        public P2PClient(INetMessageRepository repo)
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(30); // Timeout for slow networks
            _repo = repo;
        }

        /// <summary>
        /// Connects to a peer and downloads missing messages in batches.
        /// </summary>
        

        public async Task SyncAsync(string peerUrl)
        {
            if (!peerUrl.EndsWith("/")) peerUrl += "/";

            // --- PHASE 1: UPLOAD (Push) ---
            // Get all messages from OUR local database

            var localMessages = await _repo.GetAllMessagesAsync();

            foreach (var msg in localMessages)
            {
                try
                {
                    var dto = MessageMapper.ToDto(msg);
                    var json = JsonSerializer.Serialize(dto);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    // Send POST to the peer
                    var response = await _httpClient.PostAsync($"{peerUrl}messages", content);

                    // We don't stop if one fails, just log it and continue
                    if (!response.IsSuccessStatusCode)
                    {
                        System.Diagnostics.Debug.WriteLine($"Failed to upload msg {msg.Id}: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Upload Error: {ex.Message}");
                }
            }

            // --- PHASE 2: DOWNLOAD (Pull) ---
            // (This is the existing logic to fetch missing messages)

            // 1. Find the latest timestamp we have locally
            // We assume local DB is ordered, so we get the max timestamp.
            // If DB is empty, we start from 0.

            var allLocal = await _repo.GetAllMessagesAsync();
            long lastTimestamp = allLocal.Any() ? allLocal.Max(m => m.TimestampUnix) : 0;
            int totalDownloaded = 0;
            int batchSize = 100;
            bool hasMore = true;

            while (hasMore)
            {
                try
                {
                    string requestUrl = $"{peerUrl}messages?since={lastTimestamp}&limit={batchSize}";
                    var response = await _httpClient.GetAsync(requestUrl);

                    if (!response.IsSuccessStatusCode) break;

                    string json = await response.Content.ReadAsStringAsync();
                    var dtos = JsonSerializer.Deserialize<List<MessageDto>>(json);

                    if (dtos == null || dtos.Count == 0)
                    {
                        hasMore = false;
                        break;
                    }

                    foreach (var dto in dtos)
                    {
                        var msg = MessageMapper.FromDto(dto);
                        await _repo.AddMessageAsync(msg); // Uses INSERT OR IGNORE

                        if (msg.TimestampUnix > lastTimestamp)
                        {
                            lastTimestamp = msg.TimestampUnix;
                        }
                    }

                    totalDownloaded += dtos.Count;
                    if (dtos.Count < batchSize) hasMore = false;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Download Error: {ex.Message}");
                    throw;
                }
            }
        }
    }
}