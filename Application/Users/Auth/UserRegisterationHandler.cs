using Domain.Entities;
using TanvirArjel.EFCore.GenericRepository;

namespace Application.Users.Auth
{
    public class UserRegisterationHandler
    {
        private readonly IRepository _repository;
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public UserRegisterationHandler(IRepository repository)
        {
            _repository = repository;
        }

        public async Task Before(UserRegisterationCommand command)
        {
            await _semaphore.WaitAsync();
            Console.WriteLine("lock applied");
        }

        public async Task<User> Handle(UserRegisterationCommand command)
        {
            User user = new User
            {
                UserName = command.UserName,
                Email = command.Email
            };

            await _repository.AddAsync(user);
            await _repository.SaveChangesAsync();

            return user;
        }

        public void Finally(UserRegisterationCommand command)
        {
            _semaphore.Release();
            Console.WriteLine("lock released");
        }
    }
}
