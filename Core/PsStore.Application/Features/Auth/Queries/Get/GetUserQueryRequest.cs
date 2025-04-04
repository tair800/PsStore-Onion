﻿using MediatR;

namespace PsStore.Application.Features.Auth.Queries.Get
{
    public class GetUserQueryRequest : IRequest<Result<GetUserQueryResponse>>
    {
        public string Email { get; set; }
    }
}
