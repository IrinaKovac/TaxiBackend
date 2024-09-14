using Microsoft.ServiceFabric.Services.Remoting;
using Models.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Logic
{
    [ServiceContract]

    public interface IAuthLogic : IService
    {
        [OperationContract]
        Task<Tuple<bool, UserType>> Login(LoginData loginData);
        [OperationContract]
        Task<bool> Register(UserProfile userProfile);
        [OperationContract]
        Task<UserProfile> GetUserProfile(string userEmail, UserType userType);
        [OperationContract]
        Task<UserProfile> UpdateUserProfile(UpdateUserProfileRequest updateUserProfileRequest, string userEmail, UserType userType);
    }
}
