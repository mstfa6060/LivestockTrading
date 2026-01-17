using Common.Definitions.Base.Enums;

namespace Common.Contracts.Queue.Models;

public class ActivityLogModelContract
{
    // For Log
    public Guid RelationId { get; set; }
    public UserModules Module { get; set; }

    public ActionModel Action { get; set; }
    // For Action
    public class ActionModel
    {
        public LogSourcesEnumContract Source { get; set; }
        public Guid UserId { get; set; }
        public string ActionerName { get; set; }

        public LogActionTypesEnumContract ActionType { get; set; }
        public DateTime ActionDate { get; set; }
        public string Content { get; set; }
        public string Desctiption { get; set; }
    }
}

// Attention: Update Enums according to their original references


public enum LogSourcesEnumContract
{
    User = 0,
    System = 1,
    Application = 2,
}

public enum LogActionTypesEnumContract
{
    Create = 0,
    Update = 1,
    Delete = 2,
    Complete = 3,
    Approve = 4,
    Reject = 5,
    Match = 6,
    None = 7,
    Cancel = 8,
    Remove = 9,
    UnDelete = 10,
    Receive = 11,
    Transfer = 12,
    Submit = 13,
    Sign = 14,
    Send = 15,
    Read = 16,
    ManualCreate = 17,
    Start = 18,
    Action = 19,

}