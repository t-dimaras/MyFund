using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace MyFund.DataModel
{
    public interface IResource
    {
        long GetResourceOwnerId();
    }
}
