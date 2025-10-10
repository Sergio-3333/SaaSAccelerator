using System;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;

namespace Marketplace.SaaS.Accelerator.DataAccess.Contracts;

public interface ILicensesRepository
{

    int CreateLicense(Licenses license);



    void UpdateLicense(string microsoftId, Action<Licenses> updateAction);


    Licenses GetById(int licenseId);



    Licenses GetLicenseByMicrosoftId(string microsoftId);


    Licenses GetByEmail(string email);

    bool ExistsLicenseId(int id);
    bool ExistsLicenseKey(string key);


}
