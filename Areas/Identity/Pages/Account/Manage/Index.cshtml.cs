// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DiaryFriends.Data;
using DiaryFriends.Models;

namespace DiaryFriends.Areas.Identity.Pages.Account.Manage;

public class IndexModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IWebHostEnvironment _webHostEnvironment;


    public IndexModel(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IWebHostEnvironment webHostEnvironment)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _webHostEnvironment = webHostEnvironment;
    }

    /// <summary>
    ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    [TempData]
    public string? StatusMessage { get; set; }

    /// <summary>
    ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    [BindProperty]
    public InputModel Input { get; set; } = default!;

    /// <summary>
    ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class InputModel
    {
        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [Phone]
        [Display(Name = "Phone number")]
        public string? PhoneNumber { get; set; }
        public IFormFile? ProfilePicture { get; set; }
        public string? CurrentProfilePicturePath { get; set; }
        public bool RemoveProfilePicture { get; set; }


        [Required]
        [Display(Name = "First name")]
        public string FirstName { get; set; } = string.Empty;

    }

    private async Task LoadAsync(ApplicationUser user)
    {
        var userName = await _userManager.GetUserNameAsync(user);
        var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

        Username = userName;

        Input = new InputModel
        {
            PhoneNumber = phoneNumber,
            CurrentProfilePicturePath = user.ProfilePicturePath,
            FirstName = user.FirstName
        };
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        await LoadAsync(user);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        if (!ModelState.IsValid)
        {
            await LoadAsync(user);
            return Page();
        }

        if (Input.RemoveProfilePicture && Input.ProfilePicture == null)
        {
            if (!string.IsNullOrEmpty(user.ProfilePicturePath))
            {
                var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, user.ProfilePicturePath.TrimStart('/'));
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }

            user.ProfilePicturePath = null;
            await _userManager.UpdateAsync(user);
        }

        if (Input.ProfilePicture != null)
        {
            const long maxSize = 2 * 1024 * 1024; // 2MB
            var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp" };

            if (Input.ProfilePicture.Length > maxSize)
            {
                ModelState.AddModelError("Input.ProfilePicture", "Picture can not be larger than 2MB.");
                await LoadAsync(user);
                return Page();
            }

            if (!allowedTypes.Contains(Input.ProfilePicture.ContentType))
            {
                ModelState.AddModelError("Input.ProfilePicture", "Only JPG, PNG or WEBP format.");
                await LoadAsync(user);
                return Page();
            }

            using var stream = Input.ProfilePicture.OpenReadStream();
            var buffer = new byte[12];
            await stream.ReadAsync(buffer.AsMemory(0, 12));
            stream.Position = 0;

            bool isValidImage =
                (buffer[0] == 0xFF && buffer[1] == 0xD8) || // JPEG
                (buffer[0] == 0x89 && buffer[1] == 0x50) || // PNG
                (buffer[8] == 0x57 && buffer[9] == 0x45 && buffer[10] == 0x42 && buffer[11] == 0x50); // WEBP

            if (!isValidImage)
            {
                ModelState.AddModelError("Input.ProfilePicture", "File is not a valid image.");
                await LoadAsync(user);
                return Page();
            }

            var fileExtension = Path.GetExtension(Input.ProfilePicture.FileName);
            var fileName = $"{user.Id}_{Guid.NewGuid()}{fileExtension}";
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "profile-pictures");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var filePath = Path.Combine(uploadsFolder, fileName);

            //using jer file mora biti otvoren dok radimo copytoasync
            using (var fileWriteStream = new FileStream(filePath, FileMode.Create))
            {
                await Input.ProfilePicture.CopyToAsync(fileWriteStream);
            }

            //ako korisnik ima staru sliku, ukloni ju
            if (!string.IsNullOrEmpty(user.ProfilePicturePath))
            {
                var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, user.ProfilePicturePath.TrimStart('/'));
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }

            user.ProfilePicturePath = $"/uploads/profile-pictures/{fileName}";
            await _userManager.UpdateAsync(user);
        }

        if (Input.FirstName != user.FirstName)
        {
            user.FirstName = Input.FirstName;
            await _userManager.UpdateAsync(user);
        }
        

        var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
        if (Input.PhoneNumber != phoneNumber)
        {
            var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
            if (!setPhoneResult.Succeeded)
            {
                StatusMessage = "Unexpected error when trying to set phone number.";
                return RedirectToPage();
            }
        }

        await _signInManager.RefreshSignInAsync(user);
        StatusMessage = "Your profile has been updated";
        return RedirectToPage();
    }
}
