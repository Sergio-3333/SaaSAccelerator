using System;

namespace Marketplace.SaaS.Accelerator.Services.Exceptions;


public class MarketplaceException : ApplicationException
{

    public string ErrorCode { get; set; }

    public MarketplaceException() : base() { }

    public MarketplaceException(string message) : base(message) { }

    public MarketplaceException(string message, string errorCode) : base(message)
    {
        this.ErrorCode = errorCode;
    }

    public MarketplaceException(string message, Exception inner) : base(message, inner) { }

    public MarketplaceException(string message, string errorCode, Exception inner) : base(message, inner)
    {
        this.ErrorCode = errorCode;
    }
}
