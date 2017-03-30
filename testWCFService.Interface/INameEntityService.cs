using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using testUseDll.Complex;
using testWCFService.Interface;

namespace testWCFService.Interface
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface INameEntityService
    {
        [OperationContract]
        bool CreateNameEntityByType(int value, IntPtr namePtr);

        [OperationContract]
        bool CreateNameEntityByTypeByParam(int value, ref NameEntity nameEntity);

        [OperationContract]
        CompositeType GetDataUsingDataContract(CompositeType composite);

        // TODO: Add your service operations here
    }
}
