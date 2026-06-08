using Domain.Entities;
using TanvirArjel.EFCore.GenericRepository;

namespace Application.Usecases.Users.Auth
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
            //Helpers.PrintTimestamp("==================== Before User Regsiteration ====================");

            //await _semaphore.WaitAsync();
            //Helpers.PrintTimestamp("Slim Semaphore Acquired");
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

            Helpers.PrintTimestamp($"User '{user.SeqId}' Registered Successfully");

            return user;
        }

        public async Task Finally(UserRegisterationCommand command)
        {
            //Helpers.PrintTimestamp("==================== After User Regsiteration =====================");

            //_semaphore.Release();
            //Helpers.PrintTimestamp("Slim Semaphore Released");
        }
    }
}
