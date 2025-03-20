﻿using MediatR;

namespace PsStore.Application.Features.Auth.Commands.RefreshToken
{
    public class RefresTokenCommandRequest : IRequest<RefresTokenCommandResponse>
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
