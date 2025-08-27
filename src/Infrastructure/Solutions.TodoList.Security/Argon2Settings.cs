using System.ComponentModel.DataAnnotations;

namespace Solutions.TodoList.Security;

public class Argon2Settings
{
    [Range(1, 10)]
    public int Iterations { get; set; } = 3;

    // Memory in KB. 65536 = 64MB
    [Range(1024, 1 << 20)]
    public int MemoryKb { get; set; } = 1 << 16;

    [Range(1, 16)]
    public int DegreeOfParallelism { get; set; } = 2;

    [Range(8, 64)]
    public int SaltSize { get; set; } = 16;

    [Range(16, 64)]
    public int HashSize { get; set; } = 32;
}