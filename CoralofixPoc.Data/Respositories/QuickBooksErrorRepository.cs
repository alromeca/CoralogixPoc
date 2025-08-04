using CoralogixPoc.Domain.Enums;
using CoralogixPoc.Domain.Models;

namespace CoralogixPoc.Data.Respositories;

public static class QuickBooksErrorRepository
{
    private static readonly Dictionary<QuickBooksErrorCode, ErrorInfo> _errorMap =
        new()
        {
                {
                    QuickBooksErrorCode.Error6190_816,
                    new ErrorInfo(
                        RootCause: "Another user opened the company file in single-user mode while you were also in single-user mode.",
                        RecoveryPath: "Switch all users to multi-user mode or have the other user close the file, then rescan the folder in the Database Server Manager."
                    )
                },
                {
                    QuickBooksErrorCode.Error6210_0,
                    new ErrorInfo(
                        RootCause: "A workstation is hosting the company file instead of the server.",
                        RecoveryPath: "On each workstation, disable “Host multi-user access” under File → Utilities so only the server hosts the company file."
                    )
                },
                {
                    QuickBooksErrorCode.Error195,
                    new ErrorInfo(
                        RootCause: "The company file’s backup or journal data is corrupted.",
                        RecoveryPath: "Run File → Utilities → Verify Data and then Rebuild Data if issues are found. If the problem persists, use the QuickBooks File Doctor."
                    )
                },
                {
                    QuickBooksErrorCode.Error15243,
                    new ErrorInfo(
                        RootCause: "The Intuit File Copy Service (FCS) is stopped or disabled, blocking program updates.",
                        RecoveryPath: "Open Services (services.msc), set “Intuit FCS” to Automatic or restart it, then retry the QuickBooks update."
                    )
                },
                {
                    QuickBooksErrorCode.H202,
                    new ErrorInfo(
                        RootCause: "Firewall, DNS, or network configuration is blocking multi-user access to the host computer.",
                        RecoveryPath: "In the QuickBooks Tool Hub go to Network Issues → Quick Fix my Network, then adjust your firewall/DNS settings or rerun the Database Server Manager."
                    )
                },
                {
                    QuickBooksErrorCode.UnrecoverableError,
                    new ErrorInfo(
                        RootCause: "QuickBooks closed unexpectedly (often due to too many open windows or data corruption).",
                        RecoveryPath: "Open the file with all windows closed (hold ALT when launching), try logging in as a different user, then run the QuickBooks File Doctor."
                    )
                },
                {
                    QuickBooksErrorCode.Error80070057,
                    new ErrorInfo(
                        RootCause: "Parameter is incorrect",
                        RecoveryPath: string.Empty
                    )
                }
        };

    public static bool TryGetErrorInfo(QuickBooksErrorCode code, out ErrorInfo info)
    {
        return _errorMap.TryGetValue(code, out info);
    }
}