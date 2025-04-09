﻿using MediatR;

public class UpdateUserCommandRequest : IRequest<Result<Unit>>
{
    public string? UserId { get; set; }
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public List<string>? Roles { get; set; }
}
