﻿namespace EntwineBackend.Authentication
{
    public class RefreshResult
    {
        public string? Username { get; set; }
        public string? AccessToken { get; set; }
        public int? UserId { get; set; }
    }
}
