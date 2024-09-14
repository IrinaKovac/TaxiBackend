using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureInterface.DTO
{
    public interface IDTOConverter<TA, TM> where TA : class, ITableEntity
    {
        TM AzureToAppModel(TA azureModel);
        TA AppModelToAzure(TM appModel);
    }
}
