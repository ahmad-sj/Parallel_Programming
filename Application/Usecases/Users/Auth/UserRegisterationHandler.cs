using Domain.Entities;
using TanvirArjel.EFCore.GenericRepository;

namespace Application.Usecases.Users.Auth
{
    public class UserRegisterationHandler
    {
        private readonly IRepository _repository;
        //private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(2, 2);

        public UserRegisterationHandler(IRepository repository)
        {
            _repository = repository;
        }

        //public async Task Before(UserRegisterationCommand command)
        //{
        //    await _semaphore.WaitAsync();
        //    Helpers.PrintTimestamp("Semaphore Acquired");
        //}

        public async Task<User> Handle(UserRegisterationCommand command)
        {
            User user = new User
            {
                UserName = command.UserName,
                Email = command.Email
            };
            await _repository.AddAsync(user);
            await _repository.SaveChangesAsync();
            await Task.Delay(3000);
            Helpers.PrintTimestamp($"User '{user.Id}' Registered Successfully");
            return user;
        }

        //public async Task Finally(UserRegisterationCommand command)
        //{
        //    _semaphore.Release();
        //    Helpers.PrintTimestamp("Semaphore Released");
        //}
    }
}



