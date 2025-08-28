using System;

namespace Marketplace.SaaS.Accelerator.DataAccess.Entities;

public partial class Products
{
    public int ProductID { get; set; }

    public string HostApplicationName { get; set; }

    public string ProductName { get; set; }

    public string ProductInfoURL { get; set; }

    public string ProductDownloadURL { get; set; }
}
