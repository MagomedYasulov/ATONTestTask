using ATONTestTask.ViewModels.Request;
using ATONTestTask.ViewModels.Resposne;

namespace ATONTestTask.Abstractions
{
    public interface IUsersService
    {
        public Task<UserExtendedDto> Create(CreateUsereDto model);
        public Task<UserExtendedDto> Get(UsersFilter filter);
        public Task<UserExtendedDto> Get(string login);
        public Task<UserDto> Update(Guid id, UpdateUserDto model);
        public Task<UserDto> UpdateLogin(Guid id, string login);
        public Task<UserDto> UpdatePassword(Guid id, string password);
        public Task<UserExtendedDto> Recover(Guid id);
        public Task Delete(Guid id, bool isSoft);
    }
}
