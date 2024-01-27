using LogicReinc.BlendFarm.Client.Exceptions;
using LogicReinc.BlendFarm.Client.ImageTypes;
using LogicReinc.BlendFarm.ImageProcessing;
using LogicReinc.BlendFarm.Shared;
using LogicReinc.BlendFarm.Shared.Misc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LogicReinc.BlendFarm.Client.Tasks;
public class SplittedTask : RenderTask {
    private bool _isVertical;

    public SplittedTask(List<RenderNode> nodes, string session, string version, long fileId, RenderManagerSettings settings = null, bool vertical = false) : base(nodes, session, version, fileId, settings) {
        _isVertical = vertical;
    }

    protected override async Task<RenderTaskResult> Execute() {
        var imageProcessor = new ImageProcessor(Settings.OutputWidth, Settings.OutputHeight);

        try {
            Dictionary<RenderNode, RenderSubTask> assignment = GetSplitSubTasks(_usedNodes, _isVertical);

            int finished = 0;
            List<string> exceptions = new List<string>();

            var res = Parallel.ForEach(_usedNodes, (node) => {
                RenderSubTask subTask = assignment[node];
                if (subTask == null)
                    return;

                string lastException = null;
                bool rendered = false;
                for (int i = 0; i < 3; i++) {
                    if (Cancelled)
                        return;

                    try {
                        var subTaskResult = ExecuteSubTask(node, subTask);

                        if (!subTaskResult.IsSuccess) {
                            node.UpdateException($"[{i + 1}/3] " + subTaskResult.Message);
                            if (subTaskResult.Message != null)
                                lastException = subTaskResult.Message;

                            // Thread.Sleep(1000);
                            continue;
                        }
                    } catch (TaskCanceledException ex) {
                        if (Cancelled)
                            return;
                    } catch (RecoverException ex) {
                        node.UpdateException(ex.Message);
                        exceptions.Add(ex.Message);
                        lastException = ex.Message;
                        return;
                    } catch (Exception ex) {
                        node.UpdateException($"[{i + 1}/3] " + ex.Message);
                        lastException = ex.Message;
                        // Thread.Sleep(1000);
                        continue;
                    }

                    ProcessTile(subTaskResult, ref imageProcessor);

                    ChangeProgress(Progress + subTask.Value);

                    finished++;
                    rendered = true;
                    return;
                }

                if (!rendered && lastException != null)
                    exceptions.Add(lastException);

                if (finished != _usedNodes.Count)
                    throw new AggregateException("Not all tiles rendered", exceptions.Select(x => new Exception(x)));

                return;
            });
        }

        using (MemoryStream memoryStream = new()) {
            imageProcessor.Save(memoryStream);

            return new RenderTaskResult {
                IsSuccess = true,
                ImageData = memoryStream.ToArray(),
            };
        }
    }


    //SubTask Split
    /// <summary>
    /// Splits up file into subtasks among validNodes based on performance
    /// Single subtask per node (see RenderSplit description)
    /// </summary>
    private Dictionary<RenderNode, RenderSubTask> GetSplitSubTasks(List<RenderNode> validNodes, bool isVertical = false, decimal overlap = 0.01m) {
        Dictionary<RenderNode, decimal> shares = GetRelativePerformance(validNodes);

        Dictionary<RenderNode, RenderSubTask> tasks = new Dictionary<RenderNode, RenderSubTask>();
        decimal offsetX = 0;
        foreach (RenderNode node in validNodes) {
            decimal share = shares[node];

            if (node == validNodes.Last())
                share = 1 - offsetX;

            decimal startX = offsetX;
            decimal endX = offsetX + share;

            if (overlap > 0) {
                startX = Math.Max(0, startX - overlap);
                endX = Math.Min(1, endX + overlap);
            }

            if (!isVertical)
                tasks.Add(node, new RenderSubTask(this, startX, endX, 0, 1, Settings.Frame));
            else
                tasks.Add(node, new RenderSubTask(this, 0, 1, startX, endX, Settings.Frame));

            offsetX += share;
        }
        return tasks;
    }

