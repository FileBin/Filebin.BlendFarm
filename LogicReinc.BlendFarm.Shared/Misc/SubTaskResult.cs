namespace LogicReinc.BlendFarm.Shared.Misc;

public struct SubTaskResult {

    public bool IsSuccess { get; init; }
    public string? Message { get; init; }

    public int X { get; init; }
    public int Y { get; init; }
    public byte[]? ImageData { get; init; }
}