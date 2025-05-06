//role of this file
//MessageParams is a helper class used to pass username, message category (e.g. Unread), and pagination info when querying messages.

using System;

namespace API.Helpers;

public class MessageParams : PaginationParams
{
    public string? Username { get; set; }
    public string Container { get; set; } = "Unread";
}
