using System.Collections.Generic;
using System.Xml.Linq;

namespace BggExt.XmlApi2;

public class ApiResult
{
    public enum OperationStatus
    {
        Success,
        Pending,
        Error
    }

    public OperationStatus Status
    {
        get
        {
            if (Errors.Count != 0)
                return OperationStatus.Error;
            else if (!string.IsNullOrEmpty(Message)) return OperationStatus.Pending;
            return OperationStatus.Success;
        }
    }

    public XElement? Xml { get; set; }
    public object? Data { get; set; }
    public string Message { get; set; } = "";
    public IList<string> Errors { get; set; } = new List<string>();
}