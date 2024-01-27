using System;
using System.IO;

namespace LogicReinc.BlendFarm.Shared.Misc;

//TODO: add node info to struct
public struct RenderTaskResult {
    public bool IsSuccess { get; init; }
    public string? Message { get; init; }

    public byte[]? ImageData { get; init; }

    public void Save(string path) {
        if (IsSuccess) {
            if (ImageData == null)
                throw new NullReferenceException("ImageData was null when saving!");
            File.WriteAllBytes(path, ImageData);
        }
        throw new InvalidOperationException($"Image can't be saved because task result not succeed!\nResultMessage:\n\t{Message}");
    }

    public static RenderTaskResult AlreadyRendering() {
        return new RenderTaskResult {
            IsSuccess = false,
            Message = "Already rendering..",
        };
    }

    public static RenderTaskResult NoReadyNodes() {
        return new RenderTaskResult {
            IsSuccess = false,
            Message = "No ready nodes available..",
        };
    }
}
