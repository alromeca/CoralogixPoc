namespace CoralogixPoc.Enums;

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
