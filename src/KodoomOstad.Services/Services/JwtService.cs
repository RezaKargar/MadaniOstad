﻿using KodoomOstad.Common;
using KodoomOstad.Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace KodoomOstad.Services.Services
{
    public class JwtService : IJwtService
    {
        private readonly Settings _siteSetting;
        private readonly SignInManager<User> _signInManager;

        public JwtService(IOptionsSnapshot<Settings> settings, SignInManager<User> signInManager)
        {
            _siteSetting = settings.Value;
            this._signInManager = signInManager;
        }

        public async Task<AccessToken> GenerateAsync(User user)
        {
            // Must be longer that 16 character
            var secretKey = Encoding.UTF8.GetBytes(_siteSetting.JwtSettings.SecretKey);

            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature);

            var claims = await _getClaimsAsync(user);

            var descriptor = new SecurityTokenDescriptor
            {
                Issuer = _siteSetting.JwtSettings.Issuer,
                Audience = _siteSetting.JwtSettings.Audience,
                IssuedAt = DateTime.Now,
                NotBefore = DateTime.Now.AddMinutes(_siteSetting.JwtSettings.NotBeforeMinutes),
                Expires = DateTime.Now.AddMinutes(_siteSetting.JwtSettings.ExpirationMinutes),
                SigningCredentials = signingCredentials,
                Subject = new ClaimsIdentity(claims)
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var securityToken = tokenHandler.CreateJwtSecurityToken(descriptor);

            return new AccessToken(securityToken);
        }

        private async Task<IEnumerable<Claim>> _getClaimsAsync(User user)
        {
            var result = await _signInManager.ClaimsFactory.CreateAsync(user);

            var list = new List<Claim>(result.Claims);

            return list;
        }
    }
}
