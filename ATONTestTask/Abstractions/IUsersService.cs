using ATONTestTask.ViewModels.Request;
using ATONTestTask.ViewModels.Resposne;

namespace ATONTestTask.Abstractions
{
    public interface IUsersService
    {
        public Task<UserExtendedDto> Create(CreateUsereDto model);
        public Task<UserExtendedDto[]> Get(UsersFilter filter);
        public Task<UserExtendedDto> Get(string login);
        public Task<UserDto> Update(string login, UpdateUserDto model);
        public Task<UserDto> UpdateLogin(string login, string newLogin);
        public Task<UserDto> UpdatePassword(string login, string password);
        public Task<UserExtendedDto> Recover(string login);
        public Task Delete(string login, bool isSoft);
    }
}
