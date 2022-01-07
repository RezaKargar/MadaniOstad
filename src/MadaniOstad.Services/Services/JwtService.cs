using MadaniOstad.Common;
using MadaniOstad.Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MadaniOstad.Services.Services
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

            var notBeforeMintues = 0;
            var expiresMintues = 0;

            if (_siteSetting.JwtSettings.NotBeforeMinutes.HasValue && _siteSetting.JwtSettings.NotBeforeMinutes.Value > 0)
            {
                notBeforeMintues = _siteSetting.JwtSettings.NotBeforeMinutes.Value;
                expiresMintues = _siteSetting.JwtSettings.NotBeforeMinutes.Value;
            }

            if (_siteSetting.JwtSettings.ExpirationMinutes.HasValue && _siteSetting.JwtSettings.ExpirationMinutes.Value > 0)
            {
                expiresMintues = _siteSetting.JwtSettings.ExpirationMinutes.Value;
            }

            var descriptor = new SecurityTokenDescriptor
            {
                Issuer = _siteSetting.JwtSettings.Issuer,
                Audience = _siteSetting.JwtSettings.Audience,
                IssuedAt = DateTime.Now,
                NotBefore = DateTime.Now.AddMinutes(notBeforeMintues),
                Expires = DateTime.Now.AddMinutes(expiresMintues),
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

        public JwtSecurityToken ReadToken(string token)
        {
            token = token?.Replace("Bearer ", string.Empty);
            var tokenHandler = new JwtSecurityTokenHandler();
            var readToken = tokenHandler.ReadJwtToken(token);
            return readToken;
        }

        public string GetIdFromToken(string token)
        {
            token = token?.Replace("Bearer ", string.Empty);
            var readToken = ReadToken(token);
            return readToken.Claims.SingleOrDefault(c => c.Type == "nameid")?.Value;
        }
    }
}
