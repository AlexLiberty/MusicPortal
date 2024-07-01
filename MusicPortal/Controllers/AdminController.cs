using Microsoft.AspNetCore.Mvc;
using MusicPortal.Models.Repository;

namespace MusicPortal.Controllers
{
    public class AdminController : Controller
    {
        private readonly IUserRepository _userRepository;

        public AdminController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userRepository.GetAllUsers();
            var nonAdminUsers = users.Where(u => !u.IsAdmin);
            return View(nonAdminUsers);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmUser(int userId)
        {
            await _userRepository.ConfirmUser(userId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> BlockUser(int userId)
        {
            await _userRepository.BlockUser(userId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UnblockUser(int userId)
        {
            await _userRepository.UnblockUser(userId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            await _userRepository.DeleteUser(userId);
            return RedirectToAction("Index");
        }
    }
}
