using System;

namespace Marketplace.SaaS.Accelerator.Services.Exceptions;

/// <summary>
/// Excepción personalizada para errores del Marketplace.
/// </summary>
public class MarketplaceException : ApplicationException
{
    /// <summary>
    /// Código de error opcional para categorizar el fallo.
    /// </summary>
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
