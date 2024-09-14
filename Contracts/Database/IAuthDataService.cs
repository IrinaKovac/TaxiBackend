using Microsoft.ServiceFabric.Services.Remoting;
using Models.Auth;
using Models.UserTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Database
{
    [ServiceContract]
    public interface IAuthDataService : IService
    {
        [OperationContract]
        Task<bool> Exists(string partitionKey, string rowKey);

        [OperationContract]
        Task<bool> ExistsWithPwd(string partitionKey, string rowKey, string password);

        [OperationContract]
        Task<Models.Auth.UserProfile> GetUserProfile(string partitionKey, string rowKey);

        [OperationContract]
        Task<Models.Auth.UserProfile> UpdateUserProfile(UpdateUserProfileRequest request, string partitionKey, string rowKey);

        [OperationContract]
        Task<bool> CreateUser(UserProfile appModel);

        [OperationContract]
        Task<bool> ExistsSocialMediaAuth(string partitionKey, string rowKey);
    }
}
