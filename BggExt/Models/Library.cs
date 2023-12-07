using System;
using System.ComponentModel.DataAnnotations;

namespace BggExt.Models;

public class Library
{
    [Key] public string Id { get; set; } = default!;

    [DataType(DataType.DateTime)] public DateTime? LastSynchronized { get; set; }
}