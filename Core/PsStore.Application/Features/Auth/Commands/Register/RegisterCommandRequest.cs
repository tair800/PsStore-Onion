﻿using MediatR;

namespace PsStore.Application.Features.Auth.Commands.Register
{
    public class RegisterCommandRequest : IRequest<Result<Unit>>
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
