
using DeltaFour.Domain.IRepositories;

namespace DeltaFour.Application.Service
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
    }
}