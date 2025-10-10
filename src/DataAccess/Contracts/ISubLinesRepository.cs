using Marketplace.SaaS.Accelerator.DataAccess.Entities;

namespace Marketplace.SaaS.Accelerator.DataAccess.Contracts;

public interface ISubLinesRepository 
{

   SubLines GetByMicrosoftId(string microsoftId);


    int AddNewLine(SubLines subLine);
}
