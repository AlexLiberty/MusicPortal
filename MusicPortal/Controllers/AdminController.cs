using Microsoft.AspNetCore.Mvc;
using MusicPortal.Models.DataBase;
using MusicPortal.Models.Repository;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<IActionResult> UserConfirmationPartial()
        {
            var users = await _userRepository.GetAllUsers();
            var unconfirmedUsers = users.Where(u => !u.IsConfirmed);
            return PartialView("UserConfirmation", unconfirmedUsers);
        }

        public async Task<IActionResult> UserManagementPartial()
        {
            var users = await _userRepository.GetAllUsers();
            var confirmedUsers = users.Where(u => u.IsConfirmed && !u.IsAdmin);
            return PartialView("UserManagement", confirmedUsers);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmUser(int userId)
        {
            await _userRepository.ConfirmUser(userId);
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> BlockUser(int userId)
        {
            await _userRepository.BlockUser(userId);
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> UnblockUser(int userId)
        {
            await _userRepository.UnblockUser(userId);
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            await _userRepository.DeleteUser(userId);
            return Json(new { success = true });
        }
    }
}
