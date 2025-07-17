namespace CoralogixPoc.Domain.Enums;

public enum EntityType
{
    Customer = 1,
    Invoice = 2,
    Item = 3,
    Quote = 4
}

public enum AccountingSystem
{
    QBO = 1,
    QBD = 2
}

public enum QuickBooksErrorCode
{
    Error6190_816,
    Error6210_0,
    Error195,
    Error15243,
    H202,
    UnrecoverableError
}

public enum ErrorType
{
    InvalidOperation = 1,
    Argument = 2,
    NullReference = 3,
    TimeOut = 4,
    Default = 5
}

public enum CompanyName
{
    AcmeCorp,
    GlobexInc,
    Initech,
    UmbrellaLLC,
    StarkIndustries
}