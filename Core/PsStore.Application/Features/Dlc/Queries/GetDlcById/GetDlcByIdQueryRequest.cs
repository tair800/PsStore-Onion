﻿using MediatR;

namespace PsStore.Application.Features.Dlc.Queries.GetDlcById
{
    public class GetDlcByIdQueryRequest : IRequest<Result<GetDlcByIdQueryResponse>>
    {
        public int Id { get; set; }
        public bool IncludeDeleted { get; set; } = false;
    }
}
