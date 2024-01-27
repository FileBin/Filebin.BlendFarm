using LogicReinc.BlendFarm.Shared.Communication.RenderNode;

namespace LogicReinc.BlendFarm.Shared.Misc;

public struct SubTaskBatchResult {

    public bool IsSuccess { get; init; }
    public string? Message { get; init; }
    public RenderBatchResult[]? Results { get; init; }
}