﻿namespace DotNetInterview.API.Items;

public class ItemDTO
{
    public Guid Id { get; set; }
    public string Ref { get; set; }
    public string ItemName { get; set; }
    public string OriginalPrice { get; set; }
    public string CurrentPrice { get; set; }
    public string Status { get; set; }
    
}