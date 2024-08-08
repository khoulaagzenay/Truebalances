﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using TrueBalances.Areas.Identity.Data;
using TrueBalances.Data;
using TrueBalances.Models;
using TrueBalances.Repositories.Interfaces;
using TrueBalances.Tools;

namespace TrueBalances.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<CustomUser> _userManager;
        private readonly SignInManager<CustomUser> _signInManager;
        private readonly IProfilePhotoService _profilePhotoService;
        private readonly IServiceProvider _serviceProvider;

        public IndexModel(
            UserManager<CustomUser> userManager,
            SignInManager<CustomUser> signInManager,
            IProfilePhotoService profilePhotoService,
            IServiceProvider serviceProvider)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _profilePhotoService = profilePhotoService;
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            [Required]
            [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Le nom doit contenir des lettres uniquement.")]
            [DataType(DataType.Text)]
            [Display(Name = "Prénom")]
            public string FirstName { get; set; }

            [Required]
            [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Le nom doit contenir des lettres uniquement.")]
            [DataType(DataType.Text)]
            [Display(Name = "Nom")]
            public string LastName { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Phone]
            [Display(Name = "Numero de téléphone")]
            public string PhoneNumber { get; set; }

            [Display(Name = "Profile Photo URL")]
            [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png" })]
            [MaxFileSize(2 * 1024 * 1024)]
            [Display(Name = "Photo de profil")]
            public IFormFile ProfilePhotoFile { get; set; }
        }

        private async Task LoadAsync(CustomUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = phoneNumber
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

            if (Input.FirstName != user.FirstName)
            {
                user.FirstName = Input.FirstName;
            }

            if (Input.LastName != user.LastName)
            {
                user.LastName = Input.LastName;
            }

            if (Input.ProfilePhotoFile != null)
            {
                var profilePhoto = await _profilePhotoService.GetProfilePhoto(user.Id);

                _profilePhotoService.UpdateProfilePhotoFile(Input.ProfilePhotoFile, profilePhoto);
            }

            await _userManager.UpdateAsync(user);
            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Votre profil a été mis à jour.";
            return RedirectToPage();
        }
    }
}
