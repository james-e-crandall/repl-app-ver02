namespace   ReplLib.Pipelines;
public sealed partial class ReplicationContext
{
    public ReplState CurrentReplState { get; set; } = ReplState.None;
}