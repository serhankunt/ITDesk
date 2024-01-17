﻿using Azure.Core;
using FluentValidation.Results;
using ITDeskServer.Abstraction;
using ITDeskServer.Context;
using ITDeskServer.DTOs;
using ITDeskServer.Models;
using ITDeskServer.Services;
using ITDeskServer.Validator;
using ITDeskSignInResultNameSpace;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ITDeskServer.Controllers;
[AllowAnonymous]
public class AuthController(
    ApplicationDbContext context,
    UserManager<AppUser> userManager, 
    SignInManager<AppUser> signInManager,
    JwtService jwtService) : ApiController
{
    //public AuthController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
    //{
    //    _userManager = userManager;
    //    _signInManager = signInManager;
    //} yukarıda authcontrolleriçine eklediklerimizden sonra komut satırı içinde olan işlemlere gerek kalmadı
    [HttpPost]
    public async Task<IActionResult> Login(LoginDto request, CancellationToken cancellationToken)
    {
        LoginValidator validator = new();
        ValidationResult validationResult =  validator.Validate(request);
        if (!validationResult.IsValid)
        {
            //string errors = string.Join("\n", validationResult.Errors.Select(s=>s.ErrorMessage));
            return StatusCode(422, validationResult.Errors.Select(s=>s.ErrorMessage));
        }

        AppUser? appUser = await userManager.FindByNameAsync(request.UserNameOrEmail);
        if (appUser is null)
        {
            appUser = await userManager.FindByEmailAsync(request.UserNameOrEmail);
            if(appUser is null)
            {
                return BadRequest(new { Message = "Kullanıcı bulunamadı!" });
            }
        }

        var result = await signInManager.CheckPasswordSignInAsync(appUser, request.Password, true);
        if (result.IsLockedOut)
        {
            TimeSpan? timeSpan = appUser.LockoutEnd - DateTime.UtcNow;
            if (timeSpan is not null)
            {
                return BadRequest(new { Message = $"Kullanıcınız 3 kere yanlış şifre girişinden dolayı {Math.Ceiling(timeSpan.Value.TotalMinutes)} dakika kadar kilitlenmiştir." });
            }
            else
            {
                return BadRequest(new { Message = "Kullanıcınız 3 kere yanlış şifre girişinden dolayı 1 dakika kadar kilitlenmiştir." });
            }
        }

        if (result.IsNotAllowed)
        {
            return BadRequest(new { Message = "mail adresiniz onaylı değil." });
        }
        if (!result.Succeeded)
        {
            return BadRequest(new { Message = "Şifreniz yanlış" });
        }

        var roles = 
            context.AppUserRoles
            .Where(p=>p.UserId == appUser.Id)
            .Include(p=>p.Role)
            .Select(s => s.Role!.Name)
            .ToList();

        string token = jwtService.CreateToken(appUser ,roles , request.RememberMe);

        return Ok(new {AccessToken = token});
    }

    [HttpPost]
    public async Task<IActionResult> GoogleLogin(GoogleLoginDto request,CancellationToken cancellationToken)
    {
        AppUser? appUser = await userManager.FindByEmailAsync(request.Email);
        if(appUser is not null)
        {

            var roles =
                context.AppUserRoles
                .Where(p => p.UserId == appUser.Id)
                .Include(p => p.Role)
                .Select(s => s.Role!.Name)
                .ToList();

            string token = jwtService.CreateToken(appUser,roles, true);
            return Ok(new {AccessToken = token});   
        }

        string userName = request.Email;

        appUser = new()
        {
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            UserName = userName,
            GoogleProvideId = request.Id
        };

        IdentityResult result =  await userManager.CreateAsync(appUser);

        if (result.Succeeded)
        {
            string token = jwtService.CreateToken(appUser,new(), true);
            return Ok(new { AccessToken = token });
        }

        IdentityError? errorResult = result.Errors.FirstOrDefault();

        string errorMessage = errorResult is null ? "Giriş esnasında bir sorunla karşılaştık" : errorResult.Description;
        return BadRequest(new { Message = errorMessage });
    }
}

